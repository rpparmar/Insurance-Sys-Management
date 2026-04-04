namespace InsuranceSys.Application.Interface
{
    public interface ITenantConnectionResolver
    {
        Task<string> GetConnectionStringAsync(int agencyId);
        Task InvalidateAsync(int agencyId);
    }
}
