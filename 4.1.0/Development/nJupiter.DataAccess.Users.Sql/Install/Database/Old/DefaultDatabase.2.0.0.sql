if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USER_ContextualPropertySchema_USER_Context]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[USER_ContextualPropertySchema] DROP CONSTRAINT FK_USER_ContextualPropertySchema_USER_Context
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USER_Property_USER_Context]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[USER_Property] DROP CONSTRAINT FK_USER_Property_USER_Context
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USER_PropertySchema_USER_DataType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[USER_PropertySchema] DROP CONSTRAINT FK_USER_PropertySchema_USER_DataType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USER_Property_USER_User]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[USER_Property] DROP CONSTRAINT FK_USER_Property_USER_User
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USER_ContextualPropertySchema_USER_PropertySchema]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[USER_ContextualPropertySchema] DROP CONSTRAINT FK_USER_ContextualPropertySchema_USER_PropertySchema
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USER_Property_USER_PropertySchema]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[USER_Property] DROP CONSTRAINT FK_USER_Property_USER_PropertySchema
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_ContextualPropertySchema]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[USER_ContextualPropertySchema]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_Property]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[USER_Property]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_PropertySchema]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[USER_PropertySchema]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_Context]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[USER_Context]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_DataType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[USER_DataType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[USER_User]
GO

CREATE TABLE [dbo].[USER_Context] (
	[ContextID] [int] IDENTITY (1, 1) NOT NULL ,
	[ContextName] [varchar] (255) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[USER_DataType] (
	[DataTypeID] [int] NOT NULL ,
	[TypeName] [varchar] (1024) NOT NULL ,
	[AssemblyName] [varchar] (1024) NULL ,
	[AssemblyPath] [varchar] (4096) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[USER_User] (
	[UserID] [uniqueidentifier] NOT NULL ,
	[Username] [nvarchar] (255) NOT NULL ,
	[Domain] [varchar] (255) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[USER_PropertySchema] (
	[PropertyID] [int] NOT NULL ,
	[PropertyName] [varchar] (255) NOT NULL ,
	[DataTypeID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[USER_ContextualPropertySchema] (
	[ContextID] [int] NOT NULL ,
	[PropertyID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[USER_Property] (
	[UniqueID] [int] IDENTITY (1, 1) NOT NULL ,
	[PropertyID] [int] NOT NULL ,
	[PropertyValue] [nvarchar] (4000) NULL ,
	[ExtendedPropertyValue] [ntext] NULL ,
	[UserID] [uniqueidentifier] NOT NULL ,
	[ContextID] [int] NULL ,
	[Timestamp] [timestamp] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[USER_Context] WITH NOCHECK ADD 
	CONSTRAINT [PK_USER_Context] PRIMARY KEY  CLUSTERED 
	(
		[ContextID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[USER_DataType] WITH NOCHECK ADD 
	CONSTRAINT [PK_USER_DataType] PRIMARY KEY  CLUSTERED 
	(
		[DataTypeID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[USER_User] WITH NOCHECK ADD 
	CONSTRAINT [PK_USER_User] PRIMARY KEY  CLUSTERED 
	(
		[UserID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[USER_PropertySchema] WITH NOCHECK ADD 
	CONSTRAINT [PK_USER_PropertySchema] PRIMARY KEY  CLUSTERED 
	(
		[PropertyID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[USER_ContextualPropertySchema] WITH NOCHECK ADD 
	CONSTRAINT [PK_USER_ContextualPropertySchema] PRIMARY KEY  CLUSTERED 
	(
		[ContextID],
		[PropertyID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[USER_Property] WITH NOCHECK ADD 
	CONSTRAINT [PK_USER_Property] PRIMARY KEY  CLUSTERED 
	(
		[UniqueID]
	)  ON [PRIMARY] 
GO

 CREATE  UNIQUE  INDEX [IX_USER_Context_ContextName_unique] ON [dbo].[USER_Context]([ContextID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[USER_User] ADD 
	CONSTRAINT [DF_USER_User_UserID] DEFAULT (newid()) FOR [UserID],
	CONSTRAINT [DF_USER_User_Domain] DEFAULT ('') FOR [Domain]
GO

 CREATE  UNIQUE  INDEX [IX_USER_User_Username_Domain_unique] ON [dbo].[USER_User]([Username], [Domain]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_USER_PropertySchema_PropertyName_unique] ON [dbo].[USER_PropertySchema]([PropertyName]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_USER_PropertySchema_DataTypeID] ON [dbo].[USER_PropertySchema]([DataTypeID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_USER_ContextualPropertySchema_PropertyID] ON [dbo].[USER_ContextualPropertySchema]([PropertyID]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_USER_Property_PropertyID_UserID_ContextID_unique] ON [dbo].[USER_Property]([PropertyID], [UserID], [ContextID]) ON [PRIMARY]
GO

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

ALTER TABLE [dbo].[USER_PropertySchema] ADD 
	CONSTRAINT [FK_USER_PropertySchema_USER_DataType] FOREIGN KEY 
	(
		[DataTypeID]
	) REFERENCES [dbo].[USER_DataType] (
		[DataTypeID]
	)
GO

ALTER TABLE [dbo].[USER_ContextualPropertySchema] ADD 
	CONSTRAINT [FK_USER_ContextualPropertySchema_USER_Context] FOREIGN KEY 
	(
		[ContextID]
	) REFERENCES [dbo].[USER_Context] (
		[ContextID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_USER_ContextualPropertySchema_USER_PropertySchema] FOREIGN KEY 
	(
		[PropertyID]
	) REFERENCES [dbo].[USER_PropertySchema] (
		[PropertyID]
	)
GO

ALTER TABLE [dbo].[USER_Property] ADD 
	CONSTRAINT [FK_USER_Property_USER_Context] FOREIGN KEY 
	(
		[ContextID]
	) REFERENCES [dbo].[USER_Context] (
		[ContextID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_USER_Property_USER_PropertySchema] FOREIGN KEY 
	(
		[PropertyID]
	) REFERENCES [dbo].[USER_PropertySchema] (
		[PropertyID]
	),
	CONSTRAINT [FK_USER_Property_USER_User] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [dbo].[USER_User] (
		[UserID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_change_tracking'
GO

exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_background_updateindex'
GO

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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetDomains]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetDomains]
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

	DELETE dbo.USER_Context
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
		SELECT PropertyID, @guidUserID, @chvnPropertyValue, @txtnExtPropertyValue, (SELECT ContextID FROM dbo.USER_Context WHERE ContextName = @chvContext)
		FROM dbo.USER_PropertySchema ps
		WHERE PropertyName = @chvProperty
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- Insert default values

INSERT USER_DataType(DataTypeID, TypeName) VALUES(0, 'nJupiter.DataAccess.Users.StringProperty')
INSERT USER_DataType(DataTypeID, TypeName) VALUES(1, 'nJupiter.DataAccess.Users.IntProperty')
INSERT USER_DataType(DataTypeID, TypeName) VALUES(2, 'nJupiter.DataAccess.Users.BoolProperty')
INSERT USER_DataType(DataTypeID, TypeName) VALUES(3, 'nJupiter.DataAccess.Users.DateTimeProperty')
INSERT USER_DataType(DataTypeID, TypeName) VALUES(4, 'nJupiter.DataAccess.Users.BinaryProperty')

INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(1, 'password', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(2, 'passwordSalt', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(3, 'firstName', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(4, 'middleName', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(5, 'lastName', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(6, 'description', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(7, 'wwwHomePage', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(8, 'streetAddress', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(9, 'streetAddress2', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(10, 'company', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(11, 'department', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(12, 'city', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(13, 'telephoneNumber', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(14, 'homePhone', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(15, 'mobile', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(16, 'postOfficeBox', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(17, 'postalCode', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(18, 'title', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(19, 'country', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(20, 'comment', 0)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(21, 'birthDate', 3)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(22, 'active', 2)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(23, 'object', 4)
INSERT USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(24, 'email', 0)

SET IDENTITY_INSERT USER_Context ON
INSERT USER_Context(ContextID, ContextName) VALUES(1, 'registration') 
SET IDENTITY_INSERT USER_Context OFF

INSERT USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 1)
INSERT USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 2)
INSERT USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 3)
INSERT USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 4)
INSERT USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 5)
INSERT USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 21)
INSERT USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 22)