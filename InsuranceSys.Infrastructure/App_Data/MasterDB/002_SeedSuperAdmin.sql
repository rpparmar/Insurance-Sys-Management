/*
    Seed script: Insert the default SuperAdmin user.
    
    Password: SuperAdmin@2026#
    Hashing: PBKDF2-SHA256, 100K iterations, 16-byte random salt.
    
    IMPORTANT: The PasswordHash and PasswordSalt below are placeholders.
    The actual seeding is performed by the application on first startup
    via MasterDbContext seeding logic (which uses PasswordHasher.cs).
    
    To manually seed, run the application once or use the C# seed utility.
    This script inserts the row with a marker that the app replaces on boot.
*/

USE [InsureSysManagement_Master];
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[TenantUsers] WHERE [Username] = N'superadmin')
BEGIN
    INSERT INTO [dbo].[TenantUsers]
    (
        [TenantId],
        [Username],
        [PasswordHash],
        [PasswordSalt],
        [Email],
        [DisplayName],
        [Role],
        [IsActive],
        [CreatedAtUtc]
    )
    VALUES
    (
        NULL,                           -- SuperAdmin is not bound to any tenant
        N'superadmin',
        N'PENDING_APP_SEED',            -- Replaced by PasswordHasher on first app start
        N'PENDING_APP_SEED',            -- Replaced by PasswordHasher on first app start
        N'admin@insuresys.local',
        N'System Administrator',
        N'SuperAdmin',
        1,
        SYSUTCDATETIME()
    );

    PRINT 'SuperAdmin user seeded. Run the application to finalize password hash.';
END
ELSE
BEGIN
    PRINT 'SuperAdmin user already exists. Skipping seed.';
END
GO
