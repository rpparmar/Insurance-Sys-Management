namespace InsuranceSys.Application.Interface
{
    public interface ITenantConnectionResolver
    {
        Task<string> GetConnectionStringAsync(int tenantId);
        Task InvalidateAsync(int tenantId);
    }
}
