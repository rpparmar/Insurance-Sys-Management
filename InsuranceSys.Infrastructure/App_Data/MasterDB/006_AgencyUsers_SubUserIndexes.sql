/*
    Master DB incremental script (idempotent).
    Indexes for agency-scoped sub user listing and lookups.
*/

SET NOCOUNT ON;
GO

--IF NOT EXISTS (
--    SELECT 1
--    FROM sys.indexes
--    WHERE name = 'IX_AgencyUsers_AgencyId_IsDeleted_IsSubUser'
--      AND object_id = OBJECT_ID('dbo.AgencyUsers')
--)
--BEGIN
--    CREATE NONCLUSTERED INDEX [IX_AgencyUsers_AgencyId_IsDeleted_IsSubUser]
--        ON [dbo].[AgencyUsers] ([AgencyId], [IsDeleted], [IsSubUser])
--        INCLUDE ([IsActive], [Role], [Username], [FirstName], [MiddleName], [LastName], [Email]);
--END
--GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_AgencyUsers_AgencyId_IsSubUser_IsDeleted_Username'
      AND object_id = OBJECT_ID('dbo.AgencyUsers')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AgencyUsers_AgencyId_IsSubUser_IsDeleted_Username]
        ON [dbo].[AgencyUsers] ([AgencyId], [IsSubUser], [IsDeleted], [Username]);
END
GO

