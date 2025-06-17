using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.Extensions.Configuration;

namespace InsuranceSys.Infrastructure
{
    public sealed class AppDBContext : IAppDBContext, IConnectionStringProvider
    {
        private readonly IConfiguration _configuration;
        public AppDBContext(IConfiguration configuration)
		{
            _configuration = configuration;
        }
        
        public async Task<int> ExecuteNonQueryAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true)
		{
			using (SqlConnection conn = new SqlConnection(await GetConnectionStringAsync(masterDBConn)))
			{
                await conn.OpenAsync().ConfigureAwait(false);
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
				{
					cmd.CommandType = cmdType;
					foreach (var param in paramCollection)
					{
						cmd.Parameters.AddWithValue(param.Key, param.Value);
					}
					return await cmd.ExecuteNonQueryAsync();
				}
			}
		}
		public async Task<DataSet> GetDataSetAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true)
		{
			DataSet ds = new DataSet();
			using (SqlConnection conn = new SqlConnection(await GetConnectionStringAsync(masterDBConn)))
			{
                await conn.OpenAsync().ConfigureAwait(false);
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
				{
					cmd.CommandType = cmdType;
					foreach (var param in paramCollection)
					{
						cmd.Parameters.AddWithValue(param.Key, param.Value);
					}
					using (var da = new SqlDataAdapter(cmd))
					{
						da.Fill(ds);						
					}
				}
			}
            return ds;
        }
		public async Task<T> GetObjectAsync<T>(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true) where T : class, new()
		{
			using (SqlConnection conn = new SqlConnection(await GetConnectionStringAsync(masterDBConn)))
			{
                await conn.OpenAsync().ConfigureAwait(false);
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
				{
					cmd.CommandType = cmdType;
					foreach (var param in paramCollection)
					{
						cmd.Parameters.AddWithValue(param.Key, param.Value);
					}
					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						if (reader.HasRows)
							return MappingGenericObject.MapToObject<T>(reader);
						else
							return null;
					}
				}
			}
		}
		public async Task<string?> ExecuteScalarAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true)
		{
			using (SqlConnection conn = new SqlConnection(await GetConnectionStringAsync(masterDBConn)))
			{
                await conn.OpenAsync().ConfigureAwait(false);
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
				{
					cmd.CommandType = cmdType;
					foreach (var param in paramCollection)
					{
						cmd.Parameters.AddWithValue(param.Key, param.Value);
					}
					return Convert.ToString(await cmd.ExecuteScalarAsync());
				}
			}
		}
		public async Task<DataTable> GetDataTableAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true)
		{
			DataTable dt = new DataTable();			
			using (SqlConnection conn = new SqlConnection(await GetConnectionStringAsync(masterDBConn)))
			{
				await conn.OpenAsync().ConfigureAwait(false);
				using (SqlCommand cmd = new SqlCommand(cmdText, conn)) 
				{
					cmd.CommandType = cmdType;
					if (paramCollection != null && paramCollection.Count > 0)
					{
						foreach (var param in paramCollection)
						{
							cmd.Parameters.Add(new SqlParameter(param.Key, param.Value ?? DBNull.Value));
						}
					}
					using (SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
					{
						dt.Load(reader); // More efficient than SqlDataAdapter in async methods
					}
				}
			}
			return dt;
		}

		public async Task<SqlDataReader> GetSqlDataReaderAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true)
		{			
			using (SqlConnection conn = new SqlConnection(await GetConnectionStringAsync(masterDBConn)))
			{
				await conn.OpenAsync().ConfigureAwait(false);
				using (SqlCommand cmd = new SqlCommand(cmdText, conn))
				{
					cmd.CommandType = cmdType;
					if (paramCollection != null && paramCollection.Count > 0)
					{
						foreach (var param in paramCollection)
						{
							cmd.Parameters.Add(new SqlParameter(param.Key, param.Value ?? DBNull.Value));
						}
					}
					return await cmd.ExecuteReaderAsync().ConfigureAwait(false);
				}
			}
		}
        public Task<string> GetConnectionStringAsync(bool masterDBConn=true)
        {
            // Option 1: From config file
            var connStr = _configuration.GetConnectionString("MasterConnection");
            if (!masterDBConn)
                connStr = _configuration.GetConnectionString("TestConnection");

            // Option 2: Lookup from central config DB or cache
            if (string.IsNullOrEmpty(connStr))
                throw new Exception("Invalid client");

            return Task.FromResult(connStr);
        }  
	}

}
