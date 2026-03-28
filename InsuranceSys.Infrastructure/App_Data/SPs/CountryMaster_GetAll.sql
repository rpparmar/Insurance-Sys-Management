if object_ID('[dbo].[CountryMaster_GetAll]') is null
begin
exec('
CREATE PROCEDURE [dbo].[CountryMaster_GetAll]
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
	FROM CountryMaster AS C
	WHERE (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
		AND
		(
			''''+CONVERT(VARCHAR(100), @SearchTerm)+'''' IS NULL
			OR C.CountryName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR C.CountryCode LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
		)

	SET @sql=N''
		SELECT
			C.[CountryID]
			,C.[CountryName]
			,C.[CountryCode]
			,C.[IsActive]
			,C.[CreatedOn]
			,C.[UpdatedOn]
		FROM CountryMaster AS C
		WHERE (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
			AND
			(
				''''''+CONVERT(VARCHAR(100), @SearchTerm)+'''''' IS NULL
				OR C.CountryName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR C.CountryCode LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
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
ALTER PROCEDURE [dbo].[CountryMaster_GetAll]
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
	FROM CountryMaster AS C
	WHERE (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
		AND
		(
			''''+CONVERT(VARCHAR(100), @SearchTerm)+'''' IS NULL
			OR C.CountryName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR C.CountryCode LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
		)

	SET @sql=N''
		SELECT
			C.[CountryID]
			,C.[CountryName]
			,C.[CountryCode]
			,C.[IsActive]
			,C.[CreatedOn]
			,C.[UpdatedOn]
		FROM CountryMaster AS C
		WHERE (C.IsDeleted = 0 OR C.IsDeleted IS NULL)
			AND
			(
				''''''+CONVERT(VARCHAR(100), @SearchTerm)+'''''' IS NULL
				OR C.CountryName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR C.CountryCode LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
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
