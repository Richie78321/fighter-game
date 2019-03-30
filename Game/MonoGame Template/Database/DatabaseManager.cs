using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace FighterGame.Database
{
    public class DatabaseManager
    {
        private const string DB_CONNECTION_STRING_NAME = "FighterGameDB";

        private enum AccountColumns
        {
            ID,
            Username,
            Password
        }

        //Object
        private SqlConnection sqlConnection;

        public DatabaseManager()
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[DB_CONNECTION_STRING_NAME].ConnectionString);
        }

        private string username = "NOT LOGGED IN!";
        public string Username => username;
        private int userIndex = -1;
        public bool LoggedIn => userIndex != -1;
        public int UserIndex => userIndex;

        public async Task<bool> AttemptLogin(string username, string password)
        {
            if (!LoggedIn)
            {
                //Gather from sql connection
                SqlCommand command = new SqlCommand("SELECT * FROM Accounts WHERE Username='" + username + "';", sqlConnection);
                await command.Connection.OpenAsync();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader.GetString((int)AccountColumns.Password).Equals(password))
                    {
                        userIndex = reader.GetInt32((int)AccountColumns.ID);
                        command.Connection.Close();
                        this.username = username;
                        return true;
                    }
                }

                command.Connection.Close();
                return false;
            }
            else return false;
        }

        public async Task<bool> CreateAccount(string username, string password)
        {
            if (!LoggedIn)
            {
                //Gather from sql connection
                SqlCommand command = new SqlCommand("SELECT Username FROM Accounts WHERE Username='" + username + "';", sqlConnection);
                await command.Connection.OpenAsync();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    command.Connection.Close();
                    return false;
                }
                else
                {
                    command.Connection.Close();
                    command = new SqlCommand("INSERT INTO Accounts (Username, Password) VALUES ('" + username + "', '" + password + "');", sqlConnection);
                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    command.Connection.Close();
                    return true;
                }
            }
            else return false;
        }
    }
}
