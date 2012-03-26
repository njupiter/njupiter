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
	
INSERT USER_DataType(DataTypeID, TypeName) VALUES(5, 'nJupiter.DataAccess.Users.XmlSerializedProperty')
