using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.Enums;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure.Repositories
{
    public sealed class SubUsersRepository : ISubUsersService
    {
        private readonly MasterDbContext _masterDb;
        private readonly IConfiguration _configuration;
        private const string MasterConnection = "MasterConnection";

        public SubUsersRepository(MasterDbContext masterDb, IConfiguration configuration)
        {
            _masterDb = masterDb;
            _configuration = configuration;
        }

        public async Task<DataSet> GetSubUsersGridAsync(ImmutableDictionary<string, object> paramCollections)
        {
            var masterConnStr = _configuration.GetConnectionString(MasterConnection)
                ?? throw new InvalidOperationException("MasterConnection not configured.");

            await using var connection = new SqlConnection(masterConnStr);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand("SubUsers_GetAll_By_AgencyId", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };

            foreach (var kv in paramCollections)
                cmd.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);

            var ds = new DataSet();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }
            return ds;
        }

        public async Task<AgencyUsersEntity?> GetSubUserByIdAsync(int userId, int agencyId)
        {
            return await _masterDb.AgencyUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.UserId == userId
                    && u.AgencyId == agencyId
                    && !u.IsDeleted
                    && u.IsSubUser);
        }

        public async Task<(bool Success, string Message, int? UserId)> CreateSubUserAsync(SubUserUpsertDto dto, int agencyId)
        {
            var username = (dto.UserName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(username))
                return (false, "Username is required.", null);

            if (dto.Role < (int)Roles.Agent)
                return (false, "Invalid role selection.", null);

            if (await IsUsernameExistsAsync(username, null))
                return (false, "This username is already in use.", null);

            if (string.IsNullOrWhiteSpace(dto.Password))
                return (false, "Password is required.", null);

            var (hash, salt) = PasswordHasher.HashPassword(dto.Password);

            var entity = new AgencyUsersEntity
            {
                AgencyId = agencyId,
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                FirstName = string.IsNullOrWhiteSpace(dto.FirstName) ? null : dto.FirstName.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
                LastName = string.IsNullOrWhiteSpace(dto.LastName) ? null : dto.LastName.Trim(),
                DisplayName = BuildDisplayName(dto.FirstName, dto.LastName, username),
                Role = dto.Role,
                IsActive = dto.IsActive,
                IsDeleted = false,
                IsSubUser = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _masterDb.AgencyUsers.Add(entity);
            await _masterDb.SaveChangesAsync();
            return (true, "User created successfully.", entity.UserId);
        }

        public async Task<(bool Success, string Message)> UpdateSubUserAsync(SubUserUpsertDto dto, int agencyId)
        {
            var username = (dto.UserName ?? string.Empty).Trim();
            if (dto.UserId <= 0)
                return (false, "Invalid user.");
            if (string.IsNullOrWhiteSpace(username))
                return (false, "Username is required.");
            if (dto.Role < (int)Roles.Agent)
                return (false, "Invalid role selection.");

            var entity = await _masterDb.AgencyUsers
                .FirstOrDefaultAsync(u =>
                    u.UserId == dto.UserId
                    && u.AgencyId == agencyId
                    && !u.IsDeleted
                    && u.IsSubUser);

            if (entity == null)
                return (false, "User not found.");

            if (!string.Equals(entity.Username, username, StringComparison.Ordinal)
                && await IsUsernameExistsAsync(username, dto.UserId))
            {
                return (false, "This username is already in use.");
            }

            entity.Username = username;
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            entity.FirstName = string.IsNullOrWhiteSpace(dto.FirstName) ? null : dto.FirstName.Trim();
            entity.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim();
            entity.LastName = string.IsNullOrWhiteSpace(dto.LastName) ? null : dto.LastName.Trim();
            entity.DisplayName = BuildDisplayName(entity.FirstName, entity.LastName, username);
            entity.Role = dto.Role;
            entity.IsActive = dto.IsActive;
            entity.IsSubUser = true;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
                entity.PasswordHash = hash;
                entity.PasswordSalt = salt;
            }

            await _masterDb.SaveChangesAsync();
            return (true, "User updated successfully.");
        }

        public async Task<bool> SoftDeleteSubUserAsync(int userId, int agencyId)
        {
            var entity = await _masterDb.AgencyUsers
                .FirstOrDefaultAsync(u =>
                    u.UserId == userId
                    && u.AgencyId == agencyId
                    && !u.IsDeleted
                    && u.IsSubUser);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _masterDb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetSubUserActiveAsync(int userId, int agencyId, bool isActive)
        {
            var entity = await _masterDb.AgencyUsers
                .FirstOrDefaultAsync(u =>
                    u.UserId == userId
                    && u.AgencyId == agencyId
                    && !u.IsDeleted
                    && u.IsSubUser);

            if (entity == null)
                return false;

            entity.IsActive = isActive;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _masterDb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUsernameExistsAsync(string username, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var trimmed = username.Trim();
            return await _masterDb.AgencyUsers
                .AnyAsync(u => (excludeUserId == null || u.UserId != excludeUserId.Value) && u.Username == trimmed);
        }

        private static string BuildDisplayName(string? firstName, string? lastName, string usernameFallback)
        {
            var fn = string.IsNullOrWhiteSpace(firstName) ? null : firstName.Trim();
            var ln = string.IsNullOrWhiteSpace(lastName) ? null : lastName.Trim();
            var name = string.Join(' ', new[] { fn, ln }.Where(s => !string.IsNullOrWhiteSpace(s)));
            return string.IsNullOrWhiteSpace(name) ? usernameFallback : name;
        }
    }
}

