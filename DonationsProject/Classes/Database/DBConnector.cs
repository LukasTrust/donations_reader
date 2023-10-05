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

        #region WriteToDB
        public async Task InsertDonations(List<Donation> donations)
        {
            if (connection == null)
            {
                Connect();
            }

            // Insert into Dates table
            List<int> dateReceiptId = await InsertDonationDate(donations.Select(x => x.ReceiptDate).ToList());

            List<int> dateReportId = await InsertReportDate(donations.Select(x => x.ReportLink).ToList());

            // Insert into Donors table
            List<int> donorId = await InsertDonor(donations.Select(x => x.Donor).ToList());

            // Insert into Parties table
            List<int> partyId = await InsertParty(donations.Select(x => x.Party).ToList());

            List<int> amountId = await InsertAmount(donations.Select(x => x.Amount).ToList());

            // Insert into Donations table
            await InsertDonation(dateReceiptId, dateReportId, donorId, partyId, amountId);
        }

        private async Task ClearAllTables()
        {
            if (connection == null)
            {
                Connect();
            }

            string[] tableNames = { "Donations", "Parties", "DonationDates", "ReportDates", "Amounts", "Donors" };

            foreach (var tableName in tableNames)
            {
                string deleteQuery = $"DELETE FROM {tableName}";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<List<int>> Insert(string tableName, string columnName, List<object> values)
        {
            List<int> ids = new List<int>();
            List<object> distinctValues = values.Distinct().ToList();
            List<(int Id, object Value)> existingKeys = await ReadDataFromDB(tableName, columnName);
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

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    await insertCommand.ExecuteNonQueryAsync();
                }

                existingKeys = await ReadDataFromDB(tableName, columnName);
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

        private async Task<List<(int Id, object Value)>> ReadDataFromDB(string tableName, string columnName)
        {
            List<(int Id, object Value)> existingKeys = new List<(int, object Value)>();
            string selectExistingKeyQuery = $"SELECT Id, {columnName} FROM {tableName}";

            using (SqlCommand command = new SqlCommand(selectExistingKeyQuery, connection))
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);  // Assuming Id is in the first column
                    object value = reader.GetValue(1);  // Assuming the value is in the second column

                    existingKeys.Add((id, value));
                }
            }
            return existingKeys;
        }


        private async Task<List<int>> InsertDonationDate(List<DateTime> dates)
        {
            List<object> list = dates.Cast<object>().ToList();
            return await Insert("DonationDates", "Date", list);
        }

        private async Task<List<int>> InsertReportDate(List<DateTime> dates)
        {
            List<object> list = dates.Cast<object>().ToList();
            return await Insert("ReportDates", "Date", list);
        }

        private async Task<List<int>> InsertDonor(List<string> donors)
        {
            List<object> list = donors.Cast<object>().ToList();
            return await Insert("Donors", "Name", list);
        }

        private async Task<List<int>> InsertParty(List<string> parties)
        {
            List<object> list = parties.Cast<object>().ToList();
            return await Insert("Parties", "Name", list);
        }

        private async Task<List<int>> InsertAmount(List<double> amounts)
        {
            List<object> list = amounts.Cast<object>().ToList();
            return await Insert("Amounts", "Amount", list);
        }

        private async Task InsertDonation(List<int> dateReceiptIds, List<int> dateReportIds, List<int> donorIds, List<int> partyIds, List<int> amounts)
        {
            int itemCount = dateReceiptIds.Count;
            if (itemCount != dateReportIds.Count || itemCount != donorIds.Count || itemCount != partyIds.Count || itemCount != amounts.Count)
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

                using (SqlCommand command = new SqlCommand(insertDonationQuery.ToString(), connection))
                {
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        command.Parameters.AddWithValue($"@FK_DonationDate{i}", dateReceiptIds[i]);
                        command.Parameters.AddWithValue($"@FK_ReportDate{i}", dateReportIds[i]);
                        command.Parameters.AddWithValue($"@FK_Donor{i}", donorIds[i]);
                        command.Parameters.AddWithValue($"@FK_Party{i}", partyIds[i]);
                        command.Parameters.AddWithValue($"@FK_Amount{i}", amounts[i]);
                    }

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        #endregion
    }
}
