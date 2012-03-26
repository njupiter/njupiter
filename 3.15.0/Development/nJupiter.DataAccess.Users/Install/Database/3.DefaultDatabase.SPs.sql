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

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go


CREATE PROC [dbo].[USER_FilterUsers]
	@chvUserName							VARCHAR(50)			= NULL,
	@intLimitPage							INT					= NULL,
	@intLimitSize							INT					= NULL
AS
	SET NOCOUNT ON

	DECLARE 	@intStartRow 			INT,
				@intWhereToStart 		INT,
				@intWhereToEnd			INT,
				@intPagingTotalNumber	INT

	DECLARE	@tblResultingUser	TABLE (
		Id			UNIQUEIDENTIFIER UNIQUE,
		Row 		INT IDENTITY PRIMARY KEY)

	INSERT @tblResultingUser(Id)
	SELECT UserID
	FROM dbo.USER_User
	WHERE @chvUserName IS NULL OR UserName LIKE @chvUserName + '%'
	ORDER BY UserName

	SELECT	@intPagingTotalNumber	= @@ROWCOUNT,
			@intStartRow 			= @@IDENTITY - @intPagingTotalNumber + 1,
			@intWhereToStart		= ISNULL((@intLimitPage - 1) * @intLimitSize + @intStartRow, @intStartRow),
			@intWhereToEnd			= @intWhereToStart + ISNULL(@intLimitSize, @intPagingTotalNumber) - 1

	SELECT UserId, UserName, Domain
	FROM @tblResultingUser r
		JOIN dbo.USER_User u ON r.Id = u.UserID
	WHERE r.Row BETWEEN @intWhereToStart AND @intWhereToEnd

	RETURN @intPagingTotalNumber

go