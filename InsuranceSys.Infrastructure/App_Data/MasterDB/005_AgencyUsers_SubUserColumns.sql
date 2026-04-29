/*
    Master DB incremental script (idempotent).
    Adds profile fields + IsSubUser marker to dbo.AgencyUsers.
*/

SET NOCOUNT ON;
GO

IF COL_LENGTH('dbo.AgencyUsers', 'FirstName') IS NULL
BEGIN
    ALTER TABLE dbo.AgencyUsers ADD FirstName NVARCHAR(25) NULL;
END
GO

IF COL_LENGTH('dbo.AgencyUsers', 'MiddleName') IS NULL
BEGIN
    ALTER TABLE dbo.AgencyUsers ADD MiddleName NVARCHAR(25) NULL;
END
GO

IF COL_LENGTH('dbo.AgencyUsers', 'LastName') IS NULL
BEGIN
    ALTER TABLE dbo.AgencyUsers ADD LastName NVARCHAR(25) NULL;
END
GO

IF COL_LENGTH('dbo.AgencyUsers', 'IsSubUser') IS NULL
BEGIN
    ALTER TABLE dbo.AgencyUsers
        ADD IsSubUser BIT NOT NULL
            CONSTRAINT DF_AgencyUsers_IsSubUser DEFAULT(0);
END
GO

