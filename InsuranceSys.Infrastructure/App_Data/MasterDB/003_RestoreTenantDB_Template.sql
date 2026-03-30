/*
    Template: Restore a new tenant database from the default .bak backup.
    
    This script is executed programmatically by TenantProvisioningService.
    Parameters are injected via ADO.NET SqlCommand:
      @NewDbName     - The target database name (e.g., InsureSys_Agency_ACME)
      @BakFilePath   - Full path to InsureSysManagement_Default.bak
      @DataDirectory  - Target directory for .mdf file
      @LogDirectory   - Target directory for .ldf file
    
    The script dynamically reads logical file names from the backup
    using RESTORE FILELISTONLY, then performs the restore with MOVE.
*/

-- Step 1: Read logical file names from the backup
DECLARE @LogicalDataName NVARCHAR(256);
DECLARE @LogicalLogName  NVARCHAR(256);
DECLARE @FileList TABLE (
    LogicalName          NVARCHAR(128),
    PhysicalName         NVARCHAR(260),
    [Type]               CHAR(1),
    FileGroupName        NVARCHAR(128) NULL,
    Size                 NUMERIC(20,0),
    MaxSize              NUMERIC(20,0),
    FileID               BIGINT,
    CreateLSN            NUMERIC(25,0),
    DropLSN              NUMERIC(25,0) NULL,
    UniqueID             UNIQUEIDENTIFIER,
    ReadOnlyLSN          NUMERIC(25,0) NULL,
    ReadWriteLSN         NUMERIC(25,0) NULL,
    BackupSizeInBytes    BIGINT,
    SourceBlockSize      INT,
    FileGroupID          INT,
    LogGroupGUID         UNIQUEIDENTIFIER NULL,
    DifferentialBaseLSN  NUMERIC(25,0) NULL,
    DifferentialBaseGUID UNIQUEIDENTIFIER NULL,
    IsReadOnly           BIT,
    IsPresent            BIT,
    TDEThumbprint        VARBINARY(32) NULL,
    SnapshotURL          NVARCHAR(360) NULL
);

INSERT INTO @FileList
EXEC('RESTORE FILELISTONLY FROM DISK = ''' + @BakFilePath + '''');

SELECT @LogicalDataName = LogicalName FROM @FileList WHERE [Type] = 'D';
SELECT @LogicalLogName  = LogicalName FROM @FileList WHERE [Type] = 'L';

-- Step 2: Build file paths
DECLARE @MdfPath NVARCHAR(512) = @DataDirectory + @NewDbName + N'.mdf';
DECLARE @LdfPath NVARCHAR(512) = @LogDirectory  + @NewDbName + N'_log.ldf';

-- Step 3: Restore the database
DECLARE @RestoreSQL NVARCHAR(MAX) = N'
RESTORE DATABASE ' + QUOTENAME(@NewDbName) + N'
FROM DISK = ' + QUOTENAME(@BakFilePath, '''') + N'
WITH 
    MOVE ' + QUOTENAME(@LogicalDataName, '''') + N' TO ' + QUOTENAME(@MdfPath, '''') + N',
    MOVE ' + QUOTENAME(@LogicalLogName, '''')  + N' TO ' + QUOTENAME(@LdfPath, '''') + N',
    REPLACE,
    STATS = 10;';

EXEC sp_executesql @RestoreSQL;

PRINT 'Database [' + @NewDbName + '] restored successfully.';
GO
