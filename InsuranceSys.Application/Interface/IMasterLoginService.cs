using InsuranceSys.Domain.Entities;

namespace InsuranceSys.Application.Interface
{
    public interface IMasterLoginService
    {
        Task<AgencyUsersEntity?> AuthenticateAsync(string username, string password);
        Task UpdateLastLoginAsync(int userId);
    }
}
