using InsuranceSys.Domain.Entities;

namespace InsuranceSys.Application.Interface
{
    public interface IMasterLoginService
    {
        Task<AgencyUsersEntity?> AuthenticateAsync(string username, string password);
        Task UpdateLastLoginAsync(int userId);

        /// <summary>
        /// Verifies the current password and updates the stored hash and salt for the given user.
        /// </summary>
        /// <param name="userId">Authenticated user's id from claims.</param>
        /// <param name="currentPassword">Plain-text current password.</param>
        /// <param name="newPassword">Plain-text new password.</param>
        /// <returns>Success flag and a user-facing message. Never includes password values.</returns>
        Task<(bool Success, string Message)> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword);
    }
}
