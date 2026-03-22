if object_ID('[dbo].[Customer_GetAll]') is null
begin
exec('
CREATE PROCEDURE [dbo].[Customer_GetAll]
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
	FROM CustomersInfo AS Cust
	WHERE (Cust.IsDeleted = 0 OR Cust.IsDeleted IS NULL)
		AND
		(
			''''+CONVERT(VARCHAR(100), @SearchTerm)+'''' IS NULL
			OR FirstName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR LastName  LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR Phone     LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR Email     LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
		)

	SET @sql=N''
		SELECT
			Cust.[CustomerID]
			,Cust.[EncryptedCustomerId]
			,Cust.[FirstName]
			,Cust.[LastName]
			,Cust.[Phone]
			,Cust.[Email]
			,Cust.[DOB]
			,COUNT(Plc.PolicyID) AS PolicyCount
			
		FROM CustomersInfo AS Cust
		LEFT JOIN PolicyDetails Plc
		ON Plc.CustomerID = Cust.CustomerID  AND (Plc.IsDeleted = 0 OR Plc.IsDeleted IS NULL)
		WHERE (Cust.IsDeleted = 0 OR Cust.IsDeleted IS NULL)
			AND
			(
				''''''+CONVERT(VARCHAR(100), @SearchTerm)+'''''' IS NULL
				OR FirstName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR LastName  LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR Phone     LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR Email     LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
			)
		GROUP BY
			Cust.CustomerID,
			Cust.EncryptedCustomerId,
			Cust.FirstName,
			Cust.LastName,
			Cust.Phone,
			Cust.Email,
			Cust.DOB
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
ALTER PROCEDURE [dbo].[Customer_GetAll]
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
	FROM CustomersInfo AS Cust
	WHERE (Cust.IsDeleted = 0 OR Cust.IsDeleted IS NULL)
		AND
		(
			''''+CONVERT(VARCHAR(100), @SearchTerm)+'''' IS NULL
			OR FirstName LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR LastName  LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR Phone     LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
			OR Email     LIKE ''%''+CONVERT(VARCHAR(100), @SearchTerm)+''%''
		)

	SET @sql=N''
		SELECT
			Cust.[CustomerID]
			,Cust.[EncryptedCustomerId]
			,Cust.[FirstName]
			,Cust.[LastName]
			,Cust.[Phone]
			,Cust.[Email]
			,Cust.[DOB]
			,COUNT(Plc.PolicyID) AS PolicyCount
			
		FROM CustomersInfo AS Cust
		LEFT JOIN PolicyDetails Plc
		ON Plc.CustomerID = Cust.CustomerID  AND (Plc.IsDeleted = 0 OR Plc.IsDeleted IS NULL)
		WHERE (Cust.IsDeleted = 0 OR Cust.IsDeleted IS NULL)
			AND
			(
				''''''+CONVERT(VARCHAR(100), @SearchTerm)+'''''' IS NULL
				OR FirstName LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR LastName  LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR Phone     LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
				OR Email     LIKE ''''%''''+CONVERT(VARCHAR(100), @SearchTerm)+''''%''''
			)
		GROUP BY
			Cust.CustomerID,
			Cust.EncryptedCustomerId,
			Cust.FirstName,
			Cust.LastName,
			Cust.Phone,
			Cust.Email,
			Cust.DOB
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
