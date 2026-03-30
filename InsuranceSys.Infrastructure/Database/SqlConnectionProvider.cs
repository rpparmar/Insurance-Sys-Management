using InsuranceSys.Application.Interface;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace InsuranceSys.Infrastructure.Database
{
    public class SqlConnectionProvider : ISqlConnectionProvider
    {
        private SqlConnection? _connection;
        private SqlTransaction? _transaction;
        private string? _connectionString;
        private bool _disposed;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantConnectionResolver _tenantConnectionResolver;

        public SqlTransaction? CurrentTransaction => _transaction;

        public SqlConnectionProvider(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ITenantConnectionResolver tenantConnectionResolver)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _tenantConnectionResolver = tenantConnectionResolver;
        }

        public async Task<SqlConnection> GetConnectionAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SqlConnectionProvider));

            // Reuse existing connection if already created in this request
            if (_connection != null)
            {
                // Ensure connection is open (in case it was closed)
                if (_connection.State != ConnectionState.Open)
                    await _connection.OpenAsync();
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
        public async Task<string> GetConnectionStringAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SqlConnectionProvider));

            // Cache connection string to avoid repeated session lookups
            if (_connectionString != null)
                return _connectionString;

            _connectionString = await ResolveTenantConnectionStringAsync();

            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "Tenant connection string not found. User may not be authenticated or tenant is not assigned.");

            return _connectionString;
        }

        private async Task<string?> ResolveTenantConnectionStringAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var tenantIdClaim = user?.FindFirst("TenantId")?.Value;

            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out var tenantId))
                return await _tenantConnectionResolver.GetConnectionStringAsync(tenantId);

            return _configuration.GetConnectionString("MasterConnection");
        }

        /// <summary>
        /// Begins a transaction on the shared connection
        /// </summary>

        public async Task<SqlTransaction> BeginTransactionAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SqlConnectionProvider));

            if (_transaction != null)
                throw new InvalidOperationException(
                    "A transaction is already active. Commit or rollback the current transaction first.");

            var connection = await GetConnectionAsync();
            _transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            return _transaction;
        }

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
