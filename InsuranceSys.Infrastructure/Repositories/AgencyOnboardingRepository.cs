using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using InsuranceSys.Domain.Enums;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class AgencyOnboardingRepository : IAgencyOnboardingService
    {
        private readonly MasterDbContext _masterDb;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AgencyOnboardingRepository> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        private const string MasterConnection = "MasterConnection";
        private sealed class SqlInstanceInfoDto
        {
            public string? InstanceName { get; init; }
            public string? InstanceDefaultDataPath { get; init; }
        }

        public AgencyOnboardingRepository(
            MasterDbContext masterDb,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment,
            ILogger<AgencyOnboardingRepository> logger)
        {
            _masterDb = masterDb;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        private static string GetServerHostNameFromConnectionString(string connectionString)
        {
            var dataSource = new SqlConnectionStringBuilder(connectionString).DataSource?.Trim() ?? string.Empty;
            if (dataSource.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
                dataSource = dataSource[4..];

            // Common forms: HOST, HOST\\INSTANCE, HOST,1433, tcp:HOST,1433
            var hostPart = dataSource.Split('\\', ',')[0].Trim();
            return hostPart;
        }

        public async Task<(bool Success, string Message)> OnboardAgencyAsync(OnboardAgencyDto dto, int createdByUserId)
        {
            var trimmedName = dto.AgencyName.Trim();
            if (await IsAgencyNameExistsAsync(trimmedName))
                return (false, "An agency with this name already exists.");

            if (await IsAdminUsernameExistsAsync(dto.AdminUsername))
                return (false, "This admin username is already in use.");

            var agencyCodeForStorage = string.IsNullOrWhiteSpace(dto.AgencyCode) ? null : dto.AgencyCode.Trim();
            if (agencyCodeForStorage != null && await IsAgencyCodeExistsAsync(agencyCodeForStorage))
                return (false, "Agency code already exists.");

            if (await IsDatabaseNameExistsAsync(dto.DesiredDatabaseName))
                return (false, "Database name already in use.");

            // SQL login name: usr_{agencyCode} when provided; otherwise usr_{sanitized admin username} (username is globally unique).
            var dbUser = BuildDatabaseLoginName(agencyCodeForStorage, dto.AdminUsername);
            var dbPassword = GenerateSecurePassword(20);
            var masterConnStr = _configuration.GetConnectionString(MasterConnection)
                ?? throw new InvalidOperationException("MasterConnection not configured.");
            var serverInstance = GetServerHostNameFromConnectionString(masterConnStr);

            try
            {
                var restoreSuccess = await RestoreAgencyDatabaseAsync(dto.DesiredDatabaseName);

                if (!restoreSuccess)
                {
                    //var fallbackEnabled = _configuration.GetValue<bool>("TenantProvisioning:FallbackToMigrations");
                    //if (fallbackEnabled)
                    //{
                    //    await CreateDatabaseViaMigrationsAsync(dto.DesiredDatabaseName);
                    //}
                    //else
                    //{
                    //    return (false, "Database restore failed and fallback is disabled.");
                    //}
                    return (false, "Database restore failed.");
                }

                await CreateSqlLoginAsync(dto.DesiredDatabaseName, dbUser, dbPassword);

                var encryptedPassword = Cryptography.EncryptUtf16(dbPassword);

                var agencyDetails = new AgencyDetailsEntity
                {
                    AgencyCode = agencyCodeForStorage,
                    AgencyName = trimmedName,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    DatabaseName = dto.DesiredDatabaseName,
                    DatabaseServer = serverInstance,
                    DatabaseUser = dbUser,
                    EncryptedDatabasePassword = encryptedPassword,
                    IsActive = true,
                    CreatedByUserId = createdByUserId
                };

                _masterDb.AgencyDetails.Add(agencyDetails);
                await _masterDb.SaveChangesAsync();

                var (hash, salt) = PasswordHasher.HashPassword(dto.AdminPassword);
                var agencyUser = new AgencyUsersEntity
                {
                    AgencyId = agencyDetails.AgencyId,
                    Username = dto.AdminUsername,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Email = dto.ContactEmail,
                    DisplayName = dto.AgencyName + " Admin",
                    Role = (int)Roles.AgencyAdmin,
                    IsActive = true
                };

                _masterDb.AgencyUsers.Add(agencyUser);
                await _masterDb.SaveChangesAsync();

                return (true, $"Agency '{trimmedName}' onboarded successfully. Database: {dto.DesiredDatabaseName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to onboard agency {AgencyCode}", agencyCodeForStorage ?? "(no code)");
                return (false, $"Onboarding failed: {ex.Message}");
            }
        }

        public async Task<List<AgencyDetailsDto>> GetAllAgencyAsync()
        {
            return await _masterDb.AgencyDetails
                .AsNoTracking()
                .Select(t => new AgencyDetailsDto
                {
                    AgencyId = t.AgencyId,
                    AgencyCode = t.AgencyCode,
                    AgencyName = t.AgencyName,
                    ContactEmail = t.ContactEmail,
                    ContactPhone = t.ContactPhone,
                    DatabaseName = t.DatabaseName,
                    DatabaseServer = t.DatabaseServer,
                    IsActive = t.IsActive,
                    CreatedAtUtc = t.CreatedAtUtc,
                    CreatedByUsername = t.CreatedByUser != null ? t.CreatedByUser.DisplayName : null
                })
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<bool> DeactivateAgencyAsync(int agencyId)
        {
            var agencyDetail = await _masterDb.AgencyDetails.FindAsync(agencyId);
            if (agencyDetail == null) return false;

            agencyDetail.IsActive = false;
            agencyDetail.UpdatedAtUtc = DateTime.UtcNow;
            await _masterDb.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Bracket-safe segment for CREATE LOGIN: letters, digits, underscore, @ (SQL Server allows in quoted identifiers).
        /// </summary>
        private static string BuildDatabaseLoginName(string? agencyCode, string adminUsername)
        {
            if (!string.IsNullOrWhiteSpace(agencyCode))
                return $"usr_{agencyCode.Trim().ToLowerInvariant()}";

            var seg = SanitizeForSqlLoginSegment(adminUsername);
            return $"usr_{seg}";
        }

        /// <summary>
        /// Lowercase, keep [a-z0-9_@], truncate to fit SQL login name limits.
        /// </summary>
        private static string SanitizeForSqlLoginSegment(string username)
        {
            var s = username.Trim().ToLowerInvariant();
            var chars = s.Where(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '@').ToArray();
            var core = new string(chars);
            if (core.Length == 0)
                core = "user";
            return core.Length > 120 ? core[..120] : core;
        }

        public async Task<bool> IsAgencyCodeExistsAsync(string agencyCode)
        {
            if (string.IsNullOrWhiteSpace(agencyCode))
                return false;

            var trimmed = agencyCode.Trim();
            return await _masterDb.AgencyDetails
                .AnyAsync(t => t.AgencyCode != null && t.AgencyCode == trimmed);
        }

        public async Task<bool> IsAgencyNameExistsAsync(string agencyName)
        {
            if (string.IsNullOrWhiteSpace(agencyName))
                return false;

            var trimmed = agencyName.Trim();
            var lower = trimmed.ToLowerInvariant();
            return await _masterDb.AgencyDetails
                .AnyAsync(t => t.AgencyName.ToLower() == lower);
        }

        public async Task<bool> IsAdminUsernameExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var trimmed = username.Trim();
            return await _masterDb.AgencyUsers
                .AnyAsync(u => u.Username == trimmed);
        }

        public async Task<bool> IsDatabaseNameExistsAsync(string databaseName)
        {
            return await _masterDb.AgencyDetails
                .AnyAsync(t => t.DatabaseName == databaseName);
        }

        private async Task<SqlInstanceInfoDto?> GetSqlInstanceInfoAsync(SqlConnection appMasterDbConnection)
        {
            await using var cmd = new SqlCommand("Get_SQL_Instance", appMasterDbConnection)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            string? instanceName = null;
            string? instanceDefaultDataPath = null;

            try
            {
                var instanceNameOrdinal = reader.GetOrdinal("InstanceName");
                if (!reader.IsDBNull(instanceNameOrdinal))
                    instanceName = reader.GetString(instanceNameOrdinal);
            }
            catch (IndexOutOfRangeException)
            {
                // Column not present - leave null; validation will fail below.
            }

            try
            {
                var pathOrdinal = reader.GetOrdinal("InstanceDefaultDataPath");
                if (!reader.IsDBNull(pathOrdinal))
                    instanceDefaultDataPath = reader.GetString(pathOrdinal);
            }
            catch (IndexOutOfRangeException)
            {
                // Column not present - leave null; validation will fail below.
            }

            return new SqlInstanceInfoDto
            {
                InstanceName = instanceName,
                InstanceDefaultDataPath = instanceDefaultDataPath
            };
        }

        private async Task<bool> RestoreAgencyDatabaseAsync(string newDbName)
        {
            var configuredBak = _configuration["TenantProvisioning:BackupFileName"];
            var bakFileName = string.IsNullOrWhiteSpace(configuredBak) ? null : Path.GetFileName(configuredBak);
            var bakFilePath = string.IsNullOrWhiteSpace(bakFileName)
                ? null
                : Path.Combine(_hostEnvironment.ContentRootPath, "App_Data", bakFileName);

            if (string.IsNullOrEmpty(bakFilePath) || !File.Exists(bakFilePath))
            {
                _logger.LogWarning("Backup file not found at {Path}. Falling back.", bakFilePath);
                return false;
            }

            var masterConnStr = _configuration.GetConnectionString(MasterConnection)!;

            // 1) Fetch SQL instance default data path from app master DB via stored procedure.
            await using var appMasterConnection = new SqlConnection(masterConnStr);
            await appMasterConnection.OpenAsync();

            SqlInstanceInfoDto? sqlInstance;
            try
            {
                sqlInstance = await GetSqlInstanceInfoAsync(appMasterConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute stored procedure Get_SQL_Instance.");
                return false;
            }

            var instanceDefaultDataPath = sqlInstance?.InstanceDefaultDataPath?.Trim();
            if (string.IsNullOrWhiteSpace(instanceDefaultDataPath))
            {
                _logger.LogError(
                    "Stored procedure Get_SQL_Instance returned empty InstanceDefaultDataPath. InstanceName={InstanceName}",
                    sqlInstance?.InstanceName);
                return false;
            }

            // 2) Restore operations must be executed in master DB context.
            var restoreConnBuilder = new SqlConnectionStringBuilder(masterConnStr)
            {
                InitialCatalog = "master"
            };

            await using var connection = new SqlConnection(restoreConnBuilder.ConnectionString);
            await connection.OpenAsync();

            var fileListSql = "RESTORE FILELISTONLY FROM DISK = @BakPath";
            string? logicalData = null, logicalLog = null;

            await using (var cmd = new SqlCommand(fileListSql, connection))
            {
                cmd.Parameters.AddWithValue("@BakPath", bakFilePath);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var type = reader["Type"].ToString();
                    var logicalName = reader["LogicalName"].ToString();
                    if (type == "D") logicalData = logicalName;
                    else if (type == "L") logicalLog = logicalName;
                }
            }

            if (logicalData == null || logicalLog == null)
            {
                _logger.LogError("Could not read logical file names from backup.");
                return false;
            }

            var mdfPath = Path.Combine(instanceDefaultDataPath, newDbName + "_Data.mdf");
            var ldfPath = Path.Combine(instanceDefaultDataPath, newDbName + "_Log.ldf");

            _logger.LogInformation(
                "Restoring agency DB {DbName} using instance default path {InstancePath}. MDF={MdfPath} LDF={LdfPath}",
                newDbName,
                instanceDefaultDataPath,
                mdfPath,
                ldfPath);

            var restoreSql = $@"
                RESTORE DATABASE [{newDbName}]
                FROM DISK = @BakPath
                WITH 
                    MOVE @LogicalData TO @MdfPath,
                    MOVE @LogicalLog  TO @LdfPath,
                    REPLACE, STATS = 10;";

            await using (var cmd = new SqlCommand(restoreSql, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@BakPath", bakFilePath);
                cmd.Parameters.AddWithValue("@LogicalData", logicalData);
                cmd.Parameters.AddWithValue("@MdfPath", mdfPath);
                cmd.Parameters.AddWithValue("@LogicalLog", logicalLog);
                cmd.Parameters.AddWithValue("@LdfPath", ldfPath);
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }

        //private async Task CreateDatabaseViaMigrationsAsync(string dbName)
        //{
        //    var masterConnStr = _configuration.GetConnectionString(MasterConnection)!;
        //    var builder = new SqlConnectionStringBuilder(masterConnStr)
        //    {
        //        InitialCatalog = dbName
        //    };

        //    var optionsBuilder = new DbContextOptionsBuilder<EfdbContext>();
        //    optionsBuilder.UseSqlServer(builder.ConnectionString, sql =>
        //    {
        //        sql.EnableRetryOnFailure(3);
        //        sql.CommandTimeout(120);
        //    });

        //    await using var tenantContext = new EfdbContext(optionsBuilder.Options);
        //    await tenantContext.Database.MigrateAsync();

        //    _logger.LogInformation("Agency DB {DbName} created via EF migrations (fallback).", dbName);
        //}

        private async Task CreateSqlLoginAsync(string dbName, string dbUser, string dbPassword)
        {
            var masterConnStr = _configuration.GetConnectionString(MasterConnection)!;
            var builder = new SqlConnectionStringBuilder(masterConnStr)
            {
                InitialCatalog = "master"
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            var sql = $@"
                IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @UserName)
                BEGIN
                    CREATE LOGIN [{dbUser}] WITH PASSWORD = '{dbPassword}', DEFAULT_DATABASE = [{dbName}];
                END

                USE [{dbName}];
                IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @UserName)
                BEGIN
                    CREATE USER [{dbUser}] FOR LOGIN [{dbUser}];
                    ALTER ROLE db_owner ADD MEMBER [{dbUser}];
                END";

            await using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@UserName", dbUser);
            await cmd.ExecuteNonQueryAsync();
        }

        private static string GenerateSecurePassword(int length)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";
            const string all = upper + lower + digits + special;

            var bytes = RandomNumberGenerator.GetBytes(length);
            var chars = new char[length];

            chars[0] = upper[bytes[0] % upper.Length];
            chars[1] = lower[bytes[1] % lower.Length];
            chars[2] = digits[bytes[2] % digits.Length];
            chars[3] = special[bytes[3] % special.Length];

            for (int i = 4; i < length; i++)
                chars[i] = all[bytes[i] % all.Length];

            return new string(chars.OrderBy(_ => RandomNumberGenerator.GetInt32(length)).ToArray());
        }
    }
}
