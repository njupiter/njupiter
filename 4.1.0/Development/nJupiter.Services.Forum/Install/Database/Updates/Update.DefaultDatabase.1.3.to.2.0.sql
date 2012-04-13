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
Name:		dbo.FORUM_DeleteCategories
Description:	Deletes categories
Input:		@guidID
		@chvDomain
		@chvnName
Revision history:
	2005-04-13	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_DeleteCategories
	@guidID	UNIQUEIDENTIFIER	= NULL,
	@chvDomain 	VARCHAR(100) 		= NULL,
	@chvnName 	NVARCHAR(100) 	= NULL
AS
	SET NOCOUNT ON

	EXEC dbo.FORUM_DeletePosts
		@guidCategoryID	= @guidID,
		@chvDomain		= @chvDomain,
		@chvnCategoryName	= @chvnName

	DELETE dbo.FORUM_Category
	WHERE ID = ISNULL(@guidID, ID) AND Domain = ISNULL(@chvDomain, Domain) AND Name = ISNULL(@chvnName, Name)

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
Name:		dbo.FORUM_DeleteCategoryAttribute
Description:	Deletes a category attribute
Input:		@chvAttributeName
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
Name:		dbo.FORUM_DeletePostAttribute
Description:	Deletes a post attribute
Input:		@chvAttributeName
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
Name:		dbo.FORUM_DeletePosts
Description:	Deletes posts
Input:		@guidID
		@bitDeleteOnlyChildren
		@guidCategoryID
		@chvDomain
		@chvnCategoryName
		@dteUntil
Revision history:
	2005-04-13	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_DeletePosts
	@guidID		UNIQUEIDENTIFIER	= NULL,
	@bitDeleteOnlyChildren	BIT			= NULL,
	@guidCategoryID 	UNIQUEIDENTIFIER 	= NULL,
	@chvDomain 		VARCHAR(100) 		= NULL,
	@chvnCategoryName 	NVARCHAR(100) 	= NULL,
	@dteUntil		DATETIME		= NULL
AS
	SET NOCOUNT ON

	DECLARE	@intNumberOfRows		INT,
			@intTotalNumberOfRows	INT
	DECLARE 	@tblAncestorPost	TABLE (
		ID 	UNIQUEIDENTIFIER PRIMARY KEY)

	IF @guidID IS NULL
		INSERT @tblAncestorPost(ID)
		SELECT ancestor.ID
		FROM dbo.FORUM_Post ancestor
			JOIN dbo.FORUM_PostDerivedInformation pdi ON ancestor.ID = pdi.ID
			JOIN dbo.FORUM_Category c ON ancestor.CategoryID = c.ID
		WHERE c.ID = ISNULL(@guidCategoryID, c.ID) AND c.Domain = ISNULL(@chvDomain, c.Domain) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
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
Name:		dbo.FORUM_FilterCategories
Description:	Filters categories
Input:		@chvDomain
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
		@bitPostLimitSortDirectionAsc
		@intPostLimitPage
		@bitPostGetAttributes
Revision history:
	2005-04-11	Kai de Leeuw 	Created
*/
--TODO
--also search?
--table variables?
CREATE PROC dbo.FORUM_FilterCategories
	@chvDomain 			VARCHAR(100) 		= NULL,
	@guidID 			UNIQUEIDENTIFIER 	= NULL,
	@guidPostID			UNIQUEIDENTIFIER	= NULL,
	@chvnName 			NVARCHAR(100) 	= NULL,
	@bitIncludeHidden 		BIT 			= NULL,
	@bitGetAttributes 		BIT 			= NULL,
	@bitPostIncludeHidden		BIT			= NULL,
	@dtePostDateFilterFrom		DATETIME		= NULL,
	@dtePostDateFilterTo		DATETIME		= NULL,
	@intPostDateFilterColumn	INT			= NULL,
	@intPostLevels 			INT 			= NULL,
	@intPostLimitSize 		INT 			= NULL,
	@intPostLimitSortColumn 	INT 			= NULL,
	@bitPostLimitSortDirectionAsc 	BIT 			= NULL,
	@intPostLimitPage 		INT 			= NULL,
	@bitPostGetAttributes 		BIT 			= NULL
AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE 	@tblResultCategory	TABLE (
		ID 		UNIQUEIDENTIFIER PRIMARY KEY,
		Name 		NVARCHAR(100),
		Domain		VARCHAR(100),
		Visible 		BIT,
		RootPostCount 	INT, 
		Row		INT IDENTITY UNIQUE)

	DECLARE 	@intLoopIndex	INT

	IF @guidPostID IS NOT NULL
		INSERT @tblResultCategory(ID, Name, Domain, Visible, RootPostCount)
		SELECT c.ID, c.Name, c.Domain, c.Visible,
		 	(SELECT COUNT(*)
		 	FROM dbo.FORUM_Post
		 	WHERE CategoryID = c.ID AND (@bitPostIncludeHidden <> '0' OR Visible = '1'))
		FROM dbo.FORUM_Category c
			JOIN dbo.FORUM_Post p ON c.ID = p.CategoryID
		WHERE p.ID = @guidPostID OR EXISTS (
			SELECT *
			FROM dbo.FORUM_PostRelation
			WHERE AncestorPostID = p.ID AND DescendantPostID = @guidPostID)
	ELSE
		INSERT @tblResultCategory(ID, Name, Domain, Visible, RootPostCount)
		SELECT c.ID, c.Name, c.Domain, c.Visible,
			(SELECT COUNT(*)
			FROM dbo.FORUM_Post 
			WHERE CategoryID = c.ID AND (@bitPostIncludeHidden <> '0' OR Visible = '1'))
		FROM dbo.FORUM_Category c
		WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND c.ID = ISNULL(@guidID, c.ID) AND c.Name = ISNULL(@chvnName, c.Name) AND
			(@bitIncludeHidden <> '0' OR @guidID IS NOT NULL OR @chvnName IS NOT NULL OR c.Visible = '1')

	SELECT	@intLoopIndex	= @@ROWCOUNT

	SELECT ID, Name, Domain, Visible, RootPostCount
	FROM @tblResultCategory

	IF @bitGetAttributes <> '0'
		SELECT cat.Name, c.ID, ca.Value, ca.ExtendedValue
		FROM @tblResultCategory c
			JOIN dbo.FORUM_CategoryAttribute ca ON c.ID = ca.CategoryID
			JOIN dbo.FORUM_CategoryAttributeType cat ON ca.CategoryAttributeTypeID = cat.ID
	
	IF @intPostLevels IS NULL OR @intPostLevels > 0
	BEGIN
		CREATE TABLE dbo.#CategoryResultPost (
			ID 		UNIQUEIDENTIFIER PRIMARY KEY,
			EffectivelyVisible	BIT,
			Rank 		SMALLINT)
		DECLARE 	@tblPagingTotalCount	TABLE (
			ID 				UNIQUEIDENTIFIER,
			PagingTotalNumberOfPosts 	INT)

		WHILE @intLoopIndex > 0
		BEGIN
			DECLARE 	@intPagingTotalNumberOfPosts 	INT,
					@guidLoopID			UNIQUEIDENTIFIER

			SELECT 	@guidLoopID 	= ID,
					@intLoopIndex 	= @intLoopIndex - 1
			FROM @tblResultCategory
			WHERE Row = @intLoopIndex

			INSERT dbo.#CategoryResultPost(ID, Rank, EffectivelyVisible)
			EXEC @intPagingTotalNumberOfPosts = dbo.FORUM_FilterPosts
				@guidCategoryID 		= @guidLoopID,
				@intLevels 			= @intPostLevels,
				@bitIncludeHidden 		= @bitPostIncludeHidden,
				@dteDateFilterFrom		= @dtePostDateFilterFrom,
				@dteDateFilterTo		= @dtePostDateFilterTo,
				@intDateFilterColumn		= @intPostDateFilterColumn,
				@intLimitSize 			= @intPostLimitSize,
				@intLimitSortColumn 		= @intPostLimitSortColumn,
				@bitLimitSortDirectionAsc	= @bitPostLimitSortDirectionAsc,
				@intLimitPage 			= @intPostLimitPage,
				@bitGetOnlyMetaData 		= '1'

			INSERT @tblPagingTotalCount(ID, PagingTotalNumberOfPosts)
			VALUES(@guidLoopID, @intPagingTotalNumberOfPosts)
		END

		SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible,
			CASE WHEN @bitPostIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END PostCount,
			CASE WHEN @bitPostIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END TimeLastPost, p3.Rank, p3.EffectivelyVisible
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			JOIN dbo.#CategoryResultPost p3 ON p.ID = p3.ID
		ORDER BY p.TimePosted

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
Name:		dbo.FORUM_FilterPosts
Description:	Filters posts
Input:		@guidID
		@guidImmediateDescendantID
		@guidRootDescendantID
		@bitGetOnlyChildren
		@guidCategoryID
		@chvnCategoryName
		@chvDomain
		@bitIncludeHidden
		@dteDateFilterFrom
		@dteDateFilterTo
		@intDateFilterColumn
		@intLevels
		@chvnSearchText
		@intLimitSize
		@intLimitSortColumn
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
	@guidID 			UNIQUEIDENTIFIER 	= NULL,
	@guidImmediateDescendantID 	UNIQUEIDENTIFIER 	= NULL,
	@guidRootDescendantID 	UNIQUEIDENTIFIER 	= NULL,
	@bitGetOnlyChildren 		BIT 			= NULL,
	@guidCategoryID 		UNIQUEIDENTIFIER 	= NULL,
	@chvnCategoryName 		NVARCHAR(100) 	= NULL,
	@chvDomain 			VARCHAR(100) 		= NULL,
	@bitIncludeHidden 		BIT 			= NULL,
	@dteDateFilterFrom		DATETIME		= NULL,
	@dteDateFilterTo		DATETIME		= NULL,
	@intDateFilterColumn		INT			= NULL,
	@intLevels 			INT 			= NULL,
	@chvnSearchText 		NVARCHAR(4000) 	= NULL,
	@chvUserIdentity		VARCHAR(255)		= NULL,
	@intLimitSize 			INT 			= NULL,
	@intLimitSortColumn 		INT 			= NULL,
	@bitLimitSortDirectionAsc 	BIT 			= NULL,
	@intLimitPage 			INT 			= NULL,
	@bitGetOnlyMetaData 		BIT 			= NULL,
	@bitGetAttributes		BIT			= NULL,
	@txtnPostsSearchQuery		NTEXT			= NULL
AS
	SET NOCOUNT ON
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE	@tblUnFinishedBranch	TABLE (
		ID 		UNIQUEIDENTIFIER PRIMARY KEY,
		EffectivelyVisible	BIT,
		Rank 		SMALLINT)

	DECLARE 	@tblResultPost		TABLE (
		ID 		UNIQUEIDENTIFIER,
		Rank 		SMALLINT,
		EffectivelyVisible	BIT,
		Row 		INT IDENTITY PRIMARY KEY)

	DECLARE 	@intStartRow 			INT,
			@intWhereToStart 		INT,
			@intHowMany 			INT,
			@intPagingTotalNumberOfPosts 	INT,
			@intOldLevels 			INT,
			@bitFetchBranches 		BIT,
			@guidSingleID 			UNIQUEIDENTIFIER,
			@bitInsertSingleID		BIT

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
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
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
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
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
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT @guidSingleID, 0, '1'
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
			SELECT relation.DescendantPostID, 0, relation.DescendantVisible
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
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT p.ID, 0, p.Visible
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
			SELECT relation.DescendantPostID, 0, relation.DescendantVisible
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
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT parentchild.ID, 0, parentchild.Visible
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
			SELECT relation.DescendantPostID, 0, relation.DescendantVisible
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
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT rootchild.ID, 0, rootchild.Visible
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
			SELECT relation.DescendantPostID, 0, relation.DescendantVisible
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
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT root.ID, 0, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible ELSE root.Visible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_PostDerivedInformation pdi ON root.ID = pdi.ID
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
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
			SELECT relation.DescendantPostID, 0, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible & relation.DescendantVisible ELSE root.Visible & relation.DescendantVisible END
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN dbo.FORUM_PostRelation relation ON root.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN dbo.FORUM_PostDerivedInformation pdi ON rootdescendant.ID = pdi.ID
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
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
	ELSE IF @guidSingleID IS NOT NULL
	BEGIN
		SELECT @bitInsertSingleID	= '1'

		IF @intLevels <> 1
		BEGIN
			IF @intLevels IS NOT NULL
				SELECT @intLevels	= @intLevels - 1

			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT p.ID, 0, p.Visible
			FROM dbo.FORUM_Post p
				JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			WHERE p.ParentID = @guidSingleID AND
				(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
				(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo) AND
				(@bitIncludeHidden <> '0' OR p.Visible = '1')
		END
	END
	ELSE IF @guidID IS NOT NULL
		INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
		SELECT p.ID, 0, p.Visible
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
		WHERE p.ParentID = @guidID AND
			(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE p.TimePosted END <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR p.Visible = '1')
	ELSE IF @guidImmediateDescendantID IS NOT NULL
		INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
		SELECT parentchild.ID, 0, parentchild.Visible
		FROM dbo.FORUM_Post child
			JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
			JOIN dbo.FORUM_PostDerivedInformation pdi ON parentchild.ID = pdi.ID
		WHERE child.ID = @guidImmediateDescendantID AND
			(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE parentchild.TimePosted END <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
	ELSE IF @guidRootDescendantID IS NOT NULL
		INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
		SELECT rootchild.ID, 0, rootchild.Visible
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
		INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
		SELECT root.ID, 0, CASE WHEN @guidCategoryID IS NULL AND @chvnCategoryName IS NULL THEN c.Visible & root.Visible ELSE root.Visible END
		FROM dbo.FORUM_Post root
			JOIN dbo.FORUM_PostDerivedInformation pdi ON root.ID = pdi.ID
			JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
		WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
			(@dteDateFilterFrom IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END >= @dteDateFilterFrom) AND
			(@dteDateFilterTo IS NULL OR CASE WHEN @intDateFilterColumn = 1 THEN CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END ELSE root.TimePosted END <= @dteDateFilterTo) AND
			(@bitIncludeHidden <> '0' OR
				(@guidCategoryID IS NULL AND @chvnCategoryName IS NULL AND c.Visible = '1' AND root.Visible = '1') OR
				((@guidCategoryID IS NOT NULL OR @chvnCategoryName IS NOT NULL) AND root.Visible = '1'))

	SELECT	@intPagingTotalNumberOfPosts 	= @@ROWCOUNT

	IF NOT(@txtnPostsSearchQuery IS NULL AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL)
		DROP TABLE dbo.#SearchedPost

	IF @intLimitSize IS NOT NULL AND @intLevels <> 1 AND @txtnPostsSearchQuery IS NULL AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL
		SELECT	@intOldLevels 		= @intLevels,
				@intLevels 		= 1,
				@bitFetchBranches 	= '1'

	WHILE 1 = 1
	BEGIN
		IF @intLevels <> 1 AND @txtnPostsSearchQuery IS NULL AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL
			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT descendantpdi.ID, 0, ancestor.EffectivelyVisible & relation.DescendantVisible
			FROM @tblUnFinishedBranch ancestor
				JOIN dbo.FORUM_PostDerivedInformation ancestorpdi ON ancestor.ID = ancestorpdi.ID
				JOIN dbo.FORUM_PostRelation relation ON ancestorpdi.ID = relation.AncestorPostID
				JOIN dbo.FORUM_PostDerivedInformation descendantpdi ON relation.DescendantPostID = descendantpdi.ID
			WHERE (@intLevels IS NULL OR @intLevels > descendantpdi.Level - ancestorpdi.Level) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')

		SELECT	@intHowMany 	= ISNULL(@intLimitSize, 0) * ISNULL(@intLimitPage, 1)
		SET ROWCOUNT @intHowMany

		IF @intHowMany = 0 OR @intLimitSortColumn NOT BETWEEN 0 AND 12
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
				ORDER BY p.ParentID DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.ParentID
		ELSE IF @intLimitSortColumn = 2
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.CategoryID DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.CategoryID
		ELSE IF @intLimitSortColumn = 3
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.UserIdentity DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.UserIdentity
		ELSE IF @intLimitSortColumn = 4
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Author DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Author
		ELSE IF @intLimitSortColumn = 5
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Title DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Title
		ELSE IF @intLimitSortColumn = 6
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY CAST(p.Body AS NVARCHAR(4000)) DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY CAST(p.Body AS NVARCHAR(4000))
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
				ORDER BY p.Visible DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT match.ID, match.Rank, match.EffectivelyVisible
				FROM @tblUnFinishedBranch match
					JOIN dbo.FORUM_Post p ON match.ID = p.ID
				ORDER BY p.Visible
		ELSE IF @intLimitSortColumn = 9
			IF @bitLimitSortDirectionAsc = '0'
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.PostCountAll DESC
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.PostCountVisible DESC
			ELSE
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.PostCountAll
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.PostCountVisible
		ELSE IF @intLimitSortColumn = 10
			IF @bitLimitSortDirectionAsc = '0'
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.TimeLastPostAll DESC
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.TimeLastPostVisible DESC
			ELSE
				IF @bitIncludeHidden <> '0' 
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.TimeLastPostAll
				ELSE
					INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
					SELECT match.ID, match.Rank, match.EffectivelyVisible
					FROM @tblUnFinishedBranch match
						JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID
					ORDER BY pdi.TimeLastPostVisible
		ELSE IF @intLimitSortColumn = 11
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM @tblUnFinishedBranch
				ORDER BY Rank DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM @tblUnFinishedBranch
				ORDER BY Rank
		ELSE IF @intLimitSortColumn = 12
			IF @bitLimitSortDirectionAsc = '0'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM @tblUnFinishedBranch
				ORDER BY EffectivelyVisible DESC
			ELSE
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				SELECT ID, Rank, EffectivelyVisible
				FROM @tblUnFinishedBranch
				ORDER BY EffectivelyVisible

		SELECT 	@intStartRow 		= ISNULL(SCOPE_IDENTITY(), 0) - @@ROWCOUNT + 1,
				@intWhereToStart	= ISNULL((@intLimitPage - 1) * @intLimitSize + @intStartRow, @intStartRow)

		SET ROWCOUNT 0

		IF @bitFetchBranches = '1'
		BEGIN
			DELETE @tblUnFinishedBranch

			INSERT @tblUnFinishedBranch(ID, Rank, EffectivelyVisible)
			SELECT ID, 0, EffectivelyVisible
			FROM @tblResultPost
			WHERE Row >= @intWhereToStart

			SELECT	@intLevels 		= @intOldLevels,
					@intLimitSize 		= NULL,
					@bitFetchBranches	= '0'
		END
		ELSE
		BEGIN
			IF @bitInsertSingleID = '1'
				INSERT @tblResultPost(ID, Rank, EffectivelyVisible)
				VALUES(@guidSingleID, 0, '1')

			IF @bitGetOnlyMetaData = '1'
				SELECT ID, Rank, EffectivelyVisible
				FROM @tblResultPost
				WHERE Row >= @intWhereToStart
			ELSE
			BEGIN
				SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END PostCount,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END TimeLastPost, rp.Rank, rp.EffectivelyVisible
				FROM @tblResultPost rp
					JOIN dbo.FORUM_Post p ON rp.ID = p.ID
					JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
				WHERE rp.Row >= @intWhereToStart
				ORDER BY p.TimePosted

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
Name:		dbo.FORUM_GetCategoryAttributes
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
Name:		dbo.FORUM_GetPostAttributes
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
Name:		dbo.FORUM_MovePosts
Description:	Moves posts
Input:		@guidFromCategoryID
		@chvFromDomain
		@chvnFromCategoryName
		@guidToCategoryID
		@chvToDomain
		@chvnToCategoryName
		@dteUntil
Revision history:
	2006-12-14	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_MovePosts
	@guidFromCategoryID 		UNIQUEIDENTIFIER 	= NULL,
	@chvFromDomain 		VARCHAR(100) 		= NULL,
	@chvnFromCategoryName 	NVARCHAR(100) 	= NULL,
	@guidToCategoryID 		UNIQUEIDENTIFIER 	= NULL,
	@chvToDomain 		VARCHAR(100) 		= NULL,
	@chvnToCategoryName 	NVARCHAR(100) 	= NULL,
	@dteUntil			DATETIME		= NULL
AS
	SET NOCOUNT ON

	UPDATE p
	SET 	p.CategoryID 	= toc.ID
	FROM dbo.FORUM_Post p
		JOIN dbo.FORUM_PostDerivedInformation pdi ON pdi.ID = p.ID
		JOIN dbo.FORUM_Category fromc ON p.CategoryID = fromc.ID,
		dbo.FORUM_Category toc
	WHERE fromc.Domain = ISNULL(@chvFromDomain, fromc.Domain) AND fromc.Name = ISNULL(@chvnFromCategoryName, fromc.Name) AND fromc.ID = ISNULL(@guidFromCategoryID, fromc.ID) AND
		pdi.TimeLastPostAll <= ISNULL(@dteUntil, pdi.TimeLastPostAll) AND
		toc.Domain = ISNULL(@chvToDomain, toc.Domain) AND toc.Name = ISNULL(@chvnToCategoryName, toc.Name) AND toc.ID = ISNULL(@guidToCategoryID, toc.ID)

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
Name:		dbo.FORUM_SaveCategory
Description:	Saves a category
Input:		@guidID
		@chvnName
		@bitVisible
		@chvDomain
Return value:	1 - If the category name is not unique within the domain
		2 - If the category does not exist and a domain has not been specified
Revision history:
	2005-04-11	Kai de Leeuw 	Created
	2006-08-02	Kai de Leeuw	Added return codes
*/
CREATE PROC dbo.FORUM_SaveCategory
	@guidID 	UNIQUEIDENTIFIER,
	@chvnName 	NVARCHAR(100),
	@bitVisible 	BIT,
	@chvDomain 	VARCHAR(100)		= NULL
AS
	SET NOCOUNT ON

	IF EXISTS (
		SELECT *
		FROM dbo.FORUM_Category
		WHERE Name = @chvnName AND Domain = ISNULL(@chvDomain,
			(SELECT Domain
			FROM dbo.FORUM_Category
			WHERE ID = @guidID))
			AND ID <> @guidID)
		RETURN 1

	UPDATE dbo.FORUM_Category
	SET 	Name 	= @chvnName,
		Visible 	= @bitVisible,
		Domain	= ISNULL(@chvDomain, Domain)
	WHERE ID = @guidID

	IF @@ROWCOUNT = 0
		IF @chvDomain IS NULL
			RETURN 2
		ELSE
			INSERT dbo.FORUM_Category(ID, Name, Visible, Domain)
			VALUES(@guidID, @chvnName, @bitVisible, @chvDomain)
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
Name:		dbo.FORUM_SaveCategoryAttribute
Description:	Saves a category attribute
Input:		@chvAttributeName
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
	@chvnValue 		NVARCHAR(3500),
	@txtnExtendedValue 	NTEXT
AS
	SET NOCOUNT ON

	UPDATE ca
	SET 	Value 		= @chvnValue,
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
Name:		dbo.FORUM_SavePost
Description:	Saves a post
Input:		@guidID
		@chvUserIdentity
		@chvnAuthor
		@chvnTitle
		@txtnBody
		@bitVisible
		@guidParentID
		@guidCategoryID
		@chvDomain
		@chvnCategoryName
Return value:	1 - If the specified category does not exist
		2 - If the specified parent does not exist
		3 - If the post to be updated is a child and a category has been specified
		4 - If the post exists and a parent is specified
		5 - If the post does not exist and neither a category nor a parent has been specified
Revision history:
	2005-04-11	Kai de Leeuw 	Created
	2005-04-13	Kai de Leeuw 	Made it possible to move root posts between categories
	2006-03-03	Kai de Leeuw	Added the possibility to specify a domain and category name
	2006-08-02	Kai de Leeuw	Added return codes
*/
CREATE PROC dbo.FORUM_SavePost
	@guidID 		UNIQUEIDENTIFIER,
	@chvUserIdentity 	VARCHAR(255),
	@chvnAuthor 		NVARCHAR(255),
	@chvnTitle 		NVARCHAR(255),
	@txtnBody 		NTEXT,
	@bitVisible 		BIT,
	@guidParentID 		UNIQUEIDENTIFIER 	= NULL,
	@guidCategoryID 	UNIQUEIDENTIFIER 	= NULL,
	@chvDomain 		VARCHAR(100) 		= NULL,
	@chvnCategoryName 	NVARCHAR(100) 	= NULL
AS
	SET NOCOUNT ON

	DECLARE	@guidActualCategoryID	UNIQUEIDENTIFIER,
			@guidActualParentID	UNIQUEIDENTIFIER,
			@guidCurrentParentID	UNIQUEIDENTIFIER,
			@bitUpdate		BIT

	SELECT 	@guidActualCategoryID	= CASE WHEN @guidCategoryID IS NOT NULL OR (@chvDomain IS NOT NULL AND @chvnCategoryName IS NOT NULL) THEN
							(SELECT ID
							FROM dbo.FORUM_Category
							WHERE ID = @guidCategoryID OR (Domain = @chvDomain AND Name = @chvnCategoryName))
						ELSE
							NULL
						END,
			@guidActualParentID 	= CASE WHEN @guidParentID IS NULL THEN
							NULL
						ELSE
							(SELECT ID
							FROM dbo.FORUM_Post
							WHERE ID = @guidParentID)
						END

	SELECT	@bitUpdate		= CASE WHEN ID IS NULL THEN '0' ELSE '1' END,
			@guidCurrentParentID	= ParentID
	FROM dbo.FORUM_Post
	WHERE ID = @guidID

	IF (@guidCategoryID IS NOT NULL OR (@chvDomain IS NOT NULL AND @chvnCategoryName IS NOT NULL)) AND @guidActualCategoryID IS NULL
		RETURN 1
	ELSE IF @guidParentID IS NOT NULL AND @guidActualParentID IS NULL
		RETURN 2
	ELSE IF @bitUpdate = '1'
		IF @guidCurrentParentID IS NOT NULL AND @guidActualCategoryID IS NOT NULL
			RETURN 3
		ELSE IF @guidActualParentID IS NOT NULL
			RETURN 4
		ELSE
			UPDATE dbo.FORUM_Post
			SET 	UserIdentity 	= @chvUserIdentity,
				Author 		= @chvnAuthor,
				Title 		= @chvnTitle,
				Body 		= @txtnBody,
				Visible 		= @bitVisible,
				CategoryID 	= ISNULL(@guidActualCategoryID, CategoryID)
			WHERE ID = @guidID
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
Name:		dbo.FORUM_SavePostAttribute
Description:	Saves a post attribute
Input:		@chvAttributeName
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
	@chvnValue 		NVARCHAR(3500),
	@txtnExtendedValue 	NTEXT
AS
	SET NOCOUNT ON

	UPDATE pa
	SET 	Value 		= @chvnValue,
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerDelete]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerDelete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerInsert]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerInsert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerUpdate]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerUpdate]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE TRIGGER dbo.FORUM_Post_TriggerDelete
ON dbo.FORUM_Post
FOR DELETE
AS
	IF @@ROWCOUNT > 0
	BEGIN
		SET NOCOUNT ON

		-- update parent
		UPDATE pderived
		SET 	PostCountAll 		= ISNULL(derivedinfoall.PostCountAll, 0),
			TimeLastPostAll 		= ISNULL(derivedinfoall.TimeLastPostAll, parent.TimePosted),
			PostCountVisible 	= ISNULL(derivedinfovisible.PostCountVisible, 0),
			TimeLastPostVisible 	= ISNULL(derivedinfovisible.TimeLastPostVisible, parent.TimePosted)
		FROM deleted d
			JOIN dbo.FORUM_PostDerivedInformation pderived ON d.ParentID = pderived.ID
			JOIN dbo.FORUM_Post parent ON pderived.ID = parent.ID
			LEFT JOIN (
				SELECT COUNT(*) PostCountAll, MAX(parentdescendant.TimePosted) TimeLastPostAll, pderived.ID, d.ID DeletedID
				FROM deleted d
					JOIN dbo.FORUM_PostDerivedInformation pderived ON d.ParentID = pderived.ID
					JOIN dbo.FORUM_PostRelation parentdescendantrelation ON pderived.ID = parentdescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post parentdescendant ON parentdescendantrelation.DescendantPostID = parentdescendant.ID
				GROUP BY pderived.ID, d.ID) derivedinfoall ON pderived.ID = derivedinfoall.ID AND d.ID = derivedinfoall.DeletedID
			LEFT JOIN (
				SELECT COUNT(*) PostCountVisible, MAX(parentdescendant.TimePosted) TimeLastPostVisible, pderived.ID, d.ID DeletedID
				FROM deleted d
					JOIN dbo.FORUM_PostDerivedInformation pderived ON d.ParentID = pderived.ID
					JOIN dbo.FORUM_PostRelation parentdescendantrelation ON pderived.ID = parentdescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post parentdescendant ON parentdescendantrelation.DescendantPostID = parentdescendant.ID
				WHERE parentdescendantrelation.DescendantVisible = '1'
				GROUP BY pderived.ID, d.ID) derivedinfovisible ON pderived.ID = derivedinfovisible.ID AND d.ID = derivedinfovisible.DeletedID

		-- update parent's ancestors
		UPDATE pderived
		SET 	PostCountAll 		= ISNULL(derivedinfoall.PostCountAll, 0),
			TimeLastPostAll 		= ISNULL(derivedinfoall.TimeLastPostAll, ancestor.TimePosted),
			PostCountVisible 	= ISNULL(derivedinfovisible.PostCountVisible, 0),
			TimeLastPostVisible 	= ISNULL(derivedinfovisible.TimeLastPostVisible, ancestor.TimePosted)
		FROM deleted d
			JOIN dbo.FORUM_PostRelation relation ON d.ParentID = relation.DescendantPostID
			JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
			JOIN dbo.FORUM_Post ancestor ON pderived.ID = ancestor.ID
			LEFT JOIN (
				SELECT COUNT(*) PostCountAll, MAX(ancestordescendant.TimePosted) TimeLastPostAll, pderived.ID, d.ID DeletedID
				FROM deleted d
					JOIN dbo.FORUM_PostRelation relation ON d.ParentID = relation.DescendantPostID
					JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
					JOIN dbo.FORUM_PostRelation ancestordescendantrelation ON relation.AncestorPostID = ancestordescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post ancestordescendant ON ancestordescendantrelation.DescendantPostID = ancestordescendant.ID
				GROUP BY pderived.ID, d.ID) derivedinfoall ON pderived.ID = derivedinfoall.ID AND d.ID = derivedinfoall.DeletedID
			LEFT JOIN (
				SELECT COUNT(*) PostCountVisible, MAX(ancestordescendant.TimePosted) TimeLastPostVisible, pderived.ID, d.ID DeletedID
				FROM deleted d
					JOIN dbo.FORUM_PostRelation relation ON d.ParentID = relation.DescendantPostID
					JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
					JOIN dbo.FORUM_PostRelation ancestordescendantrelation ON relation.AncestorPostID = ancestordescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post ancestordescendant ON ancestordescendantrelation.DescendantPostID = ancestordescendant.ID
				WHERE ancestordescendantrelation.DescendantVisible = '1'
				GROUP BY pderived.ID, d.ID) derivedinfovisible ON pderived.ID = derivedinfovisible.ID AND d.ID = derivedinfovisible.DeletedID
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE TRIGGER dbo.FORUM_Post_TriggerInsert
ON dbo.FORUM_Post
FOR INSERT
AS
	IF @@ROWCOUNT > 0
	BEGIN
		SET NOCOUNT ON

		-- insert into fulltext index
		INSERT dbo.FORUM_PostFullText(ID, FullText)
		SELECT i.ID, post.Body
		FROM inserted i
			JOIN dbo.FORUM_Post post ON i.ID = post.ID

		-- insert author and title into fulltext index
		DECLARE curPost 	CURSOR FAST_FORWARD LOCAL FOR
			SELECT i.ID, ISNULL(i.Author + ' ', '') + i.Title + ' ', TEXTPTR(pft.FullText)
			FROM inserted i
				JOIN dbo.FORUM_PostFullText pft ON i.ID = pft.ID

		DECLARE 	@guidID			UNIQUEIDENTIFIER,
				@chvnAuthorTitleFullText 	NVARCHAR(512),
				@binTextPtr 			BINARY(16)

		OPEN curPost
		FETCH curPost INTO @guidID, @chvnAuthorTitleFullText, @binTextPtr
		WHILE @@FETCH_STATUS = 0
		BEGIN
			UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtr 0 0 @chvnAuthorTitleFullText

			-- ensure that full text index is conscious about the change
			UPDATE dbo.FORUM_PostFullText
			SET FullText = FullText
			WHERE ID = @guidID 

			FETCH curPost INTO @guidID, @chvnAuthorTitleFullText, @binTextPtr
		END
		CLOSE curPost
		DEALLOCATE curPost

		-- insert into derived information
		-- (make me one level higher than my parent, if I have one, else 0)
		INSERT dbo.FORUM_PostDerivedInformation(ID, PostCountAll, PostCountVisible, TimeLastPostAll, TimeLastPostVisible, Level)
		SELECT i.ID, 0, 0, i.TimePosted, i.TimePosted, ISNULL(pderived.Level + 1, 0)
		FROM inserted i
			LEFT JOIN dbo.FORUM_PostDerivedInformation pderived ON i.ParentID = pderived.ID

		-- insert into relations (my parent and me)
		-- (make me visible or not for my parent)
		INSERT dbo.FORUM_PostRelation(DescendantPostID, AncestorPostID, DescendantVisible)
		SELECT ID, ParentID, Visible
		FROM inserted
		WHERE ParentID IS NOT NULL

		-- insert into relations (my parent's ancestors and me)
		-- (make me invisible for my parent's ancestors if I or my parent is invisible,
		-- else make me visible for all ancestors that does not have invisible children in our line of ancestry,
		-- that is, between my parent and the ancestors)
		INSERT dbo.FORUM_PostRelation(DescendantPostID, AncestorPostID, DescendantVisible)
		SELECT i.ID, relation.AncestorPostID,
			CASE WHEN i.Visible = '0' OR parent.Visible = '0' THEN '0' ELSE CASE WHEN EXISTS (
				SELECT *
				FROM dbo.FORUM_PostRelation ancestordescendant
					JOIN dbo.FORUM_PostRelation parentancestor ON ancestordescendant.DescendantPostID = parentancestor.AncestorPostID
				WHERE ancestordescendant.AncestorPostID = relation.AncestorPostID AND
					parentancestor.DescendantPostID = i.ParentID AND
					ancestordescendant.DescendantVisible = '0') THEN '0' ELSE '1' END END
		FROM inserted i
			JOIN dbo.FORUM_PostRelation relation ON i.ParentID = relation.DescendantPostID
			JOIN dbo.FORUM_Post parent ON i.ParentID = parent.ID

		--update my ancestors with derived information
		UPDATE pderived
		SET 	TimeLastPostAll 		= i.TimePosted,
			PostCountAll 		= PostCountall + 1,
			TimeLastPostVisible 	= CASE WHEN i.Visible = '0' OR relation.DescendantVisible = '0' THEN TimeLastPostVisible ELSE i.TimePosted END,
			PostCountVisible 	= PostCountVisible + CASE WHEN i.Visible = '0' OR relation.DescendantVisible = '0' THEN 0 ELSE 1 END
		FROM inserted i
			JOIN dbo.FORUM_PostRelation relation ON i.ID = relation.DescendantPostID
			JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE TRIGGER dbo.FORUM_Post_TriggerUpdate
ON dbo.FORUM_Post
FOR UPDATE
AS
	IF @@ROWCOUNT > 0
	BEGIN
		SET NOCOUNT ON

		-- update full text index
		IF UPDATE(Author) OR UPDATE(Title) OR UPDATE(Body)
		BEGIN
			DECLARE curPost 	CURSOR FAST_FORWARD LOCAL FOR
				SELECT i.ID, ISNULL(i.Author + ' ', '') + i.Title + ' ', TEXTPTR(post.Body), TEXTPTR(pft.FullText)
				FROM inserted i
					JOIN dbo.FORUM_Post post ON i.ID = post.ID
					JOIN dbo.FORUM_PostFullText pft ON i.ID = pft.ID

			DECLARE 	@guidID			UNIQUEIDENTIFIER,
					@chvnAuthorTitleFullText	NVARCHAR(512),
					@binTextPtrSource		BINARY(16),
					@binTextPtrDestination		BINARY(16)

			OPEN curPost
			FETCH curPost INTO @guidID, @chvnAuthorTitleFullText, @binTextPtrSource, @binTextPtrDestination
			WHILE @@FETCH_STATUS = 0
			BEGIN
				UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtrDestination 0 NULL dbo.FORUM_Post.Body @binTextPtrSource
				UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtrDestination 0 0 @chvnAuthorTitleFullText

				-- ensure that full text index is conscious about the change
				UPDATE dbo.FORUM_PostFullText
				SET FullText = FullText
				WHERE ID = @guidID 

				FETCH curPost INTO @guidID, @chvnAuthorTitleFullText, @binTextPtrSource, @binTextPtrDestination
			END
			CLOSE curPost
			DEALLOCATE curPost
		END

		IF UPDATE(Visible)
		BEGIN
			-- update visibility for me to my ancestors
			-- (except the ones that has hidden children in our ancestry line)
			UPDATE relation
			SET 	DescendantVisible 	= i.Visible
			FROM inserted i
				JOIN dbo.FORUM_PostRelation relation ON i.ID = relation.DescendantPostID
			WHERE NOT EXISTS (
				SELECT *
				FROM dbo.FORUM_PostRelation ancestordescendant
					JOIN dbo.FORUM_PostRelation thisancestor ON ancestordescendant.DescendantPostID = thisancestor.AncestorPostID
				WHERE ancestordescendant.AncestorPostID = relation.AncestorPostID AND
					thisancestor.DescendantPostID = i.ID AND
					ancestordescendant.DescendantVisible = '0')

			-- update visibility for my descendants to my ancestors
			-- (except the ones that has hidden children in our ancestry line)
			UPDATE ancestordescendant
			SET	DescendantVisible 	= i.Visible
			FROM inserted i
				JOIN dbo.FORUM_PostRelation descendant ON i.ID = descendant.AncestorPostID
				JOIN dbo.FORUM_PostRelation ancestor ON i.ID = ancestor.DescendantPostID
				JOIN dbo.FORUM_PostRelation ancestordescendant ON ancestor.AncestorPostID = ancestordescendant.AncestorPostID AND
					descendant.DescendantPostID = ancestordescendant.DescendantPostID
			WHERE NOT EXISTS (
				SELECT *
				FROM dbo.FORUM_PostRelation ancestordescendant2
					JOIN dbo.FORUM_PostRelation thisancestor ON ancestordescendant2.DescendantPostID = thisancestor.AncestorPostID
				WHERE ancestordescendant2.AncestorPostID = ancestor.AncestorPostID AND
					thisancestor.DescendantPostID = i.ID AND
					ancestordescendant2.DescendantVisible = '0')

			-- update my ancestors with derived information
			UPDATE pderived
			SET 	PostCountVisible 	= ISNULL(derivedinfo.PostCountVisible, 0),
				TimeLastPostVisible 	= ISNULL(derivedinfo.TimeLastPostVisible, ancestor.TimePosted)
			FROM inserted i
				JOIN dbo.FORUM_PostRelation relation ON i.ID = relation.DescendantPostID
				JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
				JOIN dbo.FORUM_Post ancestor ON pderived.ID = ancestor.ID
				LEFT JOIN (
					SELECT COUNT(*) PostCountVisible, MAX(ancestordescendant.TimePosted) TimeLastPostVisible, pderived.ID, i.ID InsertedID
					FROM inserted i
						JOIN dbo.FORUM_PostRelation relation ON i.ID = relation.DescendantPostID
						JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
						JOIN dbo.FORUM_PostRelation ancestordescendantrelation ON relation.AncestorPostID = ancestordescendantrelation.AncestorPostID
						JOIN dbo.FORUM_Post ancestordescendant ON ancestordescendantrelation.DescendantPostID = ancestordescendant.ID
					WHERE ancestordescendantrelation.DescendantVisible = '1'
					GROUP BY pderived.ID, i.ID) derivedinfo ON pderived.ID = derivedinfo.ID AND i.ID = derivedinfo.InsertedID
		END
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

UPDATE pa
SET Value = RIGHT('0000000000' + CAST(CAST(pa.Value AS BIGINT) + 2147483648 AS VARCHAR(10)), 10)
FROM FORUM_PostAttribute pa JOIN
	FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID
WHERE pat.AttributeDataTypeID = 2

UPDATE pa
SET Value = RIGHT('0000000000000000000' + pa.Value, 19)
FROM FORUM_PostAttribute pa JOIN
	FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID
WHERE pat.AttributeDataTypeID = 4

exec sp_fulltext_column N'[dbo].[FORUM_PostFullText]', N'FullText', N'add', 0  

DECLARE @intOffset INT
SELECT @intOffset = -1
DECLARE @intbOffset BIGINT
SELECT @intbOffset = @intOffset * 36000000000;

--UPDATE pa
--SET Value = RIGHT('0000000000000000000' + CAST(CAST(pa.Value AS BIGINT) + @intbOffset - COALESCE(
--	CASE WHEN pa.Value BETWEEN '0632474892000000000' AND '0632662380000000000' THEN 36000000000 END,
--	CASE WHEN pa.Value BETWEEN '0632789388000000000' AND '0632976876000000000' THEN 36000000000 END,
--	CASE WHEN pa.Value BETWEEN '0633103884000000000' AND '0633291372000000000' THEN 36000000000 END,
--	0) AS VARCHAR(19)), 19)
--FROM dbo.FORUM_PostAttribute pa JOIN
--	dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID = pat.ID
--WHERE pat.AttributeDataTypeID = 4

UPDATE dbo.FORUM_Post
SET	TimePosted	= DATEADD(hh, @intOffset - ISNULL(
	CASE YEAR(TimePosted) 
	WHEN 2005 THEN CASE WHEN TimePosted BETWEEN '2005-03-27 03:00:00' AND '2005-10-30 03:00' THEN 1 END
	WHEN 2006 THEN CASE WHEN TimePosted BETWEEN '2006-03-26 03:00:00' AND '2006-10-29 03:00' THEN 1 END
	WHEN 2007 THEN CASE WHEN TimePosted BETWEEN '2007-03-25 03:00:00' AND '2007-10-28 03:00' THEN 1 END
	END, 0), TimePosted)
	
UPDATE dbo.FORUM_PostDerivedInformation
SET	TimeLastPostVisible	= DATEADD(hh, @intOffset - ISNULL(
	CASE YEAR(TimeLastPostVisible) 
	WHEN 2005 THEN CASE WHEN TimeLastPostVisible BETWEEN '2005-03-27 03:00:00' AND '2005-10-30 03:00' THEN 1 END
	WHEN 2006 THEN CASE WHEN TimeLastPostVisible BETWEEN '2006-03-26 03:00:00' AND '2006-10-29 03:00' THEN 1 END
	WHEN 2007 THEN CASE WHEN TimeLastPostVisible BETWEEN '2007-03-25 03:00:00' AND '2007-10-28 03:00' THEN 1 END
	END, 0), TimeLastPostVisible),
	TimeLastPostAll		= DATEADD(hh, @intOffset - ISNULL(
	CASE YEAR(TimeLastPostAll) 
	WHEN 2005 THEN CASE WHEN TimeLastPostAll BETWEEN '2005-03-27 03:00:00' AND '2005-10-30 03:00' THEN 1 END
	WHEN 2006 THEN CASE WHEN TimeLastPostAll BETWEEN '2006-03-26 03:00:00' AND '2006-10-29 03:00' THEN 1 END
	WHEN 2007 THEN CASE WHEN TimeLastPostAll BETWEEN '2007-03-25 03:00:00' AND '2007-10-28 03:00' THEN 1 END
	END, 0), TimeLastPostAll)

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
ALTER TABLE dbo.FORUM_Post
	DROP CONSTRAINT DF_FORUM_Post_TimePosted
GO
ALTER TABLE dbo.FORUM_Post ADD CONSTRAINT
	DF_FORUM_Post_TimePosted DEFAULT (getutcdate()) FOR TimePosted
GO
COMMIT

DELETE dbo.FORUM_AttributeDataType WHERE ID = 6