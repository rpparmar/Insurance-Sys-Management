using InsuranceSys.Infrastructure.Database.Interface;
using System.Collections.Immutable;
using System.Data;
using Microsoft.Data.SqlClient;

namespace InsuranceSys.Infrastructure.Database
{
    /// <summary>
    /// Optimized ADO.NET database context that reuses the tenant connection per request.
    /// </summary>
    public class AdoNetDBContext : IAdoNetDBContext
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
            string cmdText)
        {
            // Reuses shared connection for this request
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            // Add current transaction if active
            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            AddParameters(cmd, paramCollection);
            return await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Executes a query and returns a DataSet
        /// </summary>
        public async Task<DataSet> GetDataSetAsync(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            AddParameters(cmd, paramCollection);

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
            string cmdText)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            AddParameters(cmd, paramCollection);

            var dt = new DataTable();
            // Use DataReader with Load() - more efficient for async
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
            string cmdText) where T : class, new()
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            AddParameters(cmd, paramCollection);
            // Use CommandBehavior.SingleRow for optimization
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (reader.HasRows)
                return MappingGenericObject.MapToObject<T>(reader);

            return null;
        }

        /// <summary>
        /// Executes a query and returns a list of objects
        /// </summary>
        public async Task<List<T>> GetListAsync<T>(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText) where T : class, new()
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            AddParameters(cmd, paramCollection);

            using var reader = await cmd.ExecuteReaderAsync();
            return MappingGenericObject.MapToList<T>(reader);
        }

        /// <summary>
        /// Executes a scalar query (returns single value)
        /// </summary>
        public async Task<string?> ExecuteScalarAsync(
            ImmutableDictionary<string, object> paramCollection,
            CommandType cmdType,
            string cmdText)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            AddParameters(cmd, paramCollection);

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
            string cmdText)
        {
            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;

            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            AddParameters(cmd, paramCollection);

            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return default;

            return (T)Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Gets the next ID from a sequence
        /// </summary>
        public async Task<int> GetNextIdAsync(string sequenceName)
        {
            if (string.IsNullOrWhiteSpace(sequenceName))
                throw new ArgumentException("Sequence name cannot be null or empty", nameof(sequenceName));

            // Note: Sequence names cannot be parameterized in SQL Server
            // So we validate the input and use string interpolation carefully
            if (!IsValidSqlIdentifier(sequenceName))
                throw new ArgumentException("Invalid sequence name", nameof(sequenceName));

            var connection = await _connectionProvider.GetConnectionAsync();

            using var cmd = new SqlCommand($"SELECT NEXT VALUE FOR {sequenceName}", connection);

            if (_connectionProvider.CurrentTransaction != null)
                cmd.Transaction = _connectionProvider.CurrentTransaction;

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private static void AddParameters(SqlCommand cmd, ImmutableDictionary<string, object>? paramCollection)
        {
            if (paramCollection == null) return;
            foreach (var param in paramCollection)
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }

        /// <summary>
        /// Validates SQL identifier to prevent injection
        /// </summary>
        private static bool IsValidSqlIdentifier(string identifier)
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
