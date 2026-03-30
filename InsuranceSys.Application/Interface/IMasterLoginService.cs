using InsuranceSys.Domain.Entities;

namespace InsuranceSys.Application.Interface
{
    public interface IMasterLoginService
    {
        Task<TenantUserEntity?> AuthenticateAsync(string username, string password);
        Task UpdateLastLoginAsync(int userId);
    }
}
