using InsuranceSys.Application.Interface;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InsuranceSys.Infrastructure.Database
{
    public class TenantConnectionResolver : ITenantConnectionResolver
    {
        private readonly ITenantConnectionCache _cache;
        private readonly MasterDbContext _masterDb;

        public TenantConnectionResolver(ITenantConnectionCache cache, MasterDbContext masterDb)
        {
            _cache = cache;
            _masterDb = masterDb;
        }

        public async Task<string> GetConnectionStringAsync(int tenantId)
        {
            var cached = await _cache.GetConnectionStringAsync(tenantId);
            if (cached != null)
                return cached;

            var tenant = await _masterDb.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.IsActive);

            if (tenant == null)
                throw new InvalidOperationException($"Active tenant with ID {tenantId} not found.");

            var password = !string.IsNullOrEmpty(tenant.EncryptedDatabasePassword)
                ? Cryptography.DecryptUtf16(tenant.EncryptedDatabasePassword)
                : string.Empty;

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = tenant.DatabaseServer,
                InitialCatalog = tenant.DatabaseName,
                UserID = tenant.DatabaseUser,
                Password = password,
                Encrypt = true,
                TrustServerCertificate = true,
                MinPoolSize = 2,
                MaxPoolSize = 50,
                ConnectTimeout = 30,
                LoadBalanceTimeout = 300
            };

            var connectionString = builder.ConnectionString;
            await _cache.SetConnectionStringAsync(tenantId, connectionString);
            return connectionString;
        }

        public async Task InvalidateAsync(int tenantId)
        {
            await _cache.InvalidateAsync(tenantId);
        }
    }
}
