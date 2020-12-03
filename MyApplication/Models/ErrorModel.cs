using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ErrorModel
    {
        private int id;
        public string samAccountName_a;
        public string samAccountName_b;
        public string field_a;
        public string field_b;
        public string value_a;
        public string expectedValue_a;
        public string value_b;

        public ErrorModel(int id)
        {
            GetFromDB(id);
        }

        public ErrorModel(string samAccountName_a, string samAccountName_b, string field_a, string field_b, string value_a, string expectedValue_a, string value_b)
        {
            this.samAccountName_a = samAccountName_a;
            this.samAccountName_b = samAccountName_b;
            this.field_a = field_a;
            this.field_b = field_b;
            this.value_a = value_a;
            this.expectedValue_a = expectedValue_a;
            this.value_b = value_b;
            GetId();
        }

        public ErrorModel(int id, string samAccountName_a, string samAccountName_b, string field_a, string field_b, string value_a, string expectedValue_a, string value_b)
        {
            this.id = id;
            this.samAccountName_a = samAccountName_a;
            this.samAccountName_b = samAccountName_b;
            this.field_a = field_a;
            this.field_b = field_b;
            this.value_a = value_a;
            this.expectedValue_a = expectedValue_a;
            this.value_b = value_b;
        }

        private void GetId()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM t_errors WHERE samaccountname_a=@samaccountname_a AND samaccountname_b=@samaccountname_b AND field_a=@field_a AND field_b=@field_b";
                cmd.Parameters.AddWithValue("@samaccountname_a", samAccountName_a);
                cmd.Parameters.AddWithValue("@samaccountname_b", samAccountName_b);
                cmd.Parameters.AddWithValue("@field_a", field_a);
                cmd.Parameters.AddWithValue("@field_b", field_b);
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        id = reader.GetInt32(reader.GetOrdinal("id"));
                    }
                    else
                    {
                        id = -1;
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private void GetFromDB(int id)
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM t_errors WHERE id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        samAccountName_a = reader.GetString(reader.GetOrdinal("samaccountname_a"));
                        samAccountName_b = reader.GetString(reader.GetOrdinal("samaccountname_b"));
                        field_a = reader.GetString(reader.GetOrdinal("field_a"));
                        field_b = reader.GetString(reader.GetOrdinal("field_b"));
                        value_a = reader.GetString(reader.GetOrdinal("value_a"));
                        expectedValue_a = reader.GetString(reader.GetOrdinal("expected_value_a"));
                        value_b = reader.GetString(reader.GetOrdinal("value_b"));
                    }
                    else
                    {
                        id = -1;
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public static ErrorModel[] GetAll()
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM t_errors";
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        errors.Add(new ErrorModel(
                            reader.GetInt32(reader.GetOrdinal("id")),
                            reader.GetString(reader.GetOrdinal("samaccountname_a")),
                            reader.GetString(reader.GetOrdinal("samaccountname_b")),
                            reader.GetString(reader.GetOrdinal("field_a")),
                            reader.GetString(reader.GetOrdinal("field_b")),
                            reader.GetString(reader.GetOrdinal("value_a")),
                            reader.GetString(reader.GetOrdinal("expected_value_a")),
                            reader.GetString(reader.GetOrdinal("value_b"))
                        ));
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return errors.ToArray();
        }

        private void Insert()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO t_errors(samaccountname_a, samaccountname_b, field_a, field_b, value_a, expected_value_a, value_b)"
                    + " VALUES(@samaccountname_a, @samaccountname_b, @field_a, @field_b, @value_a, @expected_value_a, @value_b);"
                    + "SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@samaccountname_a", samAccountName_a);
                cmd.Parameters.AddWithValue("@samaccountname_b", samAccountName_b);
                cmd.Parameters.AddWithValue("@field_a", field_a);
                cmd.Parameters.AddWithValue("@field_b", field_b);
                cmd.Parameters.AddWithValue("@value_a", value_a);
                cmd.Parameters.AddWithValue("@expected_value_a", expectedValue_a);
                cmd.Parameters.AddWithValue("@value_b", value_b);
                id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private void Update()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE t_errors SET samaccountname_a=@samaccountname_a, samaccountname_b=@samaccountname_b,"
                    + " field_a=@field_a, field_b=@field_b, value_a=@value_a, expected_value_a=@expected_value_a, value_b=@value_b WHERE id=@id";
                cmd.Parameters.AddWithValue("@samaccountname_a", samAccountName_a);
                cmd.Parameters.AddWithValue("@samaccountname_b", samAccountName_b);
                cmd.Parameters.AddWithValue("@field_a", field_a);
                cmd.Parameters.AddWithValue("@field_b", field_b);
                cmd.Parameters.AddWithValue("@value_a", value_a);
                cmd.Parameters.AddWithValue("@expected_value_a", expectedValue_a);
                cmd.Parameters.AddWithValue("@value_b", value_b);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public void InsertOrUpdate()
        {
            if (id == -1)
            {
                Insert();
            }
            else
            {
                Update();
            }
        }
    }
}