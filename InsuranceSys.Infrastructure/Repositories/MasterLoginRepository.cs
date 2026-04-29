using InsuranceSys.Application.Interface;
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
    }
}
