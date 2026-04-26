using InsuranceSys.Application.DTO;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Application.Interface
{
    public interface IAgencyOnboardingService
    {
        Task<(bool Success, string Message)> OnboardAgencyAsync(OnboardAgencyDto dto, int createdByUserId);
        Task<List<AgencyDetailsDto>> GetAllAgencyAsync();
        Task<DataSet> GetAgencyListGridAsync(ImmutableDictionary<string, object> parameters);
        Task<AgencyDetailsDto?> GetAgencyByIdAsync(int agencyId);
        Task<int> UpdateAgencyDetailsAsync(AgencyDetailsDto dto);
        Task<bool> DeactivateAgencyAsync(int agencyId);
        Task<bool> IsAgencyCodeExistsAsync(string agencyCode, int? excludeAgencyId = null);
        Task<bool> IsAgencyNameExistsAsync(string agencyName, int? excludeAgencyId = null);
        Task<bool> IsAdminUsernameExistsAsync(string username);
        Task<bool> IsDatabaseNameExistsAsync(string databaseName);
    }
}
