/*
    Master database only (same database as AgencyDetails / MasterDbContext).
    Lists agencies for SuperAdmin grid with paging, search, and sort.
*/

SET NOCOUNT ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[AgencyDetails_GetAll]
    @PageNumber INT,
    @PageSize INT,
    @SearchTerm NVARCHAR(100) = N'',
    @SortExp NVARCHAR(200) = N'AgencyName ASC'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sql NVARCHAR(MAX);
    DECLARE @params NVARCHAR(500) = N'@PageNumber INT, @PageSize INT, @SearchTerm NVARCHAR(100)';

    SELECT COUNT(*) AS TotalRecords
    FROM [dbo].[AgencyDetails] AS a
    WHERE ISNULL(a.[IsDeleted], 0) <> 1
      AND (
            NULLIF(LTRIM(RTRIM(@SearchTerm)), N'') IS NULL
            OR a.[AgencyName] LIKE N'%' + @SearchTerm + N'%'
            OR (a.[ContactEmail] IS NOT NULL AND a.[ContactEmail] LIKE N'%' + @SearchTerm + N'%')
            OR (a.[ContactPhone] IS NOT NULL AND a.[ContactPhone] LIKE N'%' + @SearchTerm + N'%')
          );

    -- Whitelist sort (first token = column as returned to client; map to SQL expressions)
    DECLARE @sortSafe NVARCHAR(200) = N'AgencyName ASC';
    DECLARE @firstTok NVARCHAR(50) =
        LOWER(LTRIM(RTRIM(LEFT(@SortExp, CHARINDEX(' ', @SortExp + N' ') - 1))));
    DECLARE @dir NVARCHAR(4) = CASE WHEN LOWER(RIGHT(LTRIM(RTRIM(@SortExp)), 4)) = N'desc' THEN N'DESC' ELSE N'ASC' END;

    IF @firstTok = N'agencyname'
        SET @sortSafe = N'a.[AgencyName] ' + @dir;
    ELSE IF @firstTok = N'email'
        SET @sortSafe = N'a.[ContactEmail] ' + @dir;
    ELSE IF @firstTok = N'contactno'
        SET @sortSafe = N'a.[ContactPhone] ' + @dir;
    ELSE IF @firstTok = N'agencyid'
        SET @sortSafe = N'a.[AgencyId] ' + @dir;
    ELSE
        SET @sortSafe = N'a.[AgencyName] ASC';

    SET @sql = N'
    SELECT
        a.[AgencyId],
        a.[AgencyName],
        a.[ContactEmail] AS [Email],
        a.[ContactPhone] AS [ContactNo],
        a.[IsActive]
    FROM [dbo].[AgencyDetails] AS a
    WHERE ISNULL(a.[IsDeleted], 0) <> 1
      AND (
            NULLIF(LTRIM(RTRIM(@SearchTerm)), N'''') IS NULL
            OR a.[AgencyName] LIKE N''%'' + @SearchTerm + N''%''
            OR (a.[ContactEmail] IS NOT NULL AND a.[ContactEmail] LIKE N''%'' + @SearchTerm + N''%'')
            OR (a.[ContactPhone] IS NOT NULL AND a.[ContactPhone] LIKE N''%'' + @SearchTerm + N''%'')
          )
    ORDER BY ' + @sortSafe + N'
    OFFSET @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY;';

    EXEC sp_executesql @sql, @params, @PageNumber = @PageNumber, @PageSize = @PageSize, @SearchTerm = @SearchTerm;
END
GO
