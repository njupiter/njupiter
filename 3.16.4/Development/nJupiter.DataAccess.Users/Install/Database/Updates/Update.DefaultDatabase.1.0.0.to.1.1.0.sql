/*
Name:       	USER_GetPropertySchema
Description:	gets properties for a certain context
Parameters:	chvContext
Output:		PropertyName
		DataType
		AssemblyName
		AssemblyPath
*/
ALTER PROCEDURE dbo.USER_GetPropertySchema
	@chvContext VARCHAR(255) = NULL
AS		
	IF(@chvContext IS NOT NULL)
		SELECT UPS.PropertyName, UDT.TypeName AS DataType, UDT.AssemblyName, UDT.AssemblyPath
		FROM  	USER_PropertySchema UPS INNER JOIN 
			USER_ContextualPropertySchema UCPS ON UCPS.PropertyID = UPS.PropertyID INNER JOIN 
			USER_Context UC ON UC.ContextID = UCPS.ContextID INNER JOIN 
			USER_DataType UDT ON UPS.DataTypeID = UDT.DataTypeID
		WHERE UC.ContextName = @chvContext
		ORDER BY SortOrder
	ELSE
		SELECT UPS.PropertyName AS PropertyName, UDT.TypeName AS DataType, UDT.AssemblyName as AssemblyName, UDT.AssemblyPath as AssemblyPath
		FROM 	USER_PropertySchema UPS INNER JOIN
			USER_DataType UDT ON UPS.DataTypeID = UDT.DataTypeID

GO

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP COLUMN Domain
GO
COMMIT

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_PropertySchema ADD CONSTRAINT
	IX_USER_PropertySchema_PropertyName_unique UNIQUE NONCLUSTERED 
	(
	PropertyName
	) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
CREATE TABLE dbo.Tmp_USER_Context
	(
	ContextID int NOT NULL IDENTITY (1, 1),
	ContextName varchar(4000) NOT NULL
	)  ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_USER_Context ON
GO
IF EXISTS(SELECT * FROM dbo.USER_Context)
	 EXEC('INSERT INTO dbo.Tmp_USER_Context (ContextID, ContextName)
		SELECT ContextID, ContextName FROM dbo.USER_Context TABLOCKX')
GO
SET IDENTITY_INSERT dbo.Tmp_USER_Context OFF
GO
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP CONSTRAINT FK_USER_ContextualPropertySchema_USER_Context
GO
DROP TABLE dbo.USER_Context
GO
EXECUTE sp_rename N'dbo.Tmp_USER_Context', N'USER_Context', 'OBJECT'
GO
ALTER TABLE dbo.USER_Context ADD CONSTRAINT
	PK_USER_Context PRIMARY KEY CLUSTERED 
	(
	ContextID
	) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_ContextualPropertySchema WITH NOCHECK ADD CONSTRAINT
	FK_USER_ContextualPropertySchema_USER_Context FOREIGN KEY
	(
	ContextID
	) REFERENCES dbo.USER_Context
	(
	ContextID
	)
GO
COMMIT

GO
/*
Name:       	USER_CreateContext
Description:	creates a new context
Parameters:	chvContext

*/
CREATE PROCEDURE dbo.USER_CreateContext
	@chvContext VARCHAR(255)
AS
	INSERT INTO USER_Context(ContextName) VALUES(@chvContext)
GO


/*
Name:       	USER_AddContextualPropertySchema
Description:	Add a property schema to a context
Parameters:	chvContext

*/
CREATE PROCEDURE dbo.USER_AddContextualPropertySchema
	@chvContext VARCHAR(255),
	@chvPropertyName VARCHAR(255)
AS

INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) 
SELECT c.ContextID, ps.PropertyID
FROM USER_Context c, USER_PropertySchema ps
WHERE c.ContextName = @chvContext AND ps.PropertyName = @chvPropertyName

GO


BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP COLUMN SortOrder
GO
COMMIT

GO

/*
Name:       	USER_GetPropertySchema
Description:	gets properties for a certain context
Parameters:	chvDomain
		chvContext
Output:		PropertyName
		DataType
		AssemblyName
		AssemblyPath
*/
ALTER PROCEDURE dbo.USER_GetPropertySchema
	@chvContext VARCHAR(255) = NULL
AS		
	IF(@chvContext IS NOT NULL)
		SELECT UPS.PropertyName, UDT.TypeName AS DataType, UDT.AssemblyName, UDT.AssemblyPath
		FROM  	USER_PropertySchema UPS INNER JOIN 
			USER_ContextualPropertySchema UCPS ON UCPS.PropertyID = UPS.PropertyID INNER JOIN 
			USER_Context UC ON UC.ContextID = UCPS.ContextID INNER JOIN 
			USER_DataType UDT ON UPS.DataTypeID = UDT.DataTypeID
		WHERE UC.ContextName = @chvContext
	ELSE
		SELECT UPS.PropertyName AS PropertyName, UDT.TypeName AS DataType, UDT.AssemblyName as AssemblyName, UDT.AssemblyPath as AssemblyPath
		FROM 	USER_PropertySchema UPS INNER JOIN
			USER_DataType UDT ON UPS.DataTypeID = UDT.DataTypeID


GO

/*
Name:       	USER_DeleteContext
Description:	Delete a context an all it dependencies permanently
Parameters:	chvContext

*/
CREATE PROCEDURE dbo.USER_DeleteContext
	@chvContext VARCHAR(255)
AS

DELETE USER_Property
FROM USER_Property INNER JOIN USER_Context 
   ON USER_Property.ContextID = USER_Context.ContextID
WHERE USER_Context.ContextName = @chvContext

DELETE USER_ContextualPropertySchema
FROM USER_ContextualPropertySchema INNER JOIN USER_Context 
   ON USER_ContextualPropertySchema.ContextID = USER_Context.ContextID
WHERE USER_Context.ContextName = @chvContext

DELETE FROM USER_Context WHERE ContextName = @chvContext


GO

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
CREATE TABLE dbo.Tmp_USER_Context
	(
	ContextID int NOT NULL IDENTITY (1, 1),
	ContextName varchar(255) NOT NULL
	)  ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_USER_Context ON
GO
IF EXISTS(SELECT * FROM dbo.USER_Context)
	 EXEC('INSERT INTO dbo.Tmp_USER_Context (ContextID, ContextName)
		SELECT ContextID, CONVERT(varchar(255), ContextName) FROM dbo.USER_Context TABLOCKX')
GO
SET IDENTITY_INSERT dbo.Tmp_USER_Context OFF
GO
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP CONSTRAINT FK_USER_ContextualPropertySchema_USER_Context
GO
DROP TABLE dbo.USER_Context
GO
EXECUTE sp_rename N'dbo.Tmp_USER_Context', N'USER_Context', 'OBJECT'
GO
ALTER TABLE dbo.USER_Context ADD CONSTRAINT
	PK_USER_Context PRIMARY KEY CLUSTERED 
	(
	ContextID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.USER_Context ADD CONSTRAINT
	IX_USER_Context_ContextName_unique UNIQUE NONCLUSTERED 
	(
	ContextID
	) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_ContextualPropertySchema WITH NOCHECK ADD CONSTRAINT
	FK_USER_ContextualPropertySchema_USER_Context FOREIGN KEY
	(
	ContextID
	) REFERENCES dbo.USER_Context
	(
	ContextID
	)
GO
COMMIT

GO

/*

   den 12 april 2006 20:45:27

   User: 

   Server: STO-2KGC00

   Database: SthlmStad_Internal

   Application: MS SQLEM - Data Tools

*/



BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
CREATE UNIQUE NONCLUSTERED INDEX IX_USER_User ON dbo.USER_User
	(
	Username,
	[Domain]
	) ON [PRIMARY]
GO
COMMIT
GO