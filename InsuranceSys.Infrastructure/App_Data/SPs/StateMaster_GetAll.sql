if object_ID('[dbo].[StateMaster_GetAll]') is null
begin
exec('
CREATE PROCEDURE [dbo].[StateMaster_GetAll]
	@PageNumber INT,
	@PageSize INT,
	@SearchTerm varchar(100)='''',
	@SortExp varchar(50)=''1 ASC''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @sql NVARCHAR(MAX);
	DECLARE @params NVARCHAR(MAX)=N''@PageNumber INT,@PageSize INT,@SearchTerm VARCHAR(100),@SortExp VARCHAR(50)'';

	SELECT COUNT(*) AS TotalRecords
	FROM StateMaster AS S
	INNER JOIN CountryMaster AS C ON C.CountryID = S.CountryID
	WHERE (S.IsDeleted = 0 OR S.IsDeleted IS NULL)
		AND (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
		AND
		(
			''''+CONVERT(VARCHAR(100), @SearchTerm)+'''' IS NULL
			OR S.StateName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR S.StateCode LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR C.CountryName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
		)

	SET @sql=N''
		SELECT
			S.[StateID]
			,S.[CountryID]
			,S.[StateName]
			,S.[StateCode]
			,C.[CountryName]
			,S.[IsActive]
			,S.[CreatedOn]
			,S.[UpdatedOn]
		FROM StateMaster AS S
		INNER JOIN CountryMaster AS C ON C.CountryID = S.CountryID
		WHERE (S.IsDeleted = 0 OR S.IsDeleted IS NULL)
			AND (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
			AND
			(
				''''''+CONVERT(VARCHAR(100), @SearchTerm)+'''''' IS NULL
				OR S.StateName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR S.StateCode LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR C.CountryName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
			)
		ORDER BY '' + CONVERT(VARCHAR(50), @SortExp) + ''
		OFFSET '' + CONVERT(VARCHAR(10), @PageNumber) + '' ROWS FETCH NEXT '' + CONVERT(VARCHAR(10), @PageSize) + '' ROWS ONLY;
	''

	EXEC sp_executesql
		@sql
		,@params
		,@PageNumber=@PageNumber
		,@PageSize=@PageSize
		,@SearchTerm=@SearchTerm
		,@SortExp=@SortExp

END
')
END
Else
begin
exec('
ALTER PROCEDURE [dbo].[StateMaster_GetAll]
	@PageNumber INT,
	@PageSize INT,
	@SearchTerm varchar(100)='''',
	@SortExp varchar(50)=''1 ASC''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @sql NVARCHAR(MAX);
	DECLARE @params NVARCHAR(MAX)=N''@PageNumber INT,@PageSize INT,@SearchTerm VARCHAR(100),@SortExp VARCHAR(50)'';

	SELECT COUNT(*) AS TotalRecords
	FROM StateMaster AS S
	INNER JOIN CountryMaster AS C ON C.CountryID = S.CountryID
	WHERE (S.IsDeleted = 0 OR S.IsDeleted IS NULL)
		AND (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
		AND
		(
			''''+CONVERT(VARCHAR(100), @SearchTerm)+'''' IS NULL
			OR S.StateName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR S.StateCode LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR C.CountryName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
		)

	SET @sql=N''
		SELECT
			S.[StateID]
			,S.[CountryID]
			,S.[StateName]
			,S.[StateCode]
			,C.[CountryName]
			,S.[IsActive]
			,S.[CreatedOn]
			,S.[UpdatedOn]
		FROM StateMaster AS S
		INNER JOIN CountryMaster AS C ON C.CountryID = S.CountryID
		WHERE (S.IsDeleted = 0 OR S.IsDeleted IS NULL)
			AND (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
			AND
			(
				''''''+CONVERT(VARCHAR(100), @SearchTerm)+'''''' IS NULL
				OR S.StateName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR S.StateCode LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR C.CountryName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
			)
		ORDER BY '' + CONVERT(VARCHAR(50), @SortExp) + ''
		OFFSET '' + CONVERT(VARCHAR(10), @PageNumber) + '' ROWS FETCH NEXT '' + CONVERT(VARCHAR(10), @PageSize) + '' ROWS ONLY;
	''

	EXEC sp_executesql
		@sql
		,@params
		,@PageNumber=@PageNumber
		,@PageSize=@PageSize
		,@SearchTerm=@SearchTerm
		,@SortExp=@SortExp

END
')
end
