using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database
{
    public class SqlConnectionProvider: ISqlConnectionProvider
    {
        private SqlConnection? _connection;
        private SqlTransaction? _transaction;
        private string? _connectionString;
        private bool _disposed;
        private readonly IConfiguration _configuration;
        public SqlTransaction? CurrentTransaction => _transaction;
        public SqlConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Gets the SqlConnection - creates and opens it once per request, reuses for subsequent calls
        /// </summary>
        public async Task<SqlConnection> GetConnectionAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SqlConnectionProvider));

            // ✅ Reuse existing connection if already created in this request
            if (_connection != null)
            {
                // Ensure connection is open (in case it was closed)
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }
                return _connection;
            }

            // Get connection string for tenant
            var connectionString = await GetConnectionStringAsync();

            // Create new connection (only once per request)
            _connection = new SqlConnection(connectionString);

            // Open the connection immediately
            await _connection.OpenAsync();

            return _connection;
        }

        /// <summary>
        /// Gets the connection string for the current tenant
        /// </summary>
        public async Task<string> GetConnectionStringAsync(bool masterDBConn = true)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SqlConnectionProvider));

            // ✅ Cache connection string to avoid repeated session lookups
            if (_connectionString != null)
            {
                return _connectionString;
            }

            // Option 1: From config file
            _connectionString = _configuration.GetConnectionString("MasterConnection");
            if (!masterDBConn)
                _connectionString = _configuration.GetConnectionString("TestConnection");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException(
                    "Tenant connection string not found. User may not be authenticated.");
            }

            return await Task.FromResult(_connectionString);
        }

        /// <summary>
        /// Begins a transaction on the shared connection
        /// </summary>
        public async Task<SqlTransaction> BeginTransactionAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SqlConnectionProvider));

            if (_transaction != null)
            {
                throw new InvalidOperationException(
                    "A transaction is already active. Commit or rollback the current transaction first.");
            }

            var connection = await GetConnectionAsync();
            _transaction = connection.BeginTransaction();

            return _transaction;
        }

        /// <summary>
        /// Properly dispose the SqlConnection and transaction when request ends
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                // Dispose transaction first
                if (_transaction != null)
                {                    
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }

                // Then dispose connection
                _connection?.Dispose();
                _connection = null;
                _connectionString = null;
            }            
            finally
            {
                _disposed = true;
            }
        }
    }
}
