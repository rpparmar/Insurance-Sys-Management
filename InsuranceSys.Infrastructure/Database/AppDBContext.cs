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
using InsuranceSys.Domain.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace InsuranceSys.Infrastructure
{
	public sealed class AppDBContext : IAppDBContext
	{
		private readonly string _connectionString;
		public AppDBContext(string conn)
		{
			_connectionString = conn;
		}

		public async Task<int> ExecuteNonQueryAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
		{
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
					return await cmd.ExecuteNonQueryAsync();
				}
			}
		}
		public Task<DataSet> GetDataSetAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
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
						da.Fill(ds);
						return Task.FromResult(ds);
					}
				}
			}
		}
		public async Task<T> GetObjectAsync<T>(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "") where T : class, new()
		{
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
		public async Task<string?> ExecuteScalarAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
		{
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
					return Convert.ToString(await cmd.ExecuteScalarAsync());
				}
			}
		}
		public async Task<DataTable> GetDataTableAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
		{
			DataTable dt = new DataTable();
			string connString = string.IsNullOrEmpty(dynamicConnstring) ? _connectionString : dynamicConnstring;
			using (SqlConnection conn = new SqlConnection(connString))
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

		public async Task<SqlDataReader> GetSqlDataReaderAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "")
		{
			string connString = string.IsNullOrEmpty(dynamicConnstring) ? _connectionString : dynamicConnstring;
			using (SqlConnection conn = new SqlConnection(connString))
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

	}

}
