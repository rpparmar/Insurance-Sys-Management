using InsuranceSys.Application.Interface;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InsuranceSys.Infrastructure.Database
{
    public class EFdbContextProvider : IEFdbContextProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantConnectionResolver _tenantConnectionResolver;
        private EfdbContext? _context;
        private string? _connectionString;
        private bool _disposed;

        public EFdbContextProvider(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ITenantConnectionResolver tenantConnectionResolver)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _tenantConnectionResolver = tenantConnectionResolver;
        }

        /// <summary>
        /// Gets the DbContext - creates it once per request, reuses for subsequent calls
        /// </summary>
        public async Task<EfdbContext> GetContextAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EFdbContextProvider));

            // Reuse existing context if already created in this request
            if (_context != null)
                return _context;

            // Get connection string
            var connectionString = await GetConnectionStringAsync();

            // Create new context (only once per request)
            var optionsBuilder = new DbContextOptionsBuilder<EfdbContext>();
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    // Connection resilience
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(30);

                    // Performance optimization
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

            // Disable thread safety checks in production (performance optimization)
#if !DEBUG
            optionsBuilder.EnableThreadSafetyChecks(false);
#endif

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
#endif

            _context = new EfdbContext(optionsBuilder.Options);

            return _context;
        }

        /// <summary>
        /// Gets the connection string for the current tenant
        /// </summary>
        public async Task<string> GetConnectionStringAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EFdbContextProvider));

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
            var tenantIdClaim = user?.FindFirst("AgencyId")?.Value;

            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out var tenantId))
                return await _tenantConnectionResolver.GetConnectionStringAsync(tenantId);

            return _configuration.GetConnectionString("MasterConnection");
        }

        /// <summary>
        /// Properly dispose the DbContext when request ends
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _context?.Dispose();
            _context = null;
            _connectionString = null;
            _disposed = true;
        }
    }
}
