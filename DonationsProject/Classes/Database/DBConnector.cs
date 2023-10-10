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
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace DonationsProject.Classes.Database
{
    public class DBConnector
    {
        private static SqlConnection connection = null;
        private const string ConnectionString = @"Server=PC-LUKAS-SCHELE\SQLSERVER;Database=PartyDonations;User Id=Programm;Password=password;";
        private const string InsertQuery = "INSERT INTO {0} ({1}) OUTPUT INSERTED.{2} VALUES ({3})";
        private const string DonationTable = "Donations";
        private const string DonationDateTable = "DonationDates";
        private const string ReportDateTable = "ReportDates";
        private const string DonorTable = "Donors";
        private const string PartyTable = "Parties";
        private const string AmountTable = "Amounts";

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

        public List<int> DateDonationIds { get; set; }
        public List<int> DateReportIds { get; set; }
        public List<int> DonorIds { get; set; }
        public List<int> PartyIds { get; set; }
        public List<int> AmountIds { get; set; }

        #region WriteToDB
        public async Task InsertDonations(List<Donation> donations)
        {
            if (connection == null)
            {
                Connect();
            }

            await ClearAllTablesAsync();

            // Insert into DonationDates table
            DateDonationIds = await InsertDonationDate(donations.Select(x => x.DonationDate).ToList());

            // Insert into ReportDates table
            DateReportIds = await InsertReportDate(donations.Select(x => x.ReportDate).ToList());

            // Insert into Donors table
            DonorIds = await InsertDonor(donations.Select(x => x.Donor).ToList());

            // Insert into Parties table
            PartyIds = await InsertParty(donations.Select(x => x.Party).ToList());

            AmountIds = await InsertAmount(donations.Select(x => x.Amount).ToList());

            // Insert into Donations table
            await InsertDonation();
        }

        private async Task<List<int>> Insert(string tableName, string columnName, List<object> values)
        {
            List<int> ids = new List<int>();
            List<object> distinctValues = values.Distinct().ToList();
            List<(int Id, object Value)> existingKeys = await ReadDataDetailTableFromDB(tableName, columnName);
            List<object> toInsert = new List<object>();

            foreach (object value in distinctValues)
            {
                if (!existingKeys.Any(x => x.Value.Equals(value)))
                {
                    if (value is double)
                    {
                        string valueString = ((double)value).ToString();
                        valueString = valueString.Replace(",", ".");
                        toInsert.Add(valueString);
                    }
                    else
                    {
                        toInsert.Add(value.ToString());
                    }
                }
            }

            if (toInsert.Count != 0)
            {
                string insertQuery = $"INSERT INTO {tableName} ({columnName}) VALUES ";

                for (int i = 0; i < toInsert.Count; i++)
                {
                    insertQuery += $"('{toInsert[i]}')";
                    if (i < toInsert.Count - 1)
                        insertQuery += ",";
                }

                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                await insertCommand.ExecuteNonQueryAsync();


                existingKeys = await ReadDataDetailTableFromDB(tableName, columnName);
            }

            foreach (object value in values)
            {
                foreach (var existingValue in existingKeys)
                {
                    if (existingValue.Value.ToString() == value.ToString())
                    {
                        ids.Add(existingValue.Id);
                        break;
                    }
                }
            }
            return ids;
        }

        private async Task<List<int>> InsertDonationDate(List<DateTime> dates)
        {
            List<object> list = dates.Cast<object>().ToList();
            return await Insert(DonationDateTable, "Date", list);
        }

        private async Task<List<int>> InsertReportDate(List<DateTime> dates)
        {
            List<object> list = dates.Cast<object>().ToList();
            return await Insert(ReportDateTable, "Date", list);
        }

        private async Task<List<int>> InsertDonor(List<string> donors)
        {
            List<object> list = donors.Cast<object>().ToList();
            return await Insert(DonorTable, "Name", list);
        }

        private async Task<List<int>> InsertParty(List<string> parties)
        {
            List<object> list = parties.Cast<object>().ToList();
            return await Insert(PartyTable, "Name", list);
        }

        private async Task<List<int>> InsertAmount(List<double> amounts)
        {
            List<object> list = amounts.Cast<object>().ToList();
            return await Insert(AmountTable, "Amount", list);
        }

        private async Task InsertDonation()
        {
            int itemCount = DateDonationIds.Count;
            if (itemCount != DateReportIds.Count || itemCount != DonorIds.Count || itemCount != PartyIds.Count || itemCount != AmountIds.Count)
            {
                throw new ArgumentException("Input lists must have the same count.");
            }

            int batchSize = 100;

            int numBatches = (int)Math.Ceiling((double)itemCount / batchSize);

            for (int batchIndex = 0; batchIndex < numBatches; batchIndex++)
            {
                int startIndex = batchIndex * batchSize;
                int endIndex = Math.Min((batchIndex + 1) * batchSize, itemCount);

                StringBuilder insertDonationQuery = new StringBuilder("INSERT INTO Donations (FK_DonationDate, FK_ReportDate, FK_Donor, FK_Party, FK_Amount) VALUES ");

                for (int i = startIndex; i < endIndex; i++)
                {
                    insertDonationQuery.Append($"(@FK_DonationDate{i}, @FK_ReportDate{i}, @FK_Donor{i}, @FK_Party{i}, @FK_Amount{i})");
                    if (i < endIndex - 1)
                        insertDonationQuery.Append(",");
                }

                SqlCommand command = new SqlCommand(insertDonationQuery.ToString(), connection);

                for (int i = startIndex; i < endIndex; i++)
                {
                    command.Parameters.AddWithValue($"@FK_DonationDate{i}", DateDonationIds[i]);
                    command.Parameters.AddWithValue($"@FK_ReportDate{i}", DateReportIds[i]);
                    command.Parameters.AddWithValue($"@FK_Donor{i}", DonorIds[i]);
                    command.Parameters.AddWithValue($"@FK_Party{i}", PartyIds[i]);
                    command.Parameters.AddWithValue($"@FK_Amount{i}", AmountIds[i]);
                }

                await command.ExecuteNonQueryAsync();

            }
        }
        #endregion

        #region Read From DB

        public async Task ReadDataFromDB()
        {
            if (connection == null)
            {
                Connect();
            }

            string selectExistingKeyQuery = String.Format("SELECT FK_DonationDate, FK_ReportDate, FK_Donor, FK_Party, FK_Amount From {0}", DonationTable);
            SqlCommand command = new SqlCommand(selectExistingKeyQuery, connection);
            List<int[]> donationsIds = new List<int[]>();

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int[] donation = new int[5];
                    for (int i = 0; i < 5; i++)
                    {
                        donation[i] = reader.GetInt32(i);
                    }
                    donationsIds.Add(donation);
                }
            }

            List<(int Id, object Value)> donationDateTable = await ReadDataDetailTableFromDB(DonationDateTable, "Date");
            List<(int Id, object Value)> reportDateTable = await ReadDataDetailTableFromDB(ReportDateTable, "Date");
            List<(int Id, object Value)> donorTable = await ReadDataDetailTableFromDB(DonorTable, "Name");
            List<(int Id, object Value)> partyTable = await ReadDataDetailTableFromDB(PartyTable, "Name");
            List<(int Id, object Value)> amountTable = await ReadDataDetailTableFromDB(AmountTable, "Amount");

            List<object[]> donations = new List<object[]>();

            foreach (int[] donationIds in donationsIds)
            {
                object[] donation = new object[5];
                for (int i = 0; i < 5; i++)
                {
                    switch (i)
                    {
                        case 0:
                            donation[i] = donationDateTable.FirstOrDefault(x => x.Id == donationIds[i]).Value;
                            break;
                        case 1:
                            donation[i] = reportDateTable.FirstOrDefault(x => x.Id == donationIds[i]).Value;
                            break;
                        case 2:
                            donation[i] = donorTable.FirstOrDefault(x => x.Id == donationIds[i]).Value;
                            break;
                        case 3:
                            donation[i] = partyTable.FirstOrDefault(x => x.Id == donationIds[i]).Value;
                            break;
                        case 4:
                            donation[i] = amountTable.FirstOrDefault(x => x.Id == donationIds[i]).Value;
                            break;
                    }
                }
                donations.Add(donation);
            }
            if (Donation.Donations.Count != 0)
            {
                Donation.Donations.Clear();
            }

            foreach (object[] donation in donations)
            {
                await Donation.CreateDonationFromDB(donation);
            }
        }

        private async Task<List<(int Id, object Value)>> ReadDataDetailTableFromDB(string tableName, string columnName)
        {
            List<(int Id, object Value)> existingKeys = new List<(int, object Value)>();
            string selectExistingKeyQuery = $"SELECT Id, {columnName} FROM {tableName}";

            SqlCommand command = new SqlCommand(selectExistingKeyQuery, connection);

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    object value = reader.GetValue(1);

                    existingKeys.Add((id, value));
                }
            }

            return existingKeys;
        }
        #endregion

        #region Clear Tables

        public async Task ClearAllTablesAsync()
        {
            if (connection == null)
            {
                Connect();
            }

            string[] tableNames = { DonationTable, DonationDateTable, ReportDateTable, AmountTable, DonorTable, PartyTable }; 
            foreach (var tableName in tableNames)
            {
                string deleteQuery = $"DELETE FROM {tableName}";
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);

                try
                {
                    await deleteCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"Cleared data from {tableName}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to clear data from {tableName}: {ex.Message}");
                }
            }
        }
    }

    #endregion
}
