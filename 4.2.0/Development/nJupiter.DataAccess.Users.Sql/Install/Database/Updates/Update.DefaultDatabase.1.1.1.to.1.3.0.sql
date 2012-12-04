if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_AddContextualPropertySchema]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_AddContextualPropertySchema]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_CreateContext]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_CreateContext]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_DeleteContext]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_DeleteContext]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_DeleteProperty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_DeleteProperty]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetByID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetByUsername]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetByUsername]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetContexts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetContexts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetProperties]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetProperties]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetPropertySchema]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetPropertySchema]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetUsersByDomain]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetUsersByDomain]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_UpdateProperty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_UpdateProperty]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_AddContextualPropertySchema
Description:	Add a property schema to a context
Parameters:	@chvContext
		@chvPropertyName
*/
CREATE PROC dbo.USER_AddContextualPropertySchema
	@chvContext 		VARCHAR(255),
	@chvPropertyName 	VARCHAR(255)
AS
	SET NOCOUNT ON

	INSERT dbo.USER_ContextualPropertySchema(ContextID, PropertyID)
	SELECT c.ContextID, ps.PropertyID
	FROM dbo.USER_Context c, dbo.USER_PropertySchema ps
	WHERE c.ContextName = @chvContext AND ps.PropertyName = @chvPropertyName
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_CreateContext
Description:	Creates a context
Parameters:	@chvContext
*/
CREATE PROC dbo.USER_CreateContext
	@chvContext 	VARCHAR(255)
AS
	SET NOCOUNT ON

	INSERT dbo.USER_Context(ContextName)
	VALUES(@chvContext)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
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

	DELETE dbo.USER_Property
	WHERE UserID = @guidUserID

	DELETE dbo.USER_User
	WHERE UserID = @guidUserID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
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

	DELETE p
	FROM dbo.USER_Property p
		JOIN dbo.USER_Context c ON p.ContextID = c.ContextID
	WHERE c.ContextName = @chvContext

	DELETE cps
	FROM dbo.USER_ContextualPropertySchema cps
		JOIN dbo.USER_Context c ON cps.ContextID = c.ContextID
	WHERE c.ContextName = @chvContext

	DELETE USER_Context
	WHERE ContextName = @chvContext
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_DeleteProperty
Description:	Deletes a property from a user
Parameters:	@guidUserID
		@chvPropertyName
		@chvContextName
*/
CREATE PROC dbo.USER_DeleteProperty
	@guidUserID 		UNIQUEIDENTIFIER,
	@chvPropertyName 	VARCHAR(255),
	@chvContextName 	VARCHAR(255)
AS		
	SET NOCOUNT ON
	
	DELETE p
	FROM dbo.USER_Property p
		LEFT JOIN dbo.USER_PropertySchema ps ON p.PropertyID = ps.PropertyID
		LEFT JOIN dbo.USER_Context c ON c.ContextID = p.ContextID
	WHERE p.UserID = @guidUserID AND ps.PropertyName = @chvPropertyName AND
		((@chvContextName IS NULL AND c.ContextID IS NULL) OR c.ContextName = @chvContextName)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_GetById
Description:	Gets a user
Parameters:	@guidUserID
*/
CREATE PROC dbo.USER_GetByID
	@guidUserID 	UNIQUEIDENTIFIER
AS	
	SET NOCOUNT ON
	
	SELECT UserID, Username, Domain
	FROM dbo.USER_User
	WHERE UserID = @guidUserID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_GetByUsername
Description:	Gets a user from a user name
Parameters:	@chvnUsername
		@chvDomain
*/
CREATE PROC dbo.USER_GetByUsername
	@chvnUsername 	NVARCHAR(255),
	@chvDomain 		VARCHAR(255)
AS
	SET NOCOUNT ON

	SELECT UserID, Username, Domain
	FROM dbo.USER_User
	WHERE Username = @chvnUsername AND Domain = @chvDomain
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_GetContexts
Description:	Gets all available contexts
*/
CREATE PROC dbo.USER_GetContexts
AS
	SET NOCOUNT ON

	SELECT ContextName 
	FROM dbo.USER_Context
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
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
		((@chvContext IS NULL AND up.ContextID = 0) OR uc.ContextName = @chvContext)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_GetPropertySchema
Description:	Gets properties for a context
Parameters:	@chvContext
*/
CREATE PROC dbo.USER_GetPropertySchema
	@chvContext 	VARCHAR(255)	= NULL
AS	
	SET NOCOUNT ON
	
	IF @chvContext IS NOT NULL
		SELECT ups.PropertyName, udt.TypeName DataType, udt.AssemblyName, udt.AssemblyPath
		FROM dbo.USER_PropertySchema ups
			JOIN dbo.USER_ContextualPropertySchema ucps ON ucps.PropertyID = ups.PropertyID 
			JOIN dbo.USER_Context uc ON uc.ContextID = ucps.ContextID
			JOIN dbo.USER_DataType udt ON ups.DataTypeID = udt.DataTypeID
		WHERE uc.ContextName = @chvContext
	ELSE
		SELECT ups.PropertyName, udt.TypeName DataType, udt.AssemblyName, udt.AssemblyPath
		FROM dbo.USER_PropertySchema ups 
			JOIN dbo.USER_DataType udt ON ups.DataTypeID = udt.DataTypeID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_GetUsersByDomain
Description:	Gets users from a domain
Parameters:	@chvDomain
*/
CREATE PROC dbo.USER_GetUsersByDomain
	@chvDomain	VARCHAR(255)
AS		
	SET NOCOUNT ON

	SELECT UserID, Username, Domain
	FROM dbo.USER_User
	WHERE Domain = @chvDomain
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/*
Name:       	USER_Update
Description:	Updates a user
Parameters:	@guidUserId
		@chvnUsername
		@chvDomain
Output:		STATUS	0 if insertion successful, 1 if username already taken
		UserID		New UserID
*/
CREATE PROC dbo.USER_Update
	@guidUserId		UNIQUEIDENTIFIER,
	@chvnUsername 	NVARCHAR(255),
	@chvDomain 		VARCHAR(255)
AS		
	SET NOCOUNT ON

	IF EXISTS (
		SELECT * 
		FROM dbo.USER_User 
		WHERE Domain = @chvDomain AND Username = @chvnUserName AND UserID <> @guidUserId)
		-- Username taken.
		SELECT 1 STATUS, NULL UserId	
	ELSE IF EXISTS (
		SELECT * 
		FROM dbo.USER_User 
		WHERE UserID = @guidUserId)
	BEGIN
		-- Update existing user.
		UPDATE dbo.USER_User 
		SET 	Username 	= @chvnUsername
		WHERE UserID = @guidUserId
		
		SELECT 0 STATUS, @guidUserId UserId
	END
	ELSE
	BEGIN	
		-- Inserting new user.
		INSERT dbo.USER_User(UserID, Username, Domain)
		VALUES(@guidUserId, @chvnUserName, @chvDomain)

		SELECT 0 STATUS, @guidUserId UserId
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
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
		SELECT PropertyID, @guidUserID, @chvnPropertyValue, @txtnExtPropertyValue, ISNULL((SELECT ContextID FROM dbo.USER_Context WHERE ContextName = @chvContext), 0)
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
DROP INDEX dbo.USER_User.IX_USER_User
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_USER_User_Username_Domain_unique ON dbo.USER_User
	(
	Username,
	[Domain]
	) ON [PRIMARY]
GO
COMMIT

exec sp_fulltext_table N'USER_Property', N'drop'

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
DROP INDEX dbo.USER_Property.IX_USER_Property
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_USER_Property_UniqueID_unique ON dbo.USER_Property
	(
	UniqueID
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
ALTER TABLE dbo.USER_User ALTER COLUMN UserID
	DROP ROWGUIDCOL
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
CREATE NONCLUSTERED INDEX IX_USER_PropertySchema_DataTypeID ON dbo.USER_PropertySchema
	(
	DataTypeID
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
CREATE NONCLUSTERED INDEX IX_USER_ContextualPropertySchema_ContextID ON dbo.USER_ContextualPropertySchema
	(
	ContextID
	) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_USER_ContextualPropertySchema_PropertyID ON dbo.USER_ContextualPropertySchema
	(
	PropertyID
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
ALTER TABLE dbo.USER_Context
	DROP CONSTRAINT IX_USER_Context_ContextName_unique
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_USER_Context_ContextName_unique ON dbo.USER_Context
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
ALTER TABLE dbo.USER_PropertySchema
	DROP CONSTRAINT IX_USER_PropertySchema_PropertyName_unique
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_USER_PropertySchema_PropertyName_unique ON dbo.USER_PropertySchema
	(
	PropertyName
	) ON [PRIMARY]
GO
COMMIT

if (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) <> 1 
exec sp_fulltext_database N'enable' 
GO

if not exists (select * from dbo.sysfulltextcatalogs where name = N'USER_Property')
exec sp_fulltext_catalog N'USER_Property', N'create' 
GO

exec sp_fulltext_table N'[dbo].[USER_Property]', N'create', N'USER_Property', N'IX_USER_Property_UniqueID_unique'
GO

exec sp_fulltext_column N'[dbo].[USER_Property]', N'PropertyValue', N'add', 1033  
GO

exec sp_fulltext_column N'[dbo].[USER_Property]', N'ExtendedPropertyValue', N'add', 1033  
GO

exec sp_fulltext_table N'[dbo].[USER_Property]', N'activate'  
GO

exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_change_tracking'
GO

exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_background_updateindex'
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

	SELECT DISTINCT Domain FROM USER_USER WHERE Domain <> ''
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

