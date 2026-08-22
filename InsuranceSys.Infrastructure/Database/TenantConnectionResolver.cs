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

        public async Task<string> GetConnectionStringAsync(int agencyId)
        {
            var cached = await _cache.GetConnectionStringAsync(agencyId);
            if (cached != null)
                return cached;

            var tenant = await _masterDb.AgencyDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.AgencyId == agencyId && t.IsActive);

            if (tenant == null)
                throw new InvalidOperationException($"Active agency with ID {agencyId} not found.");

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
                ApplicationName = "InsureSysManagement",
                MinPoolSize = 2,
                MaxPoolSize = 50,
                ConnectTimeout = 30,
                LoadBalanceTimeout = 300
            };

            var connectionString = builder.ConnectionString;
            await _cache.SetConnectionStringAsync(agencyId, connectionString);
            return connectionString;
        }

        public async Task InvalidateAsync(int agencyId)
        {
            await _cache.InvalidateAsync(agencyId);
        }
    }
}
