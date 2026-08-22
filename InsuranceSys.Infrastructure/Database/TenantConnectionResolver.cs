using InsuranceSys.Application.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InsuranceSys.Infrastructure.Database
{
    /// <summary>
    /// Resolves and caches tenant-specific SQL Server connection strings based on the authenticated agency context.
    /// </summary>
    public class TenantConnectionResolver : ITenantConnectionResolver
    {
        private readonly ITenantConnectionCache _cache;
        private readonly MasterDbContext _masterDb;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantConnectionResolver> _logger;
        private const string MasterConnection = "MasterConnection";

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantConnectionResolver"/> class.
        /// </summary>
        /// <param name="cache">The tenant connection cache instance.</param>
        /// <param name="masterDb">The master database context.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="logger">The structured logger instance.</param>
        public TenantConnectionResolver(
            ITenantConnectionCache cache,
            MasterDbContext masterDb,
            IConfiguration configuration,
            ILogger<TenantConnectionResolver> logger)
        {
            _cache = cache;
            _masterDb = masterDb;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the SQL connection string for the specified agency, using in-memory cache if present or resolving in real-time from the master database.
        /// </summary>
        /// <param name="agencyId">The unique identifier of the agency (tenant).</param>
        /// <returns>The resolved connection string pointing to the tenant's database.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the agency is inactive, not found, or master connection string is missing.</exception>
        public async Task<string> GetConnectionStringAsync(int agencyId)
        {
            var cached = await _cache.GetConnectionStringAsync(agencyId);
            if (cached != null)
            {
                //_logger.LogInformation("Connection string for AgencyId {AgencyId} retrieved from cache.", agencyId);
                return cached;
            }

            //_logger.LogInformation("Cache miss for AgencyId {AgencyId}. Resolving connection string in real-time from Master DB.", agencyId);

            var tenant = await _masterDb.AgencyDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.AgencyId == agencyId && t.IsActive);

            if (tenant == null)
            {
                _logger.LogWarning("Active agency with ID {AgencyId} not found in Master DB.", agencyId);
                throw new InvalidOperationException($"Active agency with ID {agencyId} not found.");
            }

            var masterConnStr = _configuration.GetConnectionString(MasterConnection)
                ?? throw new InvalidOperationException("MasterConnection not configured.");

            var builder = new SqlConnectionStringBuilder(masterConnStr)
            {
                InitialCatalog = tenant.DatabaseName
            };

            if (!string.IsNullOrWhiteSpace(tenant.DatabaseServer))
            {
                builder.DataSource = tenant.DatabaseServer;
            }

            var connectionString = builder.ConnectionString;
            await _cache.SetConnectionStringAsync(agencyId, connectionString);

            //_logger.LogInformation("Successfully resolved and cached real-time connection string for AgencyId {AgencyId} (Database: {DatabaseName}).", agencyId, tenant.DatabaseName);

            return connectionString;
        }

        /// <summary>
        /// Evicts the cached connection string for the specified agency.
        /// </summary>
        /// <param name="agencyId">The unique identifier of the agency (tenant) to invalidate.</param>
        public async Task InvalidateAsync(int agencyId)
        {
            _logger.LogInformation("Invalidating connection string cache for AgencyId {AgencyId}.", agencyId);
            await _cache.InvalidateAsync(agencyId);
        }
    }
}

