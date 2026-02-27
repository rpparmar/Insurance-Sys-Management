using InsuranceSys.Infrastructure.Database.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database
{
    /// <summary>
    /// Optimized ADO.NET database context that reuses connections per request
    /// </summary>
    public class AdoNetDBContext:IAdoNetDBContext
    {
        private readonly ISqlConnectionProvider _connectionProvider;
        public AdoNetDBContext(ISqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText,
            bool masterDBConn = true)
        {
            // ✅ Reuses shared connection for this request
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            // Add current transaction if active
            if (_connectionProvider.CurrentTransaction != null)
            {
                cmd.Transaction = _connectionProvider.CurrentTransaction;
            }

            // Add parameters
            if (paramCollection != null)
            {
                foreach (var param in paramCollection)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            var result = await cmd.ExecuteNonQueryAsync();
            return result;
        }

        /// <summary>
        /// Executes a query and returns a DataSet
        /// </summary>
        public async Task<DataSet> GetDataSetAsync(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText,
            bool masterDBConn = true)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
            {
                cmd.Transaction = _connectionProvider.CurrentTransaction;
            }

            if (paramCollection != null)
            {
                foreach (var param in paramCollection)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            var ds = new DataSet();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds;            
        }

        /// <summary>
        /// Executes a query and returns a DataTable
        /// </summary>
        public async Task<DataTable> GetDataTableAsync(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText,
            bool masterDBConn = true)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
            {
                cmd.Transaction = _connectionProvider.CurrentTransaction;
            }

            if (paramCollection != null)
            {
                foreach (var param in paramCollection)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            var dt = new DataTable();
            // ✅ Use DataReader with Load() - more efficient for async
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            return dt;            
        }

        /// <summary>
        /// Executes a query and returns a single object mapped from the first row
        /// </summary>
        public async Task<T?> GetObjectAsync<T>(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText,
            bool masterDBConn = true) where T : class, new()
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
            {
                cmd.Transaction = _connectionProvider.CurrentTransaction;
            }

            if (paramCollection != null)
            {
                foreach (var param in paramCollection)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            // ✅ Use CommandBehavior.SingleRow for optimization
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (reader.HasRows)
            {
                var result = MappingGenericObject.MapToObject<T>(reader);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Executes a query and returns a list of objects
        /// </summary>
        public async Task<List<T>> GetListAsync<T>(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText,
            bool masterDBConn = true) where T : class, new()
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
            {
                cmd.Transaction = _connectionProvider.CurrentTransaction;
            }

            if (paramCollection != null)
            {
                foreach (var param in paramCollection)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            using var reader = await cmd.ExecuteReaderAsync();

            var result = MappingGenericObject.MapToList<T>(reader);

            return result;            
        }

        /// <summary>
        /// Executes a scalar query (returns single value)
        /// </summary>
        public async Task<string?> ExecuteScalarAsync(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText,
            bool masterDBConn = true)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
            {
                cmd.Transaction = _connectionProvider.CurrentTransaction;
            }

            if (paramCollection != null)
            {
                foreach (var param in paramCollection)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value
                ? Convert.ToString(result)
                : null;            
        }

        /// <summary>
        /// Executes a scalar query and returns typed result
        /// </summary>
        public async Task<T?> ExecuteScalarAsync<T>(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText,
            bool masterDBConn = true)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
            {
                cmd.Transaction = _connectionProvider.CurrentTransaction;
            }

            if (paramCollection != null)
            {
                foreach (var param in paramCollection)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            var result = await cmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
                return default(T);

            return (T)Convert.ChangeType(result, typeof(T));            
        }

        /// <summary>
        /// Gets the next ID from a sequence
        /// </summary>
        public async Task<int> GetNextIdAsync(string sequenceName)
        {
            if (string.IsNullOrWhiteSpace(sequenceName))
                throw new ArgumentException("Sequence name cannot be null or empty", nameof(sequenceName));

            // ✅ Use parameterized query to prevent SQL injection
            var cmdText = "SELECT NEXT VALUE FOR @SequenceName";

            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);

            // Note: Sequence names cannot be parameterized in SQL Server
            // So we validate the input and use string interpolation carefully
            if (!IsValidSqlIdentifier(sequenceName))
            {
                throw new ArgumentException("Invalid sequence name", nameof(sequenceName));
            }

            cmd.CommandText = $"SELECT NEXT VALUE FOR {sequenceName}";
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);            
        }

        /// <summary>
        /// Validates SQL identifier to prevent injection
        /// </summary>
        private bool IsValidSqlIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return false;

            // SQL identifiers can contain: letters, digits, _, $, #
            // Must start with letter, _, @ or #
            return System.Text.RegularExpressions.Regex.IsMatch(
                identifier,
                @"^[a-zA-Z_@#][a-zA-Z0-9_$#@]*$");
        }
    }
}
