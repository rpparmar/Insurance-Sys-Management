using InsuranceSys.Application.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace InsuranceSys.Infrastructure.Database
{
    /// <summary>
    /// In-memory cache implementation for storing resolved tenant database connection strings.
    /// </summary>
    public class TenantConnectionCache : ITenantConnectionCache
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _slidingExpiration;
        private readonly TimeSpan _absoluteExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantConnectionCache"/> class with configured cache policies.
        /// </summary>
        /// <param name="cache">The memory cache instance.</param>
        /// <param name="configuration">The application configuration containing tenant cache options.</param>
        public TenantConnectionCache(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;

            var slidingMin = configuration.GetValue("TenantCache:SlidingExpirationMinutes", 15);
            var absoluteMin = configuration.GetValue("TenantCache:AbsoluteExpirationMinutes", 60);

            _slidingExpiration = TimeSpan.FromMinutes(slidingMin);
            _absoluteExpiration = TimeSpan.FromMinutes(absoluteMin);
        }

        private static string CacheKey(int tenantId) => $"tenant_conn_{tenantId}";

        /// <summary>
        /// Retrieves the cached connection string for a given tenant.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant.</param>
        /// <returns>The cached connection string, or <c>null</c> if not present in cache.</returns>
        public Task<string?> GetConnectionStringAsync(int tenantId)
        {
            _cache.TryGetValue(CacheKey(tenantId), out string? connStr);
            return Task.FromResult(connStr);
        }

        /// <summary>
        /// Stores a tenant connection string in the cache with configured sliding and absolute expiration policies.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant.</param>
        /// <param name="connectionString">The resolved connection string to cache.</param>
        public Task SetConnectionStringAsync(int tenantId, string connectionString)
        {
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = _slidingExpiration,
                AbsoluteExpirationRelativeToNow = _absoluteExpiration,
                Priority = CacheItemPriority.High
            };

            _cache.Set(CacheKey(tenantId), connectionString, options);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the cached connection string for a tenant.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant to evict from cache.</param>
        public Task InvalidateAsync(int tenantId)
        {
            _cache.Remove(CacheKey(tenantId));
            return Task.CompletedTask;
        }
    }
}

