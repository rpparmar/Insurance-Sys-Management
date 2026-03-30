namespace InsuranceSys.Application.Interface
{
    public interface ITenantConnectionCache
    {
        Task<string?> GetConnectionStringAsync(int tenantId);
        Task SetConnectionStringAsync(int tenantId, string connectionString);
        Task InvalidateAsync(int tenantId);
    }
}
