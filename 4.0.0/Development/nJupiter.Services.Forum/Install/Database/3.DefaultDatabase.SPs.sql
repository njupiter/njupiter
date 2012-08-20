if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_DeleteCategories]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_DeleteCategories]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_DeleteCategoryAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_DeleteCategoryAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_DeletePostAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_DeletePostAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_DeletePosts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_DeletePosts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_FilterCategories]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_FilterCategories]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_FilterPosts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_FilterPosts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_GetCategoryAttributes]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_GetCategoryAttributes]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_GetDomains]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_GetDomains]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_GetNumberOfPosts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_GetNumberOfPosts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_GetPostAttributes]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_GetPostAttributes]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_MovePosts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_MovePosts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_SaveCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_SaveCategory]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_SaveCategoryAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_SaveCategoryAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_SavePost]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_SavePost]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_SavePostAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_SavePostAttribute]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_DeleteCategories
Description:	Deletes categories
Input:			@guidID
				@chvnDomain
				@chvnName
Revision history:
	2005-04-13	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_DeleteCategories
	@guidID			UNIQUEIDENTIFIER	= NULL,
	@chvnDomain 	NVARCHAR(100) 		= NULL,
	@chvnName		NVARCHAR(100) 		= NULL
AS
	SET NOCOUNT ON

	EXEC dbo.FORUM_DeletePosts
		@guidCategoryID		= @guidID,
		@chvnDomain			= @chvnDomain,
		@chvnCategoryName	= @chvnName

	DELETE dbo.FORUM_Category
	WHERE ID = ISNULL(@guidID, ID) AND Domain = ISNULL(@chvnDomain, Domain) AND Name = ISNULL(@chvnName, Name)

	RETURN @@ROWCOUNT
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_DeleteCategoryAttribute
Description:	Deletes a category attribute
Input:			@chvAttributeName
				@guidCategoryID
Revision history:
	2005-04-13	Kai de Leeuw 	Created
	2006-02-21	Kai de Leeuw	Changed the parameter to be an attribute name instead of an id
*/
CREATE PROC dbo.FORUM_DeleteCategoryAttribute
	@chvAttributeName	VARCHAR(100),
	@guidCategoryID 	UNIQUEIDENTIFIER
AS
	SET NOCOUNT ON

	DELETE ca
	FROM dbo.FORUM_CategoryAttribute ca
		JOIN dbo.FORUM_CategoryAttributeType cat ON ca.CategoryAttributeTypeID = cat.ID
	WHERE cat.Name = @chvAttributeName AND ca.CategoryID = @guidCategoryID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_DeletePostAttribute
Description:	Deletes a post attribute
Input:			@chvAttributeName
				@guidPostID
Revision history:
	2005-04-13	Kai de Leeuw 	Created
	2006-02-21	Kai de Leeuw	Changed the parameter to be an attribute name instead of an id
*/
CREATE PROC dbo.FORUM_DeletePostAttribute
	@chvAttributeName 	VARCHAR(100),
	@guidPostID 		UNIQUEIDENTIFIER
AS
	SET NOCOUNT ON

	DELETE pa
	FROM dbo.FORUM_PostAttribute pa
		JOIN dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID
	WHERE pat.Name = @chvAttributeName AND pa.PostID = @guidPostID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_DeletePosts
Description:	Deletes posts
Input:			@guidID
				@bitDeleteOnlyChildren
				@guidCategoryID
				@chvnDomain
				@chvnCategoryName
				@dteUntil
Revision history:
	2005-04-13	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_DeletePosts
	@guidID					UNIQUEIDENTIFIER	= NULL,
	@bitDeleteOnlyChildren	BIT					= NULL,
	@guidCategoryID 		UNIQUEIDENTIFIER 	= NULL,
	@chvnDomain 			NVARCHAR(100) 		= NULL,
	@chvnCategoryName 		NVARCHAR(100) 		= NULL,
	@dteUntil				DATETIME			= NULL
AS
	SET NOCOUNT ON

	DECLARE	@intNumberOfRows		INT,
			@intTotalNumberOfRows	INT
	DECLARE @tblAncestorPost		TABLE (
		ID 	UNIQUEIDENTIFIER PRIMARY KEY)

	IF @guidID IS NULL
		INSERT @tblAncestorPost(ID)
		SELECT ancestor.ID
		FROM dbo.FORUM_Post ancestor
			JOIN dbo.FORUM_PostDerivedInformation pdi ON ancestor.ID = pdi.ID
			JOIN dbo.FORUM_Category c ON ancestor.CategoryID = c.ID
		WHERE c.ID = ISNULL(@guidCategoryID, c.ID) AND c.Domain = ISNULL(@chvnDomain, c.Domain) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
			pdi.TimeLastPostAll <= ISNULL(@dteUntil, pdi.TimeLastPostAll)
	ELSE IF @bitDeleteOnlyChildren <> '1'
		INSERT @tblAncestorPost(ID)
		SELECT p.ID
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
		WHERE p.ID = @guidID AND
			pdi.TimeLastPostAll <= ISNULL(@dteUntil, pdi.TimeLastPostAll)
	ELSE
		INSERT @tblAncestorPost(ID)
		SELECT p.ID
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
		WHERE p.ParentID = @guidID AND
			pdi.TimeLastPostAll <= ISNULL(@dteUntil, pdi.TimeLastPostAll)

	SELECT	@intNumberOfRows		= @@ROWCOUNT,
			@intTotalNumberOfRows	= 0

	WHILE @intNumberOfRows > 0
	BEGIN
		DELETE descendant
		FROM dbo.FORUM_Post descendant
			JOIN dbo.FORUM_PostDerivedInformation pdi ON descendant.ID = pdi.ID
			JOIN dbo.FORUM_PostRelation pr ON descendant.ID = pr.DescendantPostID
			JOIN @tblAncestorPost ancestor ON pr.AncestorPostID = ancestor.ID
		WHERE pdi.PostCountAll = 0

		SELECT	@intNumberOfRows		= @@ROWCOUNT,
				@intTotalNumberOfRows	= @intTotalNumberOfRows + @intNumberOfRows
	END

	DELETE p
	FROM dbo.FORUM_Post p
		JOIN @tblAncestorPost ancestor ON p.ID = ancestor.ID

	RETURN @intTotalNumberOfRows + @@ROWCOUNT
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_FilterCategories
Description:	Filters categories
Input:			@chvnDomain
				@guidID
				@guidPostID
				@chvnName
				@bitIncludeHidden
				@bitGetAttributes
				@bitPostIncludeHidden
				@dtePostDateFilterFrom
				@dtePostDateFilterTo
				@intPostDateFilterColumn
				@intPostLevels
				@intPostLimitSize
				@intPostLimitSortColumn
				@chvPostLimitSortAttributeName
				@chvPostLimitSortAttributeDefaultValue
				@bitPostLimitSortDirectionAsc
				@intPostLimitPage
				@bitPostGetAttributes
Revision history:
	2005-04-11	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_FilterCategories
	@chvnDomain 							VARCHAR(100)		= NULL,
	@guidID 								UNIQUEIDENTIFIER	= NULL,
	@guidPostID								UNIQUEIDENTIFIER	= NULL,
	@chvnName 								NVARCHAR(100) 		= NULL,
	@bitIncludeHidden 						BIT 				= NULL,
	@bitGetAttributes 						BIT 				= NULL,
	@bitPostIncludeHidden					BIT					= NULL,
	@dtePostDateFilterFrom					DATETIME			= NULL,
	@dtePostDateFilterTo					DATETIME			= NULL,
	@intPostDateFilterColumn				INT					= NULL,
	@intPostLevels 							INT 				= NULL,
	@intPostLimitSize 						INT 				= NULL,
	@intPostLimitSortColumn 				INT 				= NULL,
	@chvPostLimitSortAttributeName			VARCHAR(50)			= NULL,
	@chvnPostLimitSortAttributeDefaultValue	NVARCHAR(4000)		= NULL,
	@bitPostLimitSortDirectionAsc 			BIT 				= NULL,
	@intPostLimitPage 						INT 				= NULL,
	@bitPostGetAttributes 					BIT 				= NULL
AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE		@tblResultCategory	TABLE (
		ID 				UNIQUEIDENTIFIER UNIQUE NULL,
		Name 			NVARCHAR(100) NULL,
		Domain			NVARCHAR(100) NULL,
		Visible 		BIT NULL,
		RootPostCount 	INT NULL,
		Timestamp		BINARY(8) NULL,
		Row				INT IDENTITY PRIMARY KEY)

	DECLARE 	@intLoopIndex		INT

	IF @guidPostID IS NOT NULL
		INSERT @tblResultCategory(ID, Name, Domain, Visible, RootPostCount, Timestamp)
		SELECT c.ID, c.Name, c.Domain, c.Visible,
		 	(SELECT COUNT(*)
		 	FROM dbo.FORUM_Post
		 	WHERE CategoryID = c.ID AND (@bitPostIncludeHidden <> '0' OR Visible = '1')), c.Timestamp
		FROM dbo.FORUM_Category c
			JOIN dbo.FORUM_Post p ON c.ID = p.CategoryID
		WHERE p.ID = @guidPostID OR EXISTS (
			SELECT *
			FROM dbo.FORUM_PostRelation
			WHERE AncestorPostID = p.ID AND DescendantPostID = @guidPostID)
	ELSE
		INSERT @tblResultCategory(ID, Name, Domain, Visible, RootPostCount, Timestamp)
		SELECT c.ID, c.Name, c.Domain, c.Visible,
			(SELECT COUNT(*)
			FROM dbo.FORUM_Post 
			WHERE CategoryID = c.ID AND (@bitPostIncludeHidden <> '0' OR Visible = '1')), c.Timestamp
		FROM dbo.FORUM_Category c
		WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND c.ID = ISNULL(@guidID, c.ID) AND c.Name = ISNULL(@chvnName, c.Name) AND
			(@bitIncludeHidden <> '0' OR @guidID IS NOT NULL OR @chvnName IS NOT NULL OR c.Visible = '1')

	SELECT	@intLoopIndex	= @@ROWCOUNT

	SELECT ID, Name, Domain, Visible, RootPostCount, Timestamp
	FROM @tblResultCategory

	IF @bitGetAttributes <> '0'
		SELECT cat.Name, c.ID, ca.Value, ca.ExtendedValue
		FROM @tblResultCategory c
			JOIN dbo.FORUM_CategoryAttribute ca ON c.ID = ca.CategoryID
			JOIN dbo.FORUM_CategoryAttributeType cat ON ca.CategoryAttributeTypeID = cat.ID
	
	IF @intPostLevels IS NULL OR @intPostLevels > 0
	BEGIN
		CREATE TABLE dbo.#CategoryResultPost (
			ID 					UNIQUEIDENTIFIER PRIMARY KEY,
			EffectivelyVisible	BIT NULL)
		DECLARE 	@tblPagingTotalCount	TABLE (
			ID 							UNIQUEIDENTIFIER NULL,
			PagingTotalNumberOfPosts 	INT NULL)

		WHILE @intLoopIndex > 0
		BEGIN
			DECLARE 	@intPagingTotalNumberOfPosts 	INT,
						@guidLoopID						UNIQUEIDENTIFIER

			SELECT 	@guidLoopID 	= ID,
					@intLoopIndex 	= @intLoopIndex - 1
			FROM @tblResultCategory
			WHERE Row = @intLoopIndex

			INSERT dbo.#CategoryResultPost(ID, EffectivelyVisible)
			EXEC @intPagingTotalNumberOfPosts = dbo.FORUM_FilterPosts
				@guidCategoryID 					= @guidLoopID,
				@intLevels 							= @intPostLevels,
				@bitIncludeHidden 					= @bitPostIncludeHidden,
				@dteDateFilterFrom					= @dtePostDateFilterFrom,
				@dteDateFilterTo					= @dtePostDateFilterTo,
				@intDateFilterColumn				= @intPostDateFilterColumn,
				@intLimitSize 						= @intPostLimitSize,
				@intLimitSortColumn 				= @intPostLimitSortColumn,
				@chvLimitSortAttributeName			= @chvPostLimitSortAttributeName,
				@chvnLimitSortAttributeDefaultValue	= @chvnPostLimitSortAttributeDefaultValue,
				@bitLimitSortDirectionAsc			= @bitPostLimitSortDirectionAsc,
				@intLimitPage 						= @intPostLimitPage,
				@bitGetOnlyMetaData 				= '1'

			INSERT @tblPagingTotalCount(ID, PagingTotalNumberOfPosts)
			VALUES(@guidLoopID, @intPagingTotalNumberOfPosts)
		END

		SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible, p.Timestamp,
			CASE WHEN @bitPostIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END PostCount,
			CASE WHEN @bitPostIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END TimeLastPost,
			rp.EffectivelyVisible, c.ID EffectiveCategoryID, c.Name EffectiveCategoryName, p.ID RootPostID, p.Title RootPostTitle
		FROM dbo.#CategoryResultPost rp
			JOIN dbo.FORUM_Post p ON p.ID = rp.ID
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			JOIN dbo.FORUM_Category c ON p.CategoryID = c.ID
		UNION ALL
		SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible, p.Timestamp,
			CASE WHEN @bitIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END,
			CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END,
			rp.EffectivelyVisible, c.ID, c.Name, root.ID, root.Title
		FROM dbo.#CategoryResultPost rp
			JOIN dbo.FORUM_Post p ON p.ID = rp.ID
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
			JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
			JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID

		SELECT ID, PagingTotalNumberOfPosts
		FROM @tblPagingTotalCount

		IF @bitPostGetAttributes <> '0'
			SELECT pat.Name, p3.ID, pa.Value, pa.ExtendedValue
			FROM dbo.#CategoryResultPost p3
				JOIN dbo.FORUM_PostAttribute pa ON p3.ID = pa.PostID
				JOIN dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID

		DROP TABLE dbo.#CategoryResultPost
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_FilterPosts
Description:	Filters posts
Input:			@guidID
				@guidImmediateDescendantID
				@guidRootDescendantID
				@bitGetOnlyChildren
				@bitGetNonThreaded
				@guidCategoryID
				@chvnCategoryName
				@chvnDomain
				@bitIncludeHidden
				@dteDateFilterFrom
				@dteDateFilterTo
				@intDateFilterColumn
				@intLevels
				@chvnSearchText
				@intLimitSize
				@intLimitSortColumn
				@chvLimitSortAttributeName
				@chvnLimitSortAttributeDefaultValue
				@bitLimitSortDirectionAsc
				@intLimitPage
				@bitGetOnlyMetaData
				@bitGetAttributes
				@txtnPostsSearchQuery
Revision history:
	2005-04-11	Kai de Leeuw 	Created
*/
--TODO:
--optimize by fetching directly when not limiting (makes page, sortcolumn, sortasc meaningless)
--search on levels
--eliminate emptying the @tblUnFinishedBranch table
CREATE PROC dbo.FORUM_FilterPosts
	@guidID 							UNIQUEIDENTIFIER 	= NULL,
	@guidImmediateDescendantID 			UNIQUEIDENTIFIER 	= NULL,
	@guidRootDescendantID 				UNIQUEIDENTIFIER 	= NULL,
	@bitGetOnlyChildren 				BIT 				= NULL,
	@bitGetNonThreaded					BIT					= NULL,
	@guidCategoryID 					UNIQUEIDENTIFIER 	= NULL,
	@chvnCategoryName 					NVARCHAR(100) 		= NULL,
	@chvnDomain 						NVARCHAR(100) 		= NULL,
	@bitIncludeHidden 					BIT 				= NULL,
	@dteDateFilterFrom					DATETIME			= NULL,
	@dteDateFilterTo					DATETIME			= NULL,
	@intDateFilterColumn				INT					= NULL,
	@intLevels 							INT 				= NULL,
	@chvnSearchText 					NVARCHAR(4000) 		= NULL,
	@chvUserIdentity					VARCHAR(255)		= NULL,
	@intLimitSize 						INT 				= NULL,
	@intLimitSortColumn 				INT 				= NULL,
	@chvLimitSortAttributeName			VARCHAR(50)			= NULL,
	@chvnLimitSortAttributeDefaultValue	NVARCHAR(4000)		= NULL,
	@bitLimitSortDirectionAsc 			BIT 				= NULL,
	@intLimitPage 						INT 				= NULL,
	@bitGetOnlyMetaData 				BIT 				= NULL,
	@bitGetAttributes					BIT					= NULL,
	@txtnPostsSearchQuery				NTEXT				= NULL
AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE		@tblUnFinishedBranch	TABLE (
		ID 					UNIQUEIDENTIFIER PRIMARY KEY,
		EffectivelyVisible	BIT NULL,
		Rank 				SMALLINT NULL)

	DECLARE 	@tblResultPost			TABLE (
		ID 					UNIQUEIDENTIFIER UNIQUE NULL,
		Rank 				SMALLINT NULL,
		EffectivelyVisible	BIT NULL,
		Row 				INT IDENTITY PRIMARY KEY)

	DECLARE 	@intStartRow 					INT,
				@intWhereToStart 				INT,
				@intHowMany 					INT,
				@intPagingTotalNumberOfPosts 	INT,
				@intOldLevels 					INT,
				@bitFetchBranches 				BIT,
				@guidSingleID 					UNIQUEIDENTIFIER,
				@bitInsertSingleID				BIT

	IF NOT(@txtnPostsSearchQuery IS NULL AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL)
		CREATE TABLE dbo.#SearchedPost (
			ID	UNIQUEIDENTIFIER PRIMARY KEY)

	IF @txtnPostsSearchQuery IS NOT NULL
		INSERT dbo.#SearchedPost(ID)
		EXEC dbo.sp_executesql
			@stmt = @txtnPostsSearchQuery

	IF @bitGetOnlyChildren <> '1'
		IF @guidID IS NOT NULL
			SELECT @guidSingleID = @guidID
		ELSE IF @guidImmediateDescendantID IS NOT NULL
			SELECT @guidSingleID = ParentID
			FROM dbo.FORUM_Post
			WHERE ID = @guidImmediateDescendantID
		ELSE IF @guidRootDescendantID IS NOT NULL
			SELECT @guidSingleID = pr.AncestorPostID
			FROM dbo.FORUM_PostRelation pr
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation
					WHERE DescendantPostID = pr.AncestorPostID)

	IF @chvnSearchText IS NOT NULL
		IF @guidSingleID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT @guidSingleID, search.Rank, '1'
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_PostDerivedInformation pdi ON root.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON root.ID = search.[KEY]
			WHERE root.ID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR root.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = root.ID))
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootdescendant.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE relation.AncestorPostID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT child.ID, search.Rank, child.Visible
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_PostDerivedInformation pdi ON child.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON child.ID = search.[KEY]
			WHERE child.ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR child.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE child.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE child.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = child.ID)) AND
				(@bitIncludeHidden <> '0' OR child.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post child ON relation.AncestorPostID = child.ID
				JOIN dbo.FORUM_Post childdescendant ON relation.DescendantPostID = childdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON childdescendant.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE child.ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR childdescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE childdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE childdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidImmediateDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT parentchild.ID, search.Rank, parentchild.Visible
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchild.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON parentchild.ID = search.[KEY]
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchild.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = parentchild.ID)) AND
				(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank, relation.DescendantVisible
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON parentchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post parentchilddescendant ON relation.DescendantPostID = parentchilddescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchilddescendant.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchilddescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchilddescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchilddescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidRootDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT rootchild.ID, search.Rank, rootchild.Visible
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootchild.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON rootchild.ID = search.[KEY]
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchild.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = rootchild.ID)) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation 
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR rootchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON rootchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootchilddescendant ON relation.DescendantPostID = rootchilddescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootchilddescendant.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON rootchild.ID = search.[KEY]
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchilddescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchilddescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchilddescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT root.ID, search.Rank, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible ELSE root.Visible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_PostDerivedInformation pdi ON root.ID = pdi.ID
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON root.ID = search.[KEY]
			WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR root.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = root.ID)) AND
				(@bitIncludeHidden <> '0' OR
					(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1') OR
					((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1'))
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible & relation.DescendantVisible ELSE root.Visible & relation.DescendantVisible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN dbo.FORUM_PostRelation relation ON root.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootdescendant.ID = pdi.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR
					(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1' AND relation.DescendantVisible = '1') OR
					((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1' AND relation.DescendantVisible = '1'))
	ELSE IF @chvUserIdentity IS NOT NULL OR @txtnPostsSearchQuery IS NOT NULL
		IF @guidSingleID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT @guidSingleID, '1'
			FROM dbo.FORUM_Post p
				JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			WHERE p.ID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR p.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = p.ID))
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootdescendant.ID = pdi.ID
			WHERE relation.AncestorPostID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT p.ID, p.Visible
			FROM dbo.FORUM_Post p
				JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			WHERE p.ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR p.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = p.ID)) AND
				(@bitIncludeHidden <> '0' OR p.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post child ON relation.AncestorPostID = child.ID
				JOIN dbo.FORUM_Post childdescendant ON relation.DescendantPostID = childdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON childdescendant.ID = pdi.ID
			WHERE child.ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR childdescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE childdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE childdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidImmediateDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT parentchild.ID, parentchild.Visible
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchild.ID = pdi.ID
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchild.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = parentchild.ID)) AND
				(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON parentchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post parentchilddescendant ON relation.DescendantPostID = parentchilddescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchilddescendant.ID = pdi.ID
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchilddescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchilddescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchilddescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidRootDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT rootchild.ID, rootchild.Visible
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootchild.ID = pdi.ID
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchild.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = rootchild.ID)) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation 
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR rootchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON rootchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootchilddescendant ON relation.DescendantPostID = rootchilddescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootchilddescendant.ID = pdi.ID
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchilddescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchilddescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchilddescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT root.ID, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible ELSE root.Visible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_PostDerivedInformation pdi ON root.ID = pdi.ID
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
			WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR root.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = root.ID)) AND
				(@bitIncludeHidden <> '0' OR
					(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1') OR
					((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1'))
			UNION ALL
			SELECT relation.DescendantPostID, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible & relation.DescendantVisible ELSE root.Visible & relation.DescendantVisible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN dbo.FORUM_PostRelation relation ON root.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootdescendant.ID = pdi.ID
			WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@txtnPostsSearchQuery IS NULL OR
				EXISTS (
					SELECT *
					FROM dbo.#SearchedPost
					WHERE ID = relation.DescendantPostID)) AND
				(@bitIncludeHidden <> '0' OR
					(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1' AND relation.DescendantVisible = '1') OR
					((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1' AND relation.DescendantVisible = '1'))
	ELSE IF @bitGetNonThreaded = '1'
		IF @guidSingleID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT @guidSingleID, '1'
			FROM dbo.FORUM_Post p
				JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			WHERE p.ID = @guidSingleID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo)
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootdescendant.ID = pdi.ID
			WHERE relation.AncestorPostID = @guidSingleID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT p.ID, p.Visible
			FROM dbo.FORUM_Post p
				JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			WHERE p.ParentID = @guidID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR p.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post child ON relation.AncestorPostID = child.ID
				JOIN dbo.FORUM_Post childdescendant ON relation.DescendantPostID = childdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON childdescendant.ID = pdi.ID
			WHERE child.ParentID = @guidID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE childdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE childdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidImmediateDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT parentchild.ID, parentchild.Visible
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchild.ID = pdi.ID
			WHERE child.ID = @guidImmediateDescendantID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON parentchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post parentchilddescendant ON relation.DescendantPostID = parentchilddescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchilddescendant.ID = pdi.ID
			WHERE child.ID = @guidImmediateDescendantID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchilddescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchilddescendant.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidRootDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT rootchild.ID, rootchild.Visible
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootchild.ID = pdi.ID
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END <= @dteDateFilterTo) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation 
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR rootchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, relation.DescendantVisible
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON rootchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootchilddescendant ON relation.DescendantPostID = rootchilddescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootchilddescendant.ID = pdi.ID
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchilddescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchilddescendant.TimePosted END <= @dteDateFilterTo) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT root.ID, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible ELSE root.Visible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_PostDerivedInformation pdi ON root.ID = pdi.ID
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
			WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR
					(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1') OR
					((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1'))
			UNION ALL
			SELECT relation.DescendantPostID, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible & relation.DescendantVisible ELSE root.Visible & relation.DescendantVisible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN dbo.FORUM_PostRelation relation ON root.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootdescendant.ID = pdi.ID
			WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootdescendant.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR
					(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1' AND relation.DescendantVisible = '1') OR
					((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1' AND relation.DescendantVisible = '1'))
	ELSE IF @guidSingleID IS NOT NULL
	BEGIN
		SELECT @bitInsertSingleID	= '1'

		IF @intLevels <> 1
		BEGIN
			IF @intLevels IS NOT NULL
				SELECT @intLevels	= @intLevels - 1

			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT p.ID, p.Visible
			FROM dbo.FORUM_Post p
				JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			WHERE p.ParentID = @guidSingleID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR p.Visible = '1')
		END
	END
	ELSE IF @guidID IS NOT NULL
		INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
		SELECT p.ID, p.Visible
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
		WHERE p.ParentID = @guidID AND
			(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR p.Visible = '1')
	ELSE IF @guidImmediateDescendantID IS NOT NULL
		INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
		SELECT parentchild.ID, parentchild.Visible
		FROM dbo.FORUM_Post child
			JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
			JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchild.ID = pdi.ID
		WHERE child.ID = @guidImmediateDescendantID AND
			(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
	ELSE IF @guidRootDescendantID IS NOT NULL
		INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
		SELECT rootchild.ID, rootchild.Visible
		FROM dbo.FORUM_PostRelation pr
			JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
			JOIN dbo.FORUM_PostDerivedInformation pdi ON rootchild.ID = pdi.ID
		WHERE pr.DescendantPostID = @guidRootDescendantID AND
			NOT EXISTS (
				SELECT *
				FROM dbo.FORUM_PostRelation
				WHERE DescendantPostID = pr.AncestorPostID) AND
			(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE rootchild.TimePosted END <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR rootchild.Visible = '1')
	ELSE
		INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
		SELECT root.ID, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible ELSE root.Visible END
		FROM dbo.FORUM_Post root
			JOIN dbo.FORUM_PostDerivedInformation pdi ON root.ID = pdi.ID
			JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
		WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
			(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR
				(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1') OR
				((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1'))

	SELECT	@intPagingTotalNumberOfPosts 	= @@ROWCOUNT

	IF NOT(@txtnPostsSearchQuery IS NULL AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL)
		DROP TABLE dbo.#SearchedPost

	IF @intLimitSize IS NOT NULL AND @bitGetNonThreaded <> '1' AND @intLevels <> 1 AND @txtnPostsSearchQuery IS NULL AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL
		SELECT	@intOldLevels 		= @intLevels,
				@intLevels 			= 1,
				@bitFetchBranches 	= '1'

	WHILE 1 = 1
	BEGIN
		IF @intLevels <> 1 AND @bitGetNonThreaded <> '1' AND @txtnPostsSearchQuery IS NULL AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL
			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT descendantpdi.ID, ancestor.EffectivelyVisible & relation.DescendantVisible
			FROM @tblUnFinishedBranch ancestor
				JOIN dbo.FORUM_PostDerivedInformation ancestorpdi ON ancestor.ID = ancestorpdi.ID
				JOIN dbo.FORUM_PostRelation relation ON ancestorpdi.ID = relation.AncestorPostID
				JOIN dbo.FORUM_PostDerivedInformation descendantpdi ON relation.DescendantPostID = descendantpdi.ID
			WHERE (@intLevels IS NULL OR @intLevels > descendantpdi.Level - ancestorpdi.Level) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')

		SELECT	@intHowMany 	= ISNULL(@intLimitSize, 0) * ISNULL(@intLimitPage, 1)
		SET ROWCOUNT @intHowMany

		IF @intHowMany = 0 OR @intLimitSortColumn NOT BETWEEN 0 AND 17
			INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
			SELECT ID, Rank, EffectivelyVisible
			FROM @tblUnFinishedBranch
		ELSE IF @intLimitSortColumn = 0
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM @tblUnFinishedBranch
				ORDER BY ID DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM @tblUnFinishedBranch
				ORDER BY ID
		ELSE IF @intLimitSortColumn = 1
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.ParentID DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.ParentID, p.TimePosted
		ELSE IF @intLimitSortColumn = 2
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.CategoryID DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.CategoryID, p.TimePosted
		ELSE IF @intLimitSortColumn = 3
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.UserIdentity DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.UserIdentity, p.TimePosted
		ELSE IF @intLimitSortColumn = 4
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Author DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Author, p.TimePosted
		ELSE IF @intLimitSortColumn = 5
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Title DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Title, p.TimePosted
		ELSE IF @intLimitSortColumn = 6
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY CAST(p.Body AS NVARCHAR(4000)) DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY CAST(p.Body AS NVARCHAR(4000)), p.TimePosted
		ELSE IF @intLimitSortColumn = 7
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.TimePosted DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.TimePosted
		ELSE IF @intLimitSortColumn = 8
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Visible DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Visible, p.TimePosted
		ELSE IF @intLimitSortColumn = 9
			IF @bitLimitSortDirectionAsc = '0'
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.PostCountAll DESC, p.TimePosted
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.PostCountVisible DESC, p.TimePosted
			ELSE
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.PostCountAll, p.TimePosted
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.PostCountVisible, p.TimePosted
		ELSE IF @intLimitSortColumn = 10
			IF @bitLimitSortDirectionAsc = '0'
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.TimeLastPostAll DESC, p.TimePosted
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.TimeLastPostVisible DESC, p.TimePosted
			ELSE
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.TimeLastPostAll, p.TimePosted
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					ORDER BY pdi.TimeLastPostVisible, p.TimePosted
		ELSE IF @intLimitSortColumn = 11
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY match.Rank DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY match.Rank, p.TimePosted
		ELSE IF @intLimitSortColumn = 12
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY match.EffectivelyVisible DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY match.EffectivelyVisible, p.TimePosted
		ELSE IF @intLimitSortColumn = 13
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, p.CategoryID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_Category c ON p.CategoryID = c.ID
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, root.CategoryID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
						JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				) match
				ORDER BY match.CategoryID DESC, match.TimePosted
			ELSE 
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, p.CategoryID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_Category c ON p.CategoryID = c.ID
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, root.CategoryID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
						JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				) match
				ORDER BY match.CategoryID, match.TimePosted
		ELSE IF @intLimitSortColumn = 14
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, c.Name, c.ID CategoryID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_Category c ON p.CategoryID = c.ID
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, c.Name, c.ID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
						JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				) match
				ORDER BY match.Name DESC, match.CategoryID, match.TimePosted
			ELSE 
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, c.Name, c.ID CategoryID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
						JOIN dbo.FORUM_Category c ON p.CategoryID = c.ID
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, c.Name, c.ID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
						JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				) match
				ORDER BY match.Name, match.CategoryID, match.TimePosted
		ELSE IF @intLimitSortColumn = 15
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Timestamp DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Timestamp
		ELSE IF @intLimitSortColumn = 16
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, p.ID RootPostID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
					WHERE p.ParentID IS NULL
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, root.ID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
					WHERE root.ParentID IS NULL
				) match
				ORDER BY match.RootPostID DESC, match.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, p.ID RootPostID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
					WHERE p.ParentID IS NULL
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, root.ID, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
					WHERE root.ParentID IS NULL
				) match
				ORDER BY match.RootPostID, match.TimePosted
		ELSE IF @intLimitSortColumn = 17
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, p.ID RootPostID, p.Title, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
					WHERE p.ParentID IS NULL
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, root.ID, root.Title, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
					WHERE root.ParentID IS NULL
				) match
				ORDER BY match.Title DESC, match.RootPostID, match.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM (
					SELECT match.ID, match.Rank, match.EffectivelyVisible, p.ID RootPostID, p.Title, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON match.ID = p.ID
					WHERE p.ParentID IS NULL
					UNION ALL
					SELECT match.ID, match.Rank, match.EffectivelyVisible, root.ID, root.Title, p.TimePosted
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_Post p ON p.ID = match.ID
						JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
						JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
					WHERE root.ParentID IS NULL
				) match
				ORDER BY match.Title, match.RootPostID, match.TimePosted
		ELSE IF @chvLimitSortAttributeName IS NOT NULL
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
					LEFT JOIN (
						SELECT pa.Value, pa.ExtendedValue, pa.PostID
						FROM dbo.FORUM_PostAttribute pa
							JOIN dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID AND pat.Name = @chvLimitSortAttributeName) pa
					ON p.ID = pa.PostID
				ORDER BY COALESCE(pa.Value, CAST(pa.ExtendedValue AS NVARCHAR(4000)), @chvnLimitSortAttributeDefaultValue) DESC, p.TimePosted
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
					LEFT JOIN (
						SELECT pa.Value, pa.ExtendedValue, pa.PostID
						FROM dbo.FORUM_PostAttribute pa
							JOIN dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID AND pat.Name = @chvLimitSortAttributeName) pa
					ON p.ID = pa.PostID
				ORDER BY COALESCE(pa.Value, CAST(pa.ExtendedValue AS NVARCHAR(4000)), @chvnLimitSortAttributeDefaultValue), p.TimePosted

		SELECT 	@intStartRow 		= ISNULL(SCOPE_IDENTITY(), 0) - @@ROWCOUNT + 1,
				@intWhereToStart	= ISNULL((@intLimitPage - 1) * @intLimitSize + @intStartRow, @intStartRow)

		SET ROWCOUNT 0

		IF @bitFetchBranches = '1'
		BEGIN
			DELETE @tblUnFinishedBranch

			INSERT @tblUnFinishedBranch(ID, EffectivelyVisible)
			SELECT ID, EffectivelyVisible
			FROM @tblResultPost
			WHERE Row >= @intWhereToStart

			SELECT	@intLevels 			= @intOldLevels,
					@intLimitSize 		= NULL,
					@bitFetchBranches	= '0'
		END
		ELSE
		BEGIN
			IF @bitInsertSingleID = '1'
				INSERT @tblResultPost(ID, EffectivelyVisible)
				VALUES(@guidSingleID, '1')

			IF @bitGetOnlyMetaData = '1'
				SELECT ID, EffectivelyVisible
				FROM @tblResultPost
				WHERE Row >= @intWhereToStart
			ELSE
			BEGIN
				SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible, p.Timestamp,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END PostCount,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END TimeLastPost,
					rp.Rank, rp.EffectivelyVisible, c.ID EffectiveCategoryID, c.Name EffectiveCategoryName, p.ID RootPostID, p.Title RootPostTitle
				FROM @tblResultPost rp
					JOIN dbo.FORUM_Post p ON rp.ID = p.ID
					JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					JOIN dbo.FORUM_Category c ON p.CategoryID = c.ID
				WHERE rp.Row >= @intWhereToStart
				UNION ALL
				SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible, p.Timestamp,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END,
					rp.Rank, rp.EffectivelyVisible, c.ID, c.Name, root.ID, root.Title
				FROM @tblResultPost rp
					JOIN dbo.FORUM_Post p ON rp.ID = p.ID
					JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					JOIN dbo.FORUM_PostRelation pr ON pr.DescendantPostID = p.ID
					JOIN dbo.FORUM_Post root ON root.ID = pr.AncestorPostID
					JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				WHERE rp.Row >= @intWhereToStart

				IF @bitGetAttributes <> '0'
					SELECT pat.Name, rp.ID, pa.Value, pa.ExtendedValue
					FROM @tblResultPost rp
						JOIN dbo.FORUM_PostAttribute pa ON rp.ID = pa.PostID
						JOIN dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID
					WHERE rp.Row >= @intWhereToStart
			END
			BREAK
		END
	END

	RETURN @intPagingTotalNumberOfPosts
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_GetCategoryAttributes
Description:	Retrieves the category attributes
Input:
Revision history:
	2005-04-18	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_GetCategoryAttributes
AS
	SET NOCOUNT ON

	SELECT cat.Name, adt.TypeName, adt.AssemblyName, adt.AssemblyPath
	FROM dbo.FORUM_CategoryAttributeType cat
		JOIN dbo.FORUM_AttributeDataType adt ON cat.AttributeDataTypeID = adt.ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_GetDomains
Description:	Retrieves all used domains
Input:
Revision history:
	2007-04-17	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_GetDomains
AS
	SET NOCOUNT ON

	SELECT DISTINCT Domain
	FROM dbo.FORUM_Category
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_GetNumberOfPosts
Description:	Gets number of posts
Input:			@guidPostID
				@guidCategoryID
				@chvnCategoryName
				@chvnDomain
				@bitIncludeHidden
				@dteDateFilterFrom
				@dteDateFilterTo
Revision history:
	2007-09-03	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_GetNumberOfPosts
	@guidPostID 		UNIQUEIDENTIFIER 	= NULL,
	@guidCategoryID 	UNIQUEIDENTIFIER 	= NULL,
	@chvnCategoryName 	NVARCHAR(100) 		= NULL,
	@chvnDomain 		NVARCHAR(100) 		= NULL,
	@bitIncludeHidden 	BIT 				= NULL,
	@dteDateFilterFrom	DATETIME			= NULL,
	@dteDateFilterTo	DATETIME			= NULL
AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE	@intCount	INT

	IF @guidPostID IS NOT NULL
		SELECT @intCount =
		(SELECT COUNT(*)
		FROM dbo.FORUM_Post
		WHERE ParentID = @guidPostID AND
			(@dteDateFilterFrom IS NULL OR TimePosted >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR TimePosted <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR Visible = '1'))
		+
		(SELECT COUNT(*)
		FROM dbo.FORUM_PostRelation relation
			JOIN dbo.FORUM_Post child ON relation.AncestorPostID = child.ID
			JOIN dbo.FORUM_Post childdescendant ON relation.DescendantPostID = childdescendant.ID
		WHERE child.ParentID = @guidPostID AND
			(@dteDateFilterFrom IS NULL OR childdescendant.TimePosted >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR childdescendant.TimePosted <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1'))
	ELSE
		SELECT @intCount =
		(SELECT COUNT(*)
		FROM dbo.FORUM_Post root
			JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
		WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
			(@dteDateFilterFrom IS NULL OR root.TimePosted >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR root.TimePosted <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR
				(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1') OR
				((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1')))
		+
		(SELECT COUNT(*)
		FROM dbo.FORUM_Post root
			JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
			JOIN dbo.FORUM_PostRelation relation ON root.ID = relation.AncestorPostID
			JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
		WHERE c.Domain = ISNULL(@chvnDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
			(@dteDateFilterFrom IS NULL OR rootdescendant.TimePosted >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR rootdescendant.TimePosted <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR
				(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1' AND relation.DescendantVisible = '1') OR
				((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1' AND relation.DescendantVisible = '1')))

	RETURN @intCount
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_GetPostAttributes
Description:	Retrieves the post attributes
Input:
Revision history:
	2005-04-18	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_GetPostAttributes
AS
	SET NOCOUNT ON

	SELECT pat.Name, adt.TypeName, adt.AssemblyName, adt.AssemblyPath
	FROM dbo.FORUM_PostAttributeType pat
		JOIN dbo.FORUM_AttributeDataType adt ON pat.AttributeDataTypeID = adt.ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_MovePosts
Description:	Moves posts
Input:			@guidFromCategoryID
				@chvnFromDomain
				@chvnFromCategoryName
				@guidToCategoryID
				@chvnToDomain
				@chvnToCategoryName
				@dteUntil
Revision history:
	2006-12-14	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_MovePosts
	@guidFromCategoryID 	UNIQUEIDENTIFIER 	= NULL,
	@chvnFromDomain 		NVARCHAR(100) 		= NULL,
	@chvnFromCategoryName 	NVARCHAR(100) 		= NULL,
	@guidToCategoryID 		UNIQUEIDENTIFIER 	= NULL,
	@chvnToDomain 			NVARCHAR(100) 		= NULL,
	@chvnToCategoryName 	NVARCHAR(100) 		= NULL,
	@dteUntil				DATETIME			= NULL
AS
	SET NOCOUNT ON

	UPDATE p
	SET 	p.CategoryID 	= toc.ID
	FROM dbo.FORUM_Post p
		JOIN dbo.FORUM_PostDerivedInformation pdi ON pdi.ID = p.ID
		JOIN dbo.FORUM_Category fromc ON p.CategoryID = fromc.ID,
		dbo.FORUM_Category toc
	WHERE fromc.Domain = ISNULL(@chvnFromDomain, fromc.Domain) AND fromc.Name = ISNULL(@chvnFromCategoryName, fromc.Name) AND fromc.ID = ISNULL(@guidFromCategoryID, fromc.ID) AND
		pdi.TimeLastPostAll <= ISNULL(@dteUntil, pdi.TimeLastPostAll) AND
		toc.Domain = ISNULL(@chvnToDomain, toc.Domain) AND toc.Name = ISNULL(@chvnToCategoryName, toc.Name) AND toc.ID = ISNULL(@guidToCategoryID, toc.ID)

	RETURN @@ROWCOUNT
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_SaveCategory
Description:	Saves a category
Input:			@guidID
				@chvnName
				@bitVisible
				@chvnDomain
				@tsTimestamp
Return value:	1 - If the category name is not unique within the domain
				2 - If the category was to be inserted and a domain has not been specified
				3 - If the category was to be updated but has been deleted
				4 - If the category has been concurrently updated
Revision history:
	2005-04-11	Kai de Leeuw 	Created
	2006-08-02	Kai de Leeuw	Added return codes
*/
CREATE PROC dbo.FORUM_SaveCategory
	@guidID 		UNIQUEIDENTIFIER,
	@chvnName 		NVARCHAR(100),
	@bitVisible 	BIT,
	@chvnDomain 	NVARCHAR(100)		= NULL,
	@tsTimestamp	TIMESTAMP			= NULL
AS
	SET NOCOUNT ON

	IF EXISTS (
		SELECT *
		FROM dbo.FORUM_Category
		WHERE Name = @chvnName AND Domain = ISNULL(@chvnDomain,
			(SELECT Domain
			FROM dbo.FORUM_Category
			WHERE ID = @guidID))
			AND ID <> @guidID)
		RETURN 1

	IF @tsTimestamp IS NOT NULL
	BEGIN
		DECLARE @tsCurrentTimestamp TIMESTAMP

		SELECT	@tsCurrentTimestamp	= Timestamp
		FROM dbo.FORUM_Category
		WHERE ID = @guidID

		IF @@ROWCOUNT = 0
			RETURN 3
		ELSE IF @tsCurrentTimestamp <> @tsTimestamp
			RETURN 4
		ELSE
			UPDATE dbo.FORUM_Category
			SET 	Name 	= @chvnName,
					Visible = @bitVisible,
					Domain	= ISNULL(@chvnDomain, Domain)
			WHERE ID = @guidID
	END
	ELSE
		IF @chvnDomain IS NULL
			RETURN 2
		ELSE
			INSERT dbo.FORUM_Category(ID, Name, Visible, Domain)
			VALUES(@guidID, @chvnName, @bitVisible, @chvnDomain)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_SaveCategoryAttribute
Description:	Saves a category attribute
Input:			@chvAttributeName
				@guidCategoryID
				@chvnValue
				@txtnExtendedValue
Revision history:
	2005-04-11	Kai de Leeuw 	Created
	2006-02-21	Kai de Leeuw	Changed the parameter to be an attribute name instead of an id
*/
CREATE PROC dbo.FORUM_SaveCategoryAttribute
	@chvAttributeName 	VARCHAR(100),
	@guidCategoryID 	UNIQUEIDENTIFIER,
	@chvnValue 			NVARCHAR(3500),
	@txtnExtendedValue 	NTEXT
AS
	SET NOCOUNT ON

	UPDATE ca
	SET 	Value 			= @chvnValue,
			ExtendedValue 	= @txtnExtendedValue
	FROM dbo.FORUM_CategoryAttribute ca
		JOIN dbo.FORUM_CategoryAttributeType cat ON ca.CategoryAttributeTypeID = cat.ID
	WHERE cat.Name = @chvAttributeName AND ca.CategoryID = @guidCategoryID

	IF @@ROWCOUNT = 0
		INSERT dbo.FORUM_CategoryAttribute(CategoryAttributeTypeID, Value, ExtendedValue, CategoryID)
		SELECT ID, @chvnValue, @txtnExtendedValue, @guidCategoryID
		FROM dbo.FORUM_CategoryAttributeType
		WHERE Name = @chvAttributeName
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_SavePost
Description:	Saves a post
Input:			@guidID
				@chvUserIdentity
				@chvnAuthor
				@chvnTitle
				@txtnBody
				@bitVisible
				@guidParentID
				@guidCategoryID
				@chvnDomain
				@chvnCategoryName
				@tsTimestamp
Return value:	1 - If the specified category does not exist
				2 - If the specified parent does not exist
				3 - If the post to be updated is a child and a category has been specified (cannot promote a child to become a root)
				4 - If the post was to be updated and a parent is specified (cannot move to another post)
				5 - If the post was to be inserted and neither a category nor a parent has been specified
				6 - If the post was to be updated but has been deleted
				7 - If the post has been concurrently updated
Revision history:
	2005-04-11	Kai de Leeuw 	Created
	2005-04-13	Kai de Leeuw 	Made it possible to move root posts between categories
	2006-03-03	Kai de Leeuw	Added the possibility to specify a domain and category name
	2006-08-02	Kai de Leeuw	Added return codes
*/
CREATE PROC dbo.FORUM_SavePost
	@guidID 			UNIQUEIDENTIFIER,
	@chvUserIdentity 	VARCHAR(255),
	@chvnAuthor 		NVARCHAR(255),
	@chvnTitle 			NVARCHAR(255),
	@txtnBody 			NTEXT,
	@bitVisible 		BIT,
	@guidParentID 		UNIQUEIDENTIFIER 	= NULL,
	@guidCategoryID 	UNIQUEIDENTIFIER 	= NULL,
	@chvnDomain 		NVARCHAR(100) 		= NULL,
	@chvnCategoryName 	NVARCHAR(100) 		= NULL,
	@tsTimestamp		TIMESTAMP			= NULL
AS
	SET NOCOUNT ON

	DECLARE	@guidActualCategoryID	UNIQUEIDENTIFIER,
			@guidActualParentID		UNIQUEIDENTIFIER

	SELECT 	@guidActualCategoryID	= CASE WHEN @guidCategoryID IS NOT NULL OR (@chvnDomain IS NOT NULL AND @chvnCategoryName IS NOT NULL) THEN
										(SELECT ID
										FROM dbo.FORUM_Category
										WHERE ID = @guidCategoryID OR (Domain = @chvnDomain AND Name = @chvnCategoryName)) 
									END,
			@guidActualParentID 	= CASE WHEN @guidParentID IS NOT NULL THEN
										(SELECT ID
										FROM dbo.FORUM_Post
										WHERE ID = @guidParentID)
									END

	IF (@guidCategoryID IS NOT NULL OR (@chvnDomain IS NOT NULL AND @chvnCategoryName IS NOT NULL)) AND @guidActualCategoryID IS NULL
		RETURN 1
	ELSE IF @guidParentID IS NOT NULL AND @guidActualParentID IS NULL
		RETURN 2
	ELSE IF @tsTimestamp IS NOT NULL
	BEGIN
		DECLARE	@guidCurrentParentID	UNIQUEIDENTIFIER,
				@tsCurrentTimestamp		TIMESTAMP

		SELECT	@guidCurrentParentID	= ParentID,
				@tsCurrentTimestamp		= Timestamp
		FROM dbo.FORUM_Post
		WHERE ID = @guidID

		IF @@ROWCOUNT = 0
			RETURN 6
		ELSE IF @tsTimestamp <> @tsCurrentTimestamp
			RETURN 7 
		ELSE IF @guidCurrentParentID IS NOT NULL AND @guidActualCategoryID IS NOT NULL
			RETURN 3
		ELSE IF @guidActualParentID IS NOT NULL
			RETURN 4
		ELSE
			UPDATE dbo.FORUM_Post
			SET 	UserIdentity 	= @chvUserIdentity,
					Author 			= @chvnAuthor,
					Title 			= @chvnTitle,
					Body 			= @txtnBody,
					Visible 		= @bitVisible,
					CategoryID 		= ISNULL(@guidActualCategoryID, CategoryID)
			WHERE ID = @guidID
	END
	ELSE IF @guidActualCategoryID IS NULL AND @guidActualParentID IS NULL
		RETURN 5
	ELSE
		INSERT dbo.FORUM_Post(ID, ParentID, CategoryID, UserIdentity, Author, Title, Body, Visible)
		VALUES(@guidID, @guidActualParentID, @guidActualCategoryID, @chvUserIdentity, @chvnAuthor, @chvnTitle, @txtnBody, @bitVisible)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Name:			dbo.FORUM_SavePostAttribute
Description:	Saves a post attribute
Input:			@chvAttributeName
				@guidPostID
				@chvnValue
				@txtnExtendedValue
Revision history:
	2005-04-11	Kai de Leeuw 	Created
	2006-02-21	Kai de Leeuw	Changed the parameter to be an attribute name instead of an id
*/
CREATE PROC dbo.FORUM_SavePostAttribute
	@chvAttributeName 	VARCHAR(100),
	@guidPostID 		UNIQUEIDENTIFIER,
	@chvnValue 			NVARCHAR(3500),
	@txtnExtendedValue 	NTEXT
AS
	SET NOCOUNT ON

	UPDATE pa
	SET 	Value 			= @chvnValue,
			ExtendedValue 	= @txtnExtendedValue
	FROM dbo.FORUM_PostAttribute pa
		JOIN dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID
	WHERE pat.Name = @chvAttributeName AND pa.PostID = @guidPostID

	IF @@ROWCOUNT = 0
		INSERT dbo.FORUM_PostAttribute(PostAttributeTypeID, Value, ExtendedValue, PostID)
		SELECT ID, @chvnValue, @txtnExtendedValue, @guidPostID
		FROM dbo.FORUM_PostAttributeType
		WHERE Name = @chvAttributeName
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO