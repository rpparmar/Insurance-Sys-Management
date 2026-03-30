using InsuranceSys.Application.DTO;
using InsuranceSys.Domain.Entities;

namespace InsuranceSys.Application.Interface
{
    public interface ITenantOnboardingService
    {
        Task<(bool Success, string Message)> OnboardTenantAsync(OnboardTenantDto dto, int createdByUserId);
        Task<List<TenantInfoDto>> GetAllTenantsAsync();
        Task<bool> DeactivateTenantAsync(int tenantId);
        Task<bool> IsTenantCodeExistsAsync(string tenantCode);
        Task<bool> IsDatabaseNameExistsAsync(string databaseName);
    }
}
