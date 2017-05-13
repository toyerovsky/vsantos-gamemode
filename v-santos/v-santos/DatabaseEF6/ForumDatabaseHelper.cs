using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Serverside.DatabaseEF6
{
    public class ForumDatabaseHelper
    {
        #region Nie patrzeć publicznie
        //private readonly string ServerConnectionString = "server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=rp;";
        private readonly string ForumConnectionString = "server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=vsantos;";
        #endregion

        public bool GetPassword(string login, out string password)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT members_pass_hash FROM core_members WHERE name = @P0";

                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            password = reader.GetString(0);
                            return true;
                        }
                        password = string.Empty;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
        }

        public string GetSalt(string login)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText =
                    "SELECT members_pass_salt FROM core_members WHERE name = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                        return String.Empty;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }

        }

        public long GetAid(string login)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText =
                    "SELECT member_id FROM core_members WHERE name = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt64(0);
                        }
                        return long.MinValue;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
        }

        public bool UserExist(string login)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText =
                    "SELECT COUNT(member_id) FROM core_members WHERE name = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", login));

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0) != 0;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }
        }
    }
}
