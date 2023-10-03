using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DonationsProject.Classes.Database
{
    public class DBConnector
    {
        private static DbConnection connection = null;
        private const string ConnectionString = @"Server=PC-LUKAS-SCHELE\SQLSERVER;Database=PartyDonations;User Id=Programm;Password=password;";
        private const string SelectQuery = "SELECT * FROM Partys";


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

        public void ExecuteQuery(string query)
        {
            if (connection == null)
            {
                Connect();
            }
            DbCommand command = connection.CreateCommand();
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }

        public void GetDate()
        {
            ExecuteQuery(SelectQuery);
        }
    }
}
