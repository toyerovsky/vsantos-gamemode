using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Serverside.Database
{
    public class ForumDatabaseHelper
    {
        #region Nie patrzeć publicznie
        //private readonly string ServerConnectionString = "server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=rp;";
        private readonly string ForumConnectionString = "server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=vsantos;";
        #endregion

        public Tuple<long, short, string> CheckPasswordMatch(string email, string password)
        {
            if (UserExist(email))
            {
                using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
                using (MySqlCommand command = new MySqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT member_id, members_pass_hash, members_pass_salt, member_group_id, mgroup_others FROM core_members WHERE email = @P0";

                    command.Parameters.Add(new MySqlParameter("@P0", email));

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            if (reader.Read())
                            {
                                var id = reader.GetInt64(0);
                                var hash = reader.GetString(1);
                                var salt = reader.GetString(2);
                                var groupid = reader.GetInt16(3);
                                var othergroups = reader.GetString(4);
                                if (hash != "" && salt != "")
                                {

                                    if (hash.Equals(GenerateIpbHash(password, salt)))
                                    {
                                        return Tuple.Create(id, groupid, othergroups);
                                    }
                                    else
                                    {
                                        return Tuple.Create(long.Parse("-1"), short.Parse("-1"), "");
                                    }

                                }
                            }
                            return Tuple.Create(long.Parse("-1"), short.Parse("-1"), "");
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message + " | " + ex.StackTrace);
                        }
                        finally
                        {
                            reader.Close();
                            connection.Close();
                        }
                    }
                }
            }
            return Tuple.Create(long.Parse("-1"), short.Parse("-1"), "");
            //return 1; DEBUG !
        }

        public static string GenerateIpbHash(string plaintext, string salt)
        {
            //return CalculateMD5Hash(CalculateMD5Hash(salt) + CalculateMD5Hash(plaintext));
            return BCrypt.Net.BCrypt.HashPassword(plaintext, "$2a$13$" + salt);
            //return CreateMD5(CreateMD5(plaintext) + "$2a$13$" + salt);
        }

        public bool GetPassword(string login, out string password)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT members_pass_hash FROM core_members WHERE email = @P0";

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

                command.CommandText = "SELECT members_pass_salt FROM core_members WHERE email = @P0";
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

        public string GetCustomField(long id)
        {
            using (MySqlConnection connection = new MySqlConnection(ForumConnectionString))
            using (MySqlCommand command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = "SELECT field_2 FROM core_pfields_content WHERE member_id = @P0";
                command.Parameters.Add(new MySqlParameter("@P0", id));

                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    try
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                        return "";
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
                    "SELECT COUNT(member_id) FROM core_members WHERE email = @P0";
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
