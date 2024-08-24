using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsuranceSys.Infrastructure;

namespace InsuranceSys.Infrastructure
{
    public sealed class AppDBContext : IAppDBContext
    {
        private readonly string _connectionString;
        public AppDBContext(string conn)
        {
            _connectionString = conn;
        }

        public async Task<int> ExecuteNonQueryAsync(Dictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(string.IsNullOrEmpty(dynamicConnstring) ? _connectionString : dynamicConnstring))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.CommandType = cmdType;
                    foreach (var param in paramCollection)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    try
                    {
                        rowsAffected = await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        //Write log
                    }
                }
                return rowsAffected;
            }
        }

        public Task<DataSet> GetDataSetAsync(Dictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(string.IsNullOrEmpty(dynamicConnstring) ? _connectionString : dynamicConnstring))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.CommandType = cmdType;
                    foreach (var param in paramCollection)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            da.Fill(ds);
                            return Task.FromResult(ds);
                        }
                        catch (Exception ex)
                        {
                            //Write log
                        }
                    }

                }
            }
            return Task.FromResult(ds);
        }

        public async Task<DataTable> GetDataTableAsync(Dictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(string.IsNullOrEmpty(dynamicConnstring) ? _connectionString : dynamicConnstring))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.CommandType = cmdType;
                    foreach (var param in paramCollection)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            da.Fill(dt);
                            return dt;
                        }
                        catch (Exception ex)
                        {
                            //Write log
                        }
                    }

                }
            }
            return dt;
        }
    }
}
