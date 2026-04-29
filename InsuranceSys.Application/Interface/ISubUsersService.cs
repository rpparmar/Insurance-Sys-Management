using InsuranceSys.Application.DTO;
using InsuranceSys.Domain.Entities;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Application.Interface
{
    public interface ISubUsersService
    {
        Task<DataSet> GetSubUsersGridAsync(ImmutableDictionary<string, object> paramCollections);
        Task<AgencyUsersEntity?> GetSubUserByIdAsync(int userId, int agencyId);

        Task<(bool Success, string Message, int? UserId)> CreateSubUserAsync(SubUserUpsertDto dto, int agencyId);
        Task<(bool Success, string Message)> UpdateSubUserAsync(SubUserUpsertDto dto, int agencyId);

        Task<bool> SoftDeleteSubUserAsync(int userId, int agencyId);
        Task<bool> SetSubUserActiveAsync(int userId, int agencyId, bool isActive);

        Task<bool> IsUsernameExistsAsync(string username, int? excludeUserId = null);
    }
}

