using InsuranceSys.Application.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace InsuranceSys.Infrastructure.Database
{
    public class TenantConnectionCache : ITenantConnectionCache
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _slidingExpiration;
        private readonly TimeSpan _absoluteExpiration;

        public TenantConnectionCache(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;

            var slidingMin = configuration.GetValue("TenantCache:SlidingExpirationMinutes", 15);
            var absoluteMin = configuration.GetValue("TenantCache:AbsoluteExpirationMinutes", 60);

            _slidingExpiration = TimeSpan.FromMinutes(slidingMin);
            _absoluteExpiration = TimeSpan.FromMinutes(absoluteMin);
        }

        private static string CacheKey(int tenantId) => $"tenant_conn_{tenantId}";

        public Task<string?> GetConnectionStringAsync(int tenantId)
        {
            _cache.TryGetValue(CacheKey(tenantId), out string? connStr);
            return Task.FromResult(connStr);
        }

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

        public Task InvalidateAsync(int tenantId)
        {
            _cache.Remove(CacheKey(tenantId));
            return Task.CompletedTask;
        }
    }
}
