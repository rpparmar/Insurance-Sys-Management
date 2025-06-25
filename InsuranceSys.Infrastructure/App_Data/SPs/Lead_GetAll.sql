if object_ID('[dbo].[Lead_GetAll]') is  null
begin
exec('
CREATE PROCEDURE [dbo].[Lead_GetAll]  
 @PageNumber INT,
	@PageSize INT,
	@SearchTerm varchar(100)='''',
	@SortExp varchar(50)=''1 ASC''
AS  
BEGIN  
 SET NOCOUNT ON;  
   
 DECLARE @sql NVARCHAR(MAX);  
 DECLARE @params NVARCHAR(MAX)=N''@PageNumber INT,@PageSize INT,@SearchTerm VARCHAR(50),@SortExp VARCHAR(50)'';  
  
SELECT COUNT(*) AS TotalRecords FROM LeadManagement WHERE ISNULL(IsDeleted,0)<>1 
	AND 
	(
		''''+CONVERT(VARCHAR(50), @SearchTerm)+'''' IS NULL OR FirstName LIKE ''%''+CONVERT(VARCHAR(50), @SearchTerm)+''%''
	)

 SET @sql=N''  
  SELECT 
		[LeadID]
		,[FirstName]
		,[LastName]
		,[Email]
		,[PhoneNumber]
		,[LeadStatus]
		,[LeadSource]
		,[AssignedTo]
		,[InquiryDate]
		,[NextFollowUpDate]
		,[Notes]
		,[Address]
		,[CompanyID]
		,[PolicyTypeID]
		FROM LeadManagement
		WHERE 
			ISNULL(IsDeleted,0)<>1 
			AND 
			(
				''''''+CONVERT(VARCHAR(50), @SearchTerm)+'''''' IS NULL OR FirstName LIKE ''''%''''+CONVERT(VARCHAR(50), @SearchTerm)+''''%''''
			)
			ORDER BY '' + CONVERT(VARCHAR(50), @sortexp) + ''  
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
ALTER  PROCEDURE [dbo].[Lead_GetAll]  
 @PageNumber INT,
	@PageSize INT,
	@SearchTerm varchar(100)='''',
	@SortExp varchar(50)=''1 ASC''
AS  
BEGIN  
 SET NOCOUNT ON;  
   
 DECLARE @sql NVARCHAR(MAX);  
 DECLARE @params NVARCHAR(MAX)=N''@PageNumber INT,@PageSize INT,@SearchTerm VARCHAR(50),@SortExp VARCHAR(50)'';  
  
SELECT COUNT(*) AS TotalRecords FROM LeadManagement WHERE ISNULL(IsDeleted,0)<>1 
	AND 
	(
		''''+CONVERT(VARCHAR(50), @SearchTerm)+'''' IS NULL OR FirstName LIKE ''%''+CONVERT(VARCHAR(50), @SearchTerm)+''%''
	)

 SET @sql=N''  
  SELECT 
		[LeadID]
		,[FirstName]
		,[LastName]
		,[Email]
		,[PhoneNumber]
		,[LeadStatus]
		,[LeadSource]
		,[AssignedTo]
		,[InquiryDate]
		,[NextFollowUpDate]
		,[Notes]
		,[Address]
		,[CompanyID]
		,[PolicyTypeID]
		FROM LeadManagement
		WHERE 
			ISNULL(IsDeleted,0)<>1 
			AND 
			(
				''''''+CONVERT(VARCHAR(50), @SearchTerm)+'''''' IS NULL OR FirstName LIKE ''''%''''+CONVERT(VARCHAR(50), @SearchTerm)+''''%''''
			)
			ORDER BY '' + CONVERT(VARCHAR(50), @sortexp) + ''  
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