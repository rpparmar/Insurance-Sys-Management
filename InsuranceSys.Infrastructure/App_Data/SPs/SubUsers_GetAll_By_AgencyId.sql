/*
    Master database stored procedure.
    DataTables contract:
      - Result set 1: single row with TotalRecords
      - Result set 2: paged rows for the grid
*/

SET NOCOUNT ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[SubUsers_GetAll_By_AgencyId]
    @PageNumber INT,
    @PageSize INT,
    @SearchTerm NVARCHAR(100) = N'',
    @SortExp NVARCHAR(200) = N'UserName ASC',
    @AgencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS TotalRecords
    FROM [dbo].[AgencyUsers] AS u
    WHERE u.[AgencyId] = @AgencyId
      AND ISNULL(u.[IsDeleted], 0) <> 1
      AND ISNULL(u.[IsSubUser], 0) = 1
      AND (
            NULLIF(LTRIM(RTRIM(@SearchTerm)), N'') IS NULL
            OR u.[Username] LIKE N'%' + @SearchTerm + N'%'
            OR (u.[FirstName] IS NOT NULL AND u.[FirstName] LIKE N'%' + @SearchTerm + N'%')
            OR (u.[MiddleName] IS NOT NULL AND u.[MiddleName] LIKE N'%' + @SearchTerm + N'%')
            OR (u.[LastName] IS NOT NULL AND u.[LastName] LIKE N'%' + @SearchTerm + N'%')
            OR (u.[Email] IS NOT NULL AND u.[Email] LIKE N'%' + @SearchTerm + N'%')
          );

    -- Whitelist sort (first token = client column; map to SQL expressions)
    DECLARE @sortSafe NVARCHAR(200) = N'u.[Username] ASC';
    DECLARE @firstTok NVARCHAR(50) =
        LOWER(LTRIM(RTRIM(LEFT(@SortExp, CHARINDEX(' ', @SortExp + N' ') - 1))));
    DECLARE @dir NVARCHAR(4) = CASE WHEN LOWER(RIGHT(LTRIM(RTRIM(@SortExp)), 4)) = N'desc' THEN N'DESC' ELSE N'ASC' END;

    IF @firstTok IN (N'username', N'user')
        SET @sortSafe = N'u.[Username] ' + @dir;
    ELSE IF @firstTok = N'firstname'
        SET @sortSafe = N'u.[FirstName] ' + @dir;
    ELSE IF @firstTok = N'middlename'
        SET @sortSafe = N'u.[MiddleName] ' + @dir;
    ELSE IF @firstTok = N'lastname'
        SET @sortSafe = N'u.[LastName] ' + @dir;
    ELSE IF @firstTok = N'email'
        SET @sortSafe = N'u.[Email] ' + @dir;
    ELSE IF @firstTok IN (N'isactive', N'status')
        SET @sortSafe = N'u.[IsActive] ' + @dir;
    ELSE
        SET @sortSafe = N'u.[Username] ASC';

    DECLARE @sql NVARCHAR(MAX) = N'
    SELECT
        u.[UserId],
        u.[Username] AS [UserName],
        u.[FirstName],
        u.[MiddleName],
        u.[LastName],
        u.[Email],
        u.[IsActive]
    FROM [dbo].[AgencyUsers] AS u
    WHERE u.[AgencyId] = @AgencyId
      AND ISNULL(u.[IsDeleted], 0) <> 1
      AND ISNULL(u.[IsSubUser], 0) = 1
      AND (
            NULLIF(LTRIM(RTRIM(@SearchTerm)), N'''') IS NULL
            OR u.[Username] LIKE N''%'' + @SearchTerm + N''%''
            OR (u.[FirstName] IS NOT NULL AND u.[FirstName] LIKE N''%'' + @SearchTerm + N''%'')
            OR (u.[MiddleName] IS NOT NULL AND u.[MiddleName] LIKE N''%'' + @SearchTerm + N''%'')
            OR (u.[LastName] IS NOT NULL AND u.[LastName] LIKE N''%'' + @SearchTerm + N''%'')
            OR (u.[Email] IS NOT NULL AND u.[Email] LIKE N''%'' + @SearchTerm + N''%'')
          )
    ORDER BY ' + @sortSafe + N'
    OFFSET @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY;';

    EXEC sp_executesql
        @sql,
        N'@PageNumber INT, @PageSize INT, @SearchTerm NVARCHAR(100), @AgencyId INT',
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @SearchTerm = @SearchTerm,
        @AgencyId = @AgencyId;
END
GO

