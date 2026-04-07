using InsuranceSys.Application.DTO;
using InsuranceSys.Domain.Entities;

namespace InsuranceSys.Application.Interface
{
    public interface IAgencyOnboardingService
    {
        Task<(bool Success, string Message)> OnboardAgencyAsync(OnboardAgencyDto dto, int createdByUserId);
        Task<List<AgencyDetailsDto>> GetAllAgencyAsync();
        Task<bool> DeactivateAgencyAsync(int agencyId);
        Task<bool> IsAgencyCodeExistsAsync(string agencyCode);
        Task<bool> IsAgencyNameExistsAsync(string agencyName);
        Task<bool> IsAdminUsernameExistsAsync(string username);
        Task<bool> IsDatabaseNameExistsAsync(string databaseName);
    }
}
