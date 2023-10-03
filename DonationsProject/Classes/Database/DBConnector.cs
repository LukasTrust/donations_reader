using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DonationsProject.Classes.Objects;
using OpenQA.Selenium;
using System.Windows.Media;

namespace DonationsProject.Classes.Database
{
    public class DBConnector
    {
        private static SqlConnection connection = null;
        private const string ConnectionString = @"Server=PC-LUKAS-SCHELE\SQLSERVER;Database=PartyDonations;User Id=Programm;Password=password;";
        private const string InsertQuery = "INSERT INTO {0} ({1}) OUTPUT INSERTED.{2} VALUES ({3})";

        public static DBConnector Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DBConnector();

                }
                return _Instance;
            }
        }

        private static DBConnector _Instance { get; set; }
        public void Connect()
        {
            if (connection == null)
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();
            }
        }

        public async Task InsertDonations(List<Donation> donations)
        {
            if (connection == null)
            {
                Connect();
            }
            foreach (Donation donation in donations)
            {
                // Insert into Dates table
                int dateReceiptId = await InsertDate(donation.ReceiptDate);

                int dateReportId = await InsertDate(donation.ReportLink);

                // Insert into Donors table
                int donorId = await InsertDonor(donation.Donor);

                // Insert into Parties table
                int partyId = await InsertParty(donation.Party);

                // Insert into Donations table
                await InsertDonation(dateReceiptId, dateReportId, donorId, partyId, donation.Amount);
            }

        }

        private async Task<int> Insert(string tableName, string columnName, string parameterName, object value)
        {
            string insertQuery = $"INSERT INTO {tableName} ({columnName}) OUTPUT INSERTED.Id VALUES (@{parameterName})";

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                await Task.Run(() =>
                {
                    command.Parameters.AddWithValue("@" + parameterName, value);
                });

                try
                {
                    return (int)command.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint violation error number
                    {
                        // Handle unique constraint violation error by returning the existing key
                        string selectExistingKeyQuery = $"SELECT Id FROM {tableName} WHERE {columnName} = @{parameterName}";
                        using (SqlCommand selectCommand = new SqlCommand(selectExistingKeyQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@" + parameterName, value);
                            return (int)selectCommand.ExecuteScalar();
                        }
                    }
                    else
                    {
                        // Handle other SQL exceptions
                        Console.WriteLine("An error occurred: " + ex.Message);
                        throw; // Rethrow the exception for other types of errors
                    }
                }
            }
        }


        private async Task<int> InsertDate(DateTime date)
        {
            return await Insert("Dates", "Date", "Date", date);
        }

        private async Task<int> InsertDonor(string donor)
        {
            return await Insert("Doners", "Name", "Name", donor);
        }

        private async Task<int> InsertParty(string party)
        {
            return await Insert("Partys", "Name", "Name", party);
        }

        private async Task InsertDonation(int dateReceiptId, int dateReportId, int donorId, int partyId, double amount)
        {
            string insertDonationQuery = string.Format(InsertQuery, "Donations", "FK_DonationDate, FK_ReportDate, FK_Doner, FK_Party, Amount", "Id", "@FK_DonationDate, @FK_ReportDate, @FK_Doner, @FK_Party, @Amount");
            SqlCommand command = new SqlCommand(insertDonationQuery, connection);
            await Task.Run(() =>
            {
                command.Parameters.AddWithValue("@FK_DonationDate", dateReceiptId);
                command.Parameters.AddWithValue("@FK_ReportDate", dateReportId);
                command.Parameters.AddWithValue("@FK_Doner", donorId);
                command.Parameters.AddWithValue("@FK_Party", partyId);
                command.Parameters.AddWithValue("@Amount", amount);

                command.ExecuteNonQuery();
            });
        }
    }
}
