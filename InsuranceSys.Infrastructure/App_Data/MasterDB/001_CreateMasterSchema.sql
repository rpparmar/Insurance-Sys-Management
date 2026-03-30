/*
    Master Database Schema: InsureSysManagement_Master
    Creates the Tenants and TenantUsers tables with indexes and constraints.
    Run this script against a fresh database named InsureSysManagement_Master.
*/

USE [InsureSysManagement_Master];
GO

-- =============================================================
-- Table: Tenants
-- Stores metadata for each tenant (agency) and its dedicated DB
-- =============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tenants')
BEGIN
    CREATE TABLE [dbo].[Tenants]
    (
        [TenantId]                  INT IDENTITY(1,1)   NOT NULL,
        [TenantCode]                NVARCHAR(50)        NOT NULL,
        [AgencyName]                NVARCHAR(200)       NOT NULL,
        [ContactEmail]              NVARCHAR(256)       NULL,
        [ContactPhone]              NVARCHAR(20)        NULL,
        [DatabaseName]              NVARCHAR(128)       NOT NULL,
        [DatabaseServer]            NVARCHAR(256)       NOT NULL,
        [DatabaseUser]              NVARCHAR(128)       NULL,
        [EncryptedDatabasePassword] NVARCHAR(512)       NULL,
        [IsActive]                  BIT                 NOT NULL DEFAULT 1,
        [CreatedAtUtc]              DATETIME2(7)        NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc]              DATETIME2(7)        NULL,
        [CreatedByUserId]           INT                 NULL,

        CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([TenantId])
    );
END
GO

-- =============================================================
-- Table: TenantUsers
-- Login users who authenticate against the Master DB
-- =============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TenantUsers')
BEGIN
    CREATE TABLE [dbo].[TenantUsers]
    (
        [UserId]            INT IDENTITY(1,1)   NOT NULL,
        [TenantId]          INT                 NULL,
        [Username]          NVARCHAR(100)       NOT NULL,
        [PasswordHash]      NVARCHAR(256)       NOT NULL,
        [PasswordSalt]      NVARCHAR(64)        NOT NULL,
        [Email]             NVARCHAR(256)       NULL,
        [DisplayName]       NVARCHAR(200)       NULL,
        [Role]              NVARCHAR(50)        NOT NULL,
        [IsActive]          BIT                 NOT NULL DEFAULT 1,
        [LastLoginAtUtc]    DATETIME2(7)        NULL,
        [CreatedAtUtc]      DATETIME2(7)        NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc]      DATETIME2(7)        NULL,

        CONSTRAINT [PK_TenantUsers] PRIMARY KEY CLUSTERED ([UserId]),
        CONSTRAINT [FK_TenantUsers_Tenants] FOREIGN KEY ([TenantId])
            REFERENCES [dbo].[Tenants]([TenantId])
    );
END
GO

-- FK from Tenants.CreatedByUserId -> TenantUsers.UserId
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Tenants_CreatedByUser'
)
BEGIN
    ALTER TABLE [dbo].[Tenants]
    ADD CONSTRAINT [FK_Tenants_CreatedByUser]
        FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[TenantUsers]([UserId]);
END
GO

-- =============================================================
-- Indexes
-- =============================================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Tenants_TenantCode' AND object_id = OBJECT_ID('dbo.Tenants'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenants_TenantCode]
        ON [dbo].[Tenants]([TenantCode]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Tenants_IsActive' AND object_id = OBJECT_ID('dbo.Tenants'))
    CREATE NONCLUSTERED INDEX [IX_Tenants_IsActive]
        ON [dbo].[Tenants]([IsActive])
        WHERE [IsActive] = 1;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TenantUsers_Username' AND object_id = OBJECT_ID('dbo.TenantUsers'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_TenantUsers_Username]
        ON [dbo].[TenantUsers]([Username]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TenantUsers_TenantId' AND object_id = OBJECT_ID('dbo.TenantUsers'))
    CREATE NONCLUSTERED INDEX [IX_TenantUsers_TenantId]
        ON [dbo].[TenantUsers]([TenantId]);
GO

PRINT 'Master schema created successfully.';
GO
