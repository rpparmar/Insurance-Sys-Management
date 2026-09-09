using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class MasterLoginRepository : IMasterLoginService
    {
        private readonly MasterDbContext _masterDb;

        public MasterLoginRepository(MasterDbContext masterDb)
        {
            _masterDb = masterDb;
        }

        public async Task<AgencyUsersEntity?> AuthenticateAsync(string username, string password)
        {
            var user = await _masterDb.AgencyUsers
                .AsNoTracking()
                .Include(u => u.AgencyDetails)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

            if (user == null)
                return null;

            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _masterDb.AgencyUsers.FindAsync(userId);
            if (user != null)
            {
                user.LastLoginAtUtc = DateTime.UtcNow;
                await _masterDb.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public async Task<(bool Success, string Message)> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword)
        {
            if (userId <= 0
                || string.IsNullOrWhiteSpace(currentPassword)
                || string.IsNullOrWhiteSpace(newPassword))
            {
                return (false, Constants.ErrorMessages.MsgPasswordChangeFailed);
            }

            var user = await _masterDb.AgencyUsers
                .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

            if (user is null || !user.IsActive)
                return (false, Constants.ErrorMessages.MsgPasswordChangeFailed);

            if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
                return (false, Constants.ErrorMessages.MsgCurrentPasswordIncorrect);

            var (hash, salt) = PasswordHasher.HashPassword(newPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _masterDb.SaveChangesAsync();
            return (true, Constants.SuccessMessages.MsgPasswordChanged);
        }
    }
}
