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
ALTER TABLE dbo.USER_Property
	DROP CONSTRAINT FK_USER_Property_USER_User
GO
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_Property WITH NOCHECK ADD CONSTRAINT
	FK_USER_Property_USER_User FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.USER_User
	(
	UserID
	) ON UPDATE CASCADE
	 ON DELETE CASCADE
	
GO
COMMIT

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
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP CONSTRAINT FK_USER_ContextualPropertySchema_USER_Context
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
	) ON DELETE CASCADE
	
GO
COMMIT

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_Delete]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_Delete
Description:	Deletes a user
Input:		@guidUserID
*/
CREATE PROC dbo.USER_Delete
	@guidUserID 	UNIQUEIDENTIFIER
AS
	SET NOCOUNT ON

	DELETE dbo.USER_User
	WHERE UserID = @guidUserID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetDomains]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetDomains]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_GetDomains
Description:	Gets all available domains
*/
CREATE  PROC dbo.USER_GetDomains
AS
	SET NOCOUNT ON

	SELECT DISTINCT Domain 
	FROM dbo.USER_User
	WHERE Domain <> ''
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_DeleteContext]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_DeleteContext]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_DeleteContext
Description:	Delete a context and all it dependencies permanently
Parameters:	@chvContext
*/
CREATE PROC dbo.USER_DeleteContext
	@chvContext 	VARCHAR(255)
AS
	SET NOCOUNT ON

	DELETE dbo.USER_Context
	WHERE ContextName = @chvContext
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

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
ALTER TABLE dbo.USER_User ADD CONSTRAINT
	DF_USER_User_UserID DEFAULT newid() FOR UserID
GO
COMMIT

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
ALTER TABLE dbo.USER_User ADD CONSTRAINT
	DF_USER_User_Domain DEFAULT '' FOR [Domain]
GO
COMMIT

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetProperties]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetProperties]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_GetProperties
Description:	Gets properties
Parameters:	@guidUserID
		@chvContext
*/
CREATE PROC dbo.USER_GetProperties
	@guidUserID 	UNIQUEIDENTIFIER,
	@chvContext 	VARCHAR(255) 		= NULL
AS
	SET NOCOUNT ON

	SELECT ups.PropertyName, up.PropertyValue, up.ExtendedPropertyValue, udt.TypeName DataType, udt.AssemblyName, udt.AssemblyPath
	FROM dbo.USER_PropertySchema ups
		LEFT JOIN dbo.USER_DataType udt ON ups.DataTypeID = udt.DataTypeID
		LEFT JOIN dbo.USER_Property up ON ups.PropertyID = up.PropertyID
		LEFT JOIN dbo.USER_Context uc ON uc.ContextID = up.ContextID
	WHERE up.UserID = @guidUserID AND 
		((@chvContext IS NULL AND up.ContextID IS NULL) OR uc.ContextName = @chvContext)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_UpdateProperty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_UpdateProperty]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_UpdateProperty
Description:	Updates a property
Input:		@guidUserID
		@chvProperty
		@chvnPropertyValue
		@txtnExtPropertyValue
		@chvContext
*/
CREATE  PROC dbo.USER_UpdateProperty
	@guidUserID		UNIQUEIDENTIFIER,
	@chvProperty 		VARCHAR(255),
	@chvnPropertyValue 	NVARCHAR(4000),
	@txtnExtPropertyValue 	NTEXT,
	@chvContext 		VARCHAR(255) 		= NULL
AS
	SET NOCOUNT ON

	IF EXISTS (
		SELECT p.PropertyID, UserID 
		FROM dbo.USER_Property p 
			JOIN dbo.USER_PropertySchema ps ON p.PropertyID = ps.PropertyID 
			LEFT JOIN dbo.USER_Context c ON p.ContextID = c.ContextID 
		WHERE ps.PropertyName = @chvProperty AND UserID = @guidUserID AND ((@chvContext IS NULL AND c.ContextID IS NULL) OR c.ContextName = @chvContext))
		UPDATE p
		SET 	PropertyValue 		= @chvnPropertyValue, 
			ExtendedPropertyValue 	= @txtnExtPropertyValue
		FROM dbo.USER_Property p 
			JOIN dbo.USER_PropertySchema ps ON p.PropertyID = ps.PropertyID 
			LEFT JOIN dbo.USER_Context c ON p.ContextID = c.ContextID
		WHERE ps.PropertyName = @chvProperty AND UserID = @guidUserID AND ((@chvContext IS NULL AND c.ContextID IS NULL) OR c.ContextName = @chvContext)
	ELSE
		INSERT dbo.USER_Property(PropertyID, UserID, PropertyValue, ExtendedPropertyValue, ContextID)
		SELECT PropertyID, @guidUserID, @chvnPropertyValue, @txtnExtPropertyValue, (SELECT ContextID FROM dbo.USER_Context WHERE ContextName = @chvContext)
		FROM dbo.USER_PropertySchema ps
		WHERE PropertyName = @chvProperty
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

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
ALTER TABLE dbo.USER_Property
	DROP CONSTRAINT PK_USER_Property
GO
ALTER TABLE dbo.USER_Property ADD CONSTRAINT
	PK_USER_Property PRIMARY KEY CLUSTERED 
	(
	UniqueID
	) ON [PRIMARY]

GO
COMMIT

EXECUTE sp_fulltext_table N'dbo.USER_Property', N'drop'

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
DROP INDEX dbo.USER_Property.IX_USER_Property_UniqueID_unique
GO
COMMIT

/*

   den 25 oktober 2006 11:58:51

   User: 

   Server: STO-2KGC00

   Database: nJupiterUnitTests_Internal

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
ALTER TABLE dbo.USER_Property
	DROP CONSTRAINT FK_USER_Property_USER_PropertySchema
GO
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_Property
	DROP CONSTRAINT FK_USER_Property_USER_User
GO
COMMIT
BEGIN TRANSACTION
CREATE TABLE dbo.Tmp_USER_Property
	(
	UniqueID int NOT NULL IDENTITY (1, 1),
	PropertyID int NOT NULL,
	PropertyValue nvarchar(4000) NULL,
	ExtendedPropertyValue ntext NULL,
	UserID uniqueidentifier NOT NULL,
	ContextID int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_USER_Property ON
GO
IF EXISTS(SELECT * FROM dbo.USER_Property)
	 EXEC('INSERT INTO dbo.Tmp_USER_Property (UniqueID, PropertyID, PropertyValue, ExtendedPropertyValue, UserID, ContextID)
		SELECT UniqueID, PropertyID, PropertyValue, ExtendedPropertyValue, UserID, ContextID FROM dbo.USER_Property (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_USER_Property OFF
GO
DROP TABLE dbo.USER_Property
GO
EXECUTE sp_rename N'dbo.Tmp_USER_Property', N'USER_Property', 'OBJECT'
GO
ALTER TABLE dbo.USER_Property ADD CONSTRAINT
	PK_USER_Property PRIMARY KEY CLUSTERED 
	(
	UniqueID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.USER_Property WITH NOCHECK ADD CONSTRAINT
	FK_USER_Property_USER_User FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.USER_User
	(
	UserID
	) ON UPDATE CASCADE
	 ON DELETE CASCADE
	
GO
ALTER TABLE dbo.USER_Property WITH NOCHECK ADD CONSTRAINT
	FK_USER_Property_USER_PropertySchema FOREIGN KEY
	(
	PropertyID
	) REFERENCES dbo.USER_PropertySchema
	(
	PropertyID
	)
GO
COMMIT

UPDATE USER_Property 
SET ContextID = NULL
WHERE ContextID = 0

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
CREATE UNIQUE NONCLUSTERED INDEX IX_USER_Property ON dbo.USER_Property
	(
	PropertyID,
	UserID,
	ContextID
	) ON [PRIMARY]
GO
COMMIT

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
DROP INDEX dbo.USER_Property.IX_USER_Property
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_USER_Property_PropertyID_UserID_ContextID_unique ON dbo.USER_Property
	(
	PropertyID,
	UserID,
	ContextID
	) ON [PRIMARY]
GO
COMMIT

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
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP CONSTRAINT FK_USER_ContextualPropertySchema_USER_Context
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
	) ON UPDATE CASCADE
	 ON DELETE CASCADE
	
GO
COMMIT
BEGIN TRANSACTION
ALTER TABLE dbo.USER_Property ADD CONSTRAINT
	FK_USER_Property_USER_Context FOREIGN KEY
	(
	ContextID
	) REFERENCES dbo.USER_Context
	(
	ContextID
	) ON UPDATE CASCADE
	 ON DELETE CASCADE
	
GO
COMMIT

if (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) <> 1 
exec sp_fulltext_database N'enable' 

GO

if not exists (select * from dbo.sysfulltextcatalogs where name = N'USER_Property')
exec sp_fulltext_catalog N'USER_Property', N'create' 

GO

exec sp_fulltext_table N'[dbo].[USER_Property]', N'create', N'USER_Property', N'PK_USER_Property'
GO

exec sp_fulltext_column N'[dbo].[USER_Property]', N'PropertyValue', N'add', 0  
GO

exec sp_fulltext_column N'[dbo].[USER_Property]', N'ExtendedPropertyValue', N'add', 0  
GO

exec sp_fulltext_table N'[dbo].[USER_Property]', N'activate'  
GO

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
ALTER TABLE dbo.USER_Property ADD
	[Timestamp] timestamp NOT NULL
GO
COMMIT

exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_change_tracking'
GO

exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_background_updateindex'
GO

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
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP CONSTRAINT PK_USER_ContextualPropertySchema
GO
ALTER TABLE dbo.USER_ContextualPropertySchema ADD CONSTRAINT
	PK_USER_ContextualPropertySchema PRIMARY KEY CLUSTERED 
	(
	ContextID,
	PropertyID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.USER_ContextualPropertySchema
	DROP COLUMN ContextSchemaID
GO
COMMIT

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
DROP INDEX dbo.USER_ContextualPropertySchema.IX_USER_ContextualPropertySchema_ContextID
GO
COMMIT