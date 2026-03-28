IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CountryMaster')
BEGIN
    CREATE TABLE [dbo].[CountryMaster]
    (
        [CountryID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CountryName] NVARCHAR(100) NOT NULL,
        [CountryCode] NVARCHAR(5) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_CountryMaster_IsActive] DEFAULT(1),
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_CountryMaster_IsDeleted] DEFAULT(0),
        [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_CountryMaster_CreatedOn] DEFAULT(SYSUTCDATETIME()),
        [UpdatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_CountryMaster_UpdatedOn] DEFAULT(SYSUTCDATETIME())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StateMaster')
BEGIN
    CREATE TABLE [dbo].[StateMaster]
    (
        [StateID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CountryID] INT NOT NULL,
        [StateName] NVARCHAR(100) NOT NULL,
        [StateCode] NVARCHAR(5) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_StateMaster_IsActive] DEFAULT(1),
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_StateMaster_IsDeleted] DEFAULT(0),
        [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_StateMaster_CreatedOn] DEFAULT(SYSUTCDATETIME()),
        [UpdatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_StateMaster_UpdatedOn] DEFAULT(SYSUTCDATETIME())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_StateMaster_CountryMaster')
BEGIN
    ALTER TABLE [dbo].[StateMaster]
    ADD CONSTRAINT [FK_StateMaster_CountryMaster]
    FOREIGN KEY ([CountryID]) REFERENCES [dbo].[CountryMaster]([CountryID]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_CountryMaster_CountryName_NotDeleted')
BEGIN
    CREATE UNIQUE INDEX [UX_CountryMaster_CountryName_NotDeleted]
    ON [dbo].[CountryMaster]([CountryName])
    WHERE [IsDeleted] = 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_CountryMaster_CountryCode_NotDeleted')
BEGIN
    CREATE UNIQUE INDEX [UX_CountryMaster_CountryCode_NotDeleted]
    ON [dbo].[CountryMaster]([CountryCode])
    WHERE [IsDeleted] = 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_StateMaster_Country_StateName_NotDeleted')
BEGIN
    CREATE UNIQUE INDEX [UX_StateMaster_Country_StateName_NotDeleted]
    ON [dbo].[StateMaster]([CountryID], [StateName])
    WHERE [IsDeleted] = 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_StateMaster_Country_StateCode_NotDeleted')
BEGIN
    CREATE UNIQUE INDEX [UX_StateMaster_Country_StateCode_NotDeleted]
    ON [dbo].[StateMaster]([CountryID], [StateCode])
    WHERE [IsDeleted] = 0;
END
GO
