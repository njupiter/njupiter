if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_USER_ContextualPropertySchema_USER_Context]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[USER_ContextualPropertySchema] DROP CONSTRAINT FK_USER_ContextualPropertySchema_USER_Context
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetPropertySchema]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetPropertySchema]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_DeleteProperty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_DeleteProperty]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetProperties]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetProperties]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_UpdateProperty]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_UpdateProperty]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_Delete]
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_GetUsersByDomain]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_GetUsersByDomain]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[USER_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[USER_Update]
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

if (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) <> 1 
exec sp_fulltext_database N'enable' 

GO


if exists (select * from dbo.sysfulltextcatalogs where name = N'USER_Property')
exec sp_fulltext_catalog N'USER_Property', N'drop'

GO

if not exists (select * from dbo.sysfulltextcatalogs where name = N'USER_Property')
exec sp_fulltext_catalog N'USER_Property', N'create' 

GO

CREATE TABLE [dbo].[USER_Context] (
	[ContextID] [int] NOT NULL ,
	[ContextName] [varchar] (4000) NOT NULL 
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
	[UserID]  uniqueidentifier ROWGUIDCOL  NOT NULL ,
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
	[ContextSchemaID] [int] IDENTITY (1, 1) NOT NULL ,
	[ContextID] [int] NOT NULL ,
	[PropertyID] [int] NOT NULL ,
	[SortOrder] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[USER_Property] (
	[UniqueID] [int] IDENTITY (1, 1) NOT NULL ,
	[PropertyID] [int] NOT NULL ,
	[PropertyValue] [nvarchar] (4000) NULL ,
	[ExtendedPropertyValue] [ntext] NULL ,
	[UserID] [uniqueidentifier] NOT NULL ,
	[ContextID] [int] NOT NULL 
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
		[ContextSchemaID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[USER_Property] WITH NOCHECK ADD 
	CONSTRAINT [PK_USER_Property] PRIMARY KEY  CLUSTERED 
	(
		[PropertyID],
		[UserID],
		[ContextID]
	)  ON [PRIMARY] 
GO

 CREATE  UNIQUE  INDEX [IX_USER_Property] ON [dbo].[USER_Property]([UniqueID]) ON [PRIMARY]
GO

if (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) <> 1 
exec sp_fulltext_database N'enable' 

GO

if not exists (select * from dbo.sysfulltextcatalogs where name = N'USER_Property')
exec sp_fulltext_catalog N'USER_Property', N'create' 

GO

exec sp_fulltext_table N'[dbo].[USER_Property]', N'create', N'USER_Property', N'IX_USER_Property'
GO

exec sp_fulltext_column N'[dbo].[USER_Property]', N'PropertyValue', N'add', 1033  
GO

exec sp_fulltext_column N'[dbo].[USER_Property]', N'ExtendedPropertyValue', N'add', 1033  
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
	),
	CONSTRAINT [FK_USER_ContextualPropertySchema_USER_PropertySchema] FOREIGN KEY 
	(
		[PropertyID]
	) REFERENCES [dbo].[USER_PropertySchema] (
		[PropertyID]
	)
GO

ALTER TABLE [dbo].[USER_Property] ADD 
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
	)
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
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
Name:       	USER_Delete
Description:	Delete user
Input:		guidUserID
Output:		-
*/
CREATE PROCEDURE dbo.USER_Delete
	@guidUserID UNIQUEIDENTIFIER
AS
	DECLARE @TranName NVARCHAR(20)
	SELECT @TranName = 'tranDeleteUser'
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRANSACTION @TranName

	DELETE 
	FROM USER_Property 
	WHERE UserID = @guidUserID
	DELETE 
	FROM USER_User 
	WHERE UserID = @guidUserID

	IF @@ERROR = 0
		COMMIT TRANSACTION @TranName
	ELSE
		ROLLBACK TRANSACTION @TranName
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
Description:	Selects a User from a supplied UserID
Parameters:	guidUserID
Output:		UserID
		Username
		Domain
*/

CREATE PROCEDURE dbo.USER_GetByID
	@guidUserID UNIQUEIDENTIFIER
AS		
	SELECT UserID, Username, Domain
	FROM USER_User
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
Name:       	USER_GETbyUsername
Description:	Selects a User matching exactly to a supplied name
Parameters:	chvnUsername
		chvDomain
Output:		UserID
		Username
		Domain
*/
CREATE PROCEDURE dbo.USER_GetByUsername
	@chvnUsername NVARCHAR(255),
	@chvDomain VARCHAR(255)
AS
	SELECT UserID, Username, Domain
	FROM USER_User
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
Description:	Get all available contexts
Input:		-
Output:		ContextName
*/
CREATE PROCEDURE dbo.USER_GetContexts
AS
	SELECT ContextName 
	FROM USER_Context
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
Description:	Gets a collection of users based on their domain
Parameters:	chvDomain
Output:		UserID
		Username
		Domain
*/

CREATE PROCEDURE dbo.USER_GetUsersByDomain
	@chvDomain VARCHAR(255)
AS		
	SELECT UserID, Username, Domain
	FROM USER_User
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
Description:	Adds a new user to the table with registered users.
Parameters:	guidUserId
		chvnUsername
		chvDomain
Output:		STATUS	0 if insertion successful, 1 if username already taken
		UserID		New UserID

*/
CREATE PROCEDURE dbo.USER_Update
	@guidUserId UNIQUEIDENTIFIER,
	@chvnUsername NVARCHAR(255),
	@chvDomain VARCHAR(255)
AS		
	IF  EXISTS(	
	  SELECT * 
	  FROM USER_User 
	  WHERE Domain = @chvDomain AND Username = @chvnUserName AND UserID <> @guidUserId)
		-- Username taken.
		SELECT 1 AS 'STATUS', NULL AS UserId	
	ELSE IF EXISTS(	
	  SELECT * 
	  FROM USER_User 
	  WHERE UserID = @guidUserId)
	BEGIN
		-- Update existing user.
		UPDATE USER_User 
		SET Username = @chvnUsername
		WHERE UserID = @guidUserId
		SELECT 0 as 'STATUS', @guidUserId AS UserId
	END
	ELSE
	BEGIN	
		-- Inserting new user.
		INSERT INTO USER_User (UserID, Username, Domain)
		VALUES (@guidUserId, @chvnUserName, @chvDomain)
		SELECT 0 AS 'STATUS', @guidUserId AS UserId
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
Name:       	USER_DeleteProperty
Description:	Deletes a certain field for a certain User
Parameters:	@guidUserID
		@intPropertyID
Output:		-
*/
CREATE PROCEDURE dbo.USER_DeleteProperty
	@guidUserID UNIQUEIDENTIFIER,
	@chvPropertyName VARCHAR(255),
	@chvContextName VARCHAR(255)
AS		
	DELETE FROM USER_Property
	FROM 	USER_Property LEFT JOIN 
		USER_PropertySchema ON USER_Property.PropertyID = USER_PropertySchema.PropertyID LEFT JOIN
		USER_Context ON USER_Context.ContextID = USER_Property.ContextID
	WHERE USER_Property.UserID = @guidUserID AND 
		USER_PropertySchema.PropertyName = @chvPropertyName AND 
		((@chvContextName IS NULL AND USER_Context.ContextID IS NULL) OR USER_Context.ContextName = @chvContextName)
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
Description:	gets property values for a certain UserID
Parameters:	guidUserID
Output:		
		PropertyName
		PropertyValue
		ExtendedPropertyValue
		DataType
*/
CREATE PROCEDURE dbo.USER_GetProperties
	@guidUserID UNIQUEIDENTIFIER,
	@chvContext VARCHAR(255) = NULL
AS
	SELECT UPS.PropertyName, UP.PropertyValue, UP.ExtendedPropertyValue, UDT.TypeName AS DataType, UDT.AssemblyName, UDT.AssemblyPath
	FROM 	USER_PropertySchema UPS LEFT JOIN
		USER_DataType UDT ON UPS.DataTypeID = UDT.DataTypeID LEFT JOIN
		USER_Property UP ON UPS.PropertyID = UP.PropertyID LEFT JOIN
		USER_Context UC ON UC.ContextID = UP.ContextID
	WHERE UP.UserID = @guidUserID AND 
		((@chvContext IS NULL AND UP.ContextID = 0) OR UC.ContextName = @chvContext)
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
Name:       	USER_UpdateField
Description:	Updates values in USER_fieldValue
Input:		guidUserID
		intFieldID
		chvnFieldValue
		txtnExtFieldValue
		chvContext
Output:		-
*/
CREATE PROCEDURE dbo.USER_UpdateProperty
	@guidUserID UNIQUEIDENTIFIER,
	@chvProperty VARCHAR(255),
	@chvnPropertyValue NVARCHAR(4000),
	@txtnExtPropertyValue NTEXT,
	@chvContext VARCHAR(255) = NULL
AS
	IF EXISTS(
	  SELECT p.PropertyID, UserID 
	  FROM	USER_Property p INNER JOIN 
		USER_PropertySchema ps ON p.PropertyID = ps.PropertyID LEFT JOIN
		USER_Context c ON p.ContextID = c.ContextID 
	  WHERE ps.PropertyName = @chvProperty AND UserID = @guidUserID AND 
		   ((@chvContext IS NULL AND c.ContextID IS NULL) OR c.ContextName = @chvContext))
		UPDATE USER_Property
		SET PropertyValue = @chvnPropertyValue, 
		ExtendedPropertyValue = @txtnExtPropertyValue
		FROM 	USER_Property p INNER JOIN 
			USER_PropertySchema ps ON p.PropertyID = ps.PropertyID INNER JOIN
			USER_Context c ON p.ContextID = c.ContextID
		WHERE ps.PropertyName = @chvProperty AND UserID = @guidUserID AND 
			((@chvContext IS NULL AND c.ContextID IS NULL) OR c.ContextName = @chvContext)
	ELSE
		INSERT INTO USER_Property(PropertyID, UserID, PropertyValue, ExtendedPropertyValue, ContextID)
		SELECT PropertyID, @guidUserID, @chvnPropertyValue, @txtnExtPropertyValue, ISNULL((SELECT ContextID FROM USER_Context WHERE ContextName = @chvContext), 0)
		FROM USER_PropertySchema ps
		WHERE PropertyName = @chvProperty
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
Description:	gets properties for a certain context
Parameters:	chvContext
Output:		PropertyName
		DataType
		AssemblyName
		AssemblyPath
*/
CREATE PROCEDURE dbo.USER_GetPropertySchema
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


exec sp_fulltext_table @tabname=N'USER_Property', @action=N'start_change_tracking'
exec sp_fulltext_table @tabname=N'USER_Property', @action=N'start_background_updateindex'

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



-- Insert default values

INSERT INTO USER_DataType(DataTypeID, TypeName) VALUES(0, 'nJupiter.DataAccess.Users.StringProperty')
INSERT INTO USER_DataType(DataTypeID, TypeName) VALUES(1, 'nJupiter.DataAccess.Users.IntProperty')
INSERT INTO USER_DataType(DataTypeID, TypeName) VALUES(2, 'nJupiter.DataAccess.Users.BoolProperty')
INSERT INTO USER_DataType(DataTypeID, TypeName) VALUES(3, 'nJupiter.DataAccess.Users.DateTimeProperty')
INSERT INTO USER_DataType(DataTypeID, TypeName) VALUES(4, 'nJupiter.DataAccess.Users.BinaryProperty')

INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(1, 'password', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(2, 'passwordSalt', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(3, 'firstName', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(4, 'middleName', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(5, 'lastName', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(6, 'description', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(7, 'wwwHomePage', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(8, 'streetAddress', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(9, 'streetAddress2', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(10, 'company', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(11, 'department', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(12, 'city', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(13, 'telephoneNumber', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(14, 'homePhone', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(15, 'mobile', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(16, 'postOfficeBox', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(17, 'postalCode', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(18, 'title', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(19, 'country', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(20, 'comment', 0)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(21, 'birthDate', 3)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(22, 'active', 2)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(23, 'object', 4)
INSERT INTO USER_PropertySchema(PropertyID, PropertyName, DataTypeID) VALUES(24, 'email', 0)

SET IDENTITY_INSERT USER_Context ON
INSERT INTO USER_Context(ContextID, ContextName) VALUES(1, 'registration') -- Do not use id 0. ContextID 0 is reserved for the default context
SET IDENTITY_INSERT USER_Context OFF

INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 1)
INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 2)
INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 3)
INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 4)
INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 5)
INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 21)
INSERT INTO USER_ContextualPropertySchema(ContextID, PropertyID) VALUES(1, 22)