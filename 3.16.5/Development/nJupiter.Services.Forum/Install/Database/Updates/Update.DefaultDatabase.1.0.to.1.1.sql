if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_DeletePosts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_DeletePosts]
GO

SET QUOTED_IDENTIFIER ON 
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
		@dteSameOrOlderThan
Revision history:
	2005-04-13	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_DeletePosts
	@guidID			UNIQUEIDENTIFIER	= NULL,
	@bitDeleteOnlyChildren		BIT			= NULL,
	@guidCategoryID 		UNIQUEIDENTIFIER 	= NULL,
	@chvDomain 			VARCHAR(100) 		= NULL,
	@chvnCategoryName 		NVARCHAR(100) 	= NULL,
	@dteSameOrOlderThan 		DATETIME		= NULL
AS
	SET NOCOUNT ON

	DECLARE 	@tblAncestorPost	TABLE (
		ID 	UNIQUEIDENTIFIER PRIMARY KEY
	)

	IF @guidID IS NULL
		INSERT @tblAncestorPost
		SELECT ancestor.ID
		FROM dbo.FORUM_Post ancestor
			JOIN dbo.FORUM_PostDerivedInformation pdi ON ancestor.ID = pdi.ID
			JOIN dbo.FORUM_Category c ON ancestor.CategoryID = c.ID
		WHERE c.ID = ISNULL(@guidCategoryID, c.ID) AND c.Domain = ISNULL(@chvDomain, c.Domain) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
			pdi.TimeLastPostAll <= ISNULL(@dteSameOrOlderThan, pdi.TimeLastPostAll)
	ELSE IF @bitDeleteOnlyChildren <> '1'
		INSERT @tblAncestorPost
		SELECT p.ID
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
		WHERE p.ID = @guidID AND
			pdi.TimeLastPostAll <= ISNULL(@dteSameOrOlderThan, pdi.TimeLastPostAll)
	ELSE
		INSERT @tblAncestorPost
		SELECT p.ID
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
		WHERE p.ParentID = @guidID AND
			pdi.TimeLastPostAll <= ISNULL(@dteSameOrOlderThan, pdi.TimeLastPostAll)
		
	WHILE @@ROWCOUNT > 0
		DELETE descendant
		FROM dbo.FORUM_Post descendant
			JOIN dbo.FORUM_PostDerivedInformation pdi ON descendant.ID = pdi.ID
			JOIN dbo.FORUM_PostRelation pr ON descendant.ID = pr.DescendantPostID
			JOIN @tblAncestorPost ancestor ON pr.AncestorPostID = ancestor.ID
		WHERE pdi.PostCountAll = 0

	DELETE p
	FROM dbo.FORUM_Post p
		JOIN @tblAncestorPost ancestor ON p.ID = ancestor.ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_FilterCategories]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_FilterCategories]
GO

SET QUOTED_IDENTIFIER ON 
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
--we can probably skip sending in @chvDomain to dbo.FORUM_FilterPosts
CREATE PROC dbo.FORUM_FilterCategories
	@chvDomain 			VARCHAR(100) 		= NULL,
	@guidID 			UNIQUEIDENTIFIER 	= NULL,
	@guidPostID			UNIQUEIDENTIFIER	= NULL,
	@chvnName 			NVARCHAR(100) 	= NULL,
	@bitIncludeHidden 		BIT 			= NULL,
	@bitGetAttributes 		BIT 			= NULL,
	@bitPostIncludeHidden		BIT			= NULL,
	@intPostLevels 			INT 			= NULL,
	@intPostLimitSize 		INT 			= NULL,
	@intPostLimitSortColumn 	INT 			= NULL,
	@bitPostLimitSortDirectionAsc 	BIT 			= NULL,
	@intPostLimitPage 		INT 			= NULL,
	@bitPostGetAttributes 		BIT 			= NULL
AS
	SET NOCOUNT ON

	DECLARE 	@tblResultCategory	TABLE (
		ID 		UNIQUEIDENTIFIER PRIMARY KEY,
		Name 		NVARCHAR(100),
		Visible 		BIT,
		RootPostCount 	INT, 
		Row		INT IDENTITY UNIQUE)

	DECLARE 	@intLoopIndex	INT

	IF @guidPostID IS NOT NULL
		INSERT @tblResultCategory
		SELECT c.ID, c.Name, c.Visible,
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
		INSERT @tblResultCategory
		SELECT c.ID, c.Name, c.Visible,
			(SELECT COUNT(*)
			FROM dbo.FORUM_Post 
			WHERE CategoryID = c.ID AND (@bitPostIncludeHidden <> '0' OR Visible = '1'))
		FROM dbo.FORUM_Category c
		WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND c.ID = ISNULL(@guidID, c.ID) AND c.Name = ISNULL(@chvnName, c.Name) AND
			(@bitIncludeHidden <> '0' OR @guidID IS NOT NULL OR (@chvDomain IS NOT NULL AND @chvnName IS NOT NULL) OR c.Visible = '1')

	SELECT	@intLoopIndex	= @@ROWCOUNT

	SELECT ID, Name, Visible, RootPostCount
	FROM @tblResultCategory

	IF @bitGetAttributes <> '0'
		SELECT cat.Name, c.ID, ca.Value, ca.ExtendedValue
		FROM @tblResultCategory c
			JOIN dbo.FORUM_CategoryAttribute ca ON c.ID = ca.CategoryID
			JOIN dbo.FORUM_CategoryAttributeType cat ON ca.CategoryAttributeTypeID = cat.ID
	
	IF @intPostLevels IS NULL OR @intPostLevels > 0
	BEGIN
		CREATE TABLE dbo.#CategoryResultPost (
			ID 	UNIQUEIDENTIFIER PRIMARY KEY,
			Rank 	SMALLINT)
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

			INSERT dbo.#CategoryResultPost
			EXEC @intPagingTotalNumberOfPosts = dbo.FORUM_FilterPosts
				@chvDomain 			= @chvDomain,
				@guidCategoryID 		= @guidLoopID,
				@chvnCategoryName 		= @chvnName,
				@intLevels 			= @intPostLevels,
				@bitIncludeHidden 		= @bitPostIncludeHidden,
				@intLimitSize 			= @intPostLimitSize,
				@intLimitSortColumn 		= @intPostLimitSortColumn,
				@bitLimitSortDirectionAsc	= @bitPostLimitSortDirectionAsc,
				@intLimitPage 			= @intPostLimitPage,
				@bitGetOnlyMetaData 		= '1'

			INSERT @tblPagingTotalCount
			VALUES(@guidLoopID, @intPagingTotalNumberOfPosts)
		END

		SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible,
			CASE WHEN @bitPostIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END PostCount,
			CASE WHEN @bitPostIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END TimeLastPost, p3.Rank
		FROM dbo.FORUM_Post p
			JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
			JOIN dbo.#CategoryResultPost p3 ON p.ID = p3.ID
		ORDER BY p.TimePosted

		SELECT *
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_FilterPosts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[FORUM_FilterPosts]
GO

SET QUOTED_IDENTIFIER ON 
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
		@intLevels
		@chvnSearchText
		@intLimitSize
		@intLimitSortColumn
		@bitLimitSortDirectionAsc
		@intLimitPage
		@bitGetOnlyMetaData
		@bitGetAttributes
Revision history:
	2005-04-11	Kai de Leeuw 	Created
*/
--TODO:
--optimize by fetching directly when not limiting (makes page, sortcolumn, sortasc meaningless)
--page on children if we fetch one post
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
	@intLevels 			INT 			= NULL,
	@chvnSearchText 		NVARCHAR(4000) 	= NULL,
	@chvUserIdentity		VARCHAR(255)		= NULL,
	@intLimitSize 			INT 			= NULL,
	@intLimitSortColumn 		INT 			= NULL,
	@bitLimitSortDirectionAsc 	BIT 			= NULL,
	@intLimitPage 			INT 			= NULL,
	@bitGetOnlyMetaData 		BIT 			= NULL,
	@bitGetAttributes		BIT			= NULL
AS
	SET NOCOUNT ON

	DECLARE	@tblUnFinishedBranch	TABLE (
		ID 	UNIQUEIDENTIFIER PRIMARY KEY,
		Rank 	SMALLINT)

	DECLARE 	@tblResultPost		TABLE (
		ID 	UNIQUEIDENTIFIER,
		Rank 	SMALLINT,
		Row 	INT IDENTITY PRIMARY KEY)

	DECLARE 	@intNumberOfPosts 		INT,
			@intStartIndex 			INT,
			@intStartRow 			INT,
			@intWhereToStart 		INT,
			@intWhereToEnd 		INT,
			@intHowMany 			INT,
			@intPagingTotalNumberOfPosts 	INT,
			@intOldLevels 			INT,
			@bitFetchBranches 		BIT,
			@guidSingleID 			UNIQUEIDENTIFIER

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
			INSERT @tblUnFinishedBranch
			SELECT @guidSingleID, search.Rank
			FROM dbo.FORUM_Post root
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON root.ID = search.[KEY]
			WHERE root.ID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR root.UserIdentity = @chvUserIdentity)
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE relation.AncestorPostID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidID IS NOT NULL
			INSERT @tblUnFinishedBranch
			SELECT ID, search.Rank
			FROM dbo.FORUM_Post child
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON child.ID = search.[KEY]
			WHERE child.ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR child.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR child.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post child ON relation.AncestorPostID = child.ID
				JOIN dbo.FORUM_Post childdescendant ON relation.DescendantPostID = childdescendant.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE child.ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR childdescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidImmediateDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch
			SELECT parentchild.ID, search.Rank
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON parentchild.ID = search.[KEY]
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchild.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON parentchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post parentchilddescendant ON relation.DescendantPostID = parentchilddescendant.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchilddescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidRootDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch
			SELECT rootchild.ID, search.Rank
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON rootchild.ID = search.[KEY]
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchild.UserIdentity = @chvUserIdentity) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation 
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR rootchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON rootchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootchilddescendant ON relation.DescendantPostID = rootchilddescendant.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON rootchild.ID = search.[KEY]
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchilddescendant.UserIdentity = @chvUserIdentity) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE
			INSERT @tblUnFinishedBranch
			SELECT root.ID, search.Rank
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON root.ID = search.[KEY]
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR root.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR
					(@chvDomain IS NULL AND root.Visible = '1') OR
					(@chvDomain IS NOT NULL AND c.Visible = '1' AND root.Visible = '1'))
			UNION ALL
			SELECT relation.DescendantPostID, search.Rank
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN dbo.FORUM_PostRelation relation ON root.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
				JOIN CONTAINSTABLE(dbo.FORUM_PostFullText, FullText, @chvnSearchText) search ON relation.DescendantPostID = search.[KEY]
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR
					(@chvDomain IS NULL AND root.Visible = '1' AND relation.DescendantVisible = '1') OR
					(@chvDomain IS NOT NULL AND c.Visible = '1' AND root.Visible = '1' AND relation.DescendantVisible = '1'))
	ELSE IF @chvUserIdentity IS NOT NULL
		IF @guidSingleID IS NOT NULL
			INSERT @tblUnFinishedBranch
			SELECT @guidSingleID, 0
			FROM dbo.FORUM_Post
			WHERE ID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR UserIdentity = @chvUserIdentity)
			UNION ALL
			SELECT relation.DescendantPostID, 0
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
			WHERE relation.AncestorPostID = @guidSingleID AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidID IS NOT NULL
			INSERT @tblUnFinishedBranch
			SELECT ID, 0
			FROM dbo.FORUM_Post
			WHERE ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, 0
			FROM dbo.FORUM_PostRelation relation
				JOIN dbo.FORUM_Post child ON relation.AncestorPostID = child.ID
				JOIN dbo.FORUM_Post childdescendant ON relation.DescendantPostID = childdescendant.ID
			WHERE child.ParentID = @guidID AND
				(@chvUserIdentity IS NULL OR childdescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidImmediateDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch
			SELECT parentchild.ID, 0
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchild.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, 0
			FROM dbo.FORUM_Post child
				JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON parentchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post parentchilddescendant ON relation.DescendantPostID = parentchilddescendant.ID
			WHERE child.ID = @guidImmediateDescendantID AND
				(@chvUserIdentity IS NULL OR parentchilddescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE IF @guidRootDescendantID IS NOT NULL
			INSERT @tblUnFinishedBranch
			SELECT rootchild.ID, 0
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchild.UserIdentity = @chvUserIdentity) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation 
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR rootchild.Visible = '1')
			UNION ALL
			SELECT relation.DescendantPostID, 0
			FROM dbo.FORUM_PostRelation pr
				JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
				JOIN dbo.FORUM_PostRelation relation ON rootchild.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootchilddescendant ON relation.DescendantPostID = rootchilddescendant.ID
			WHERE pr.DescendantPostID = @guidRootDescendantID AND
				(@chvUserIdentity IS NULL OR rootchilddescendant.UserIdentity = @chvUserIdentity) AND
				NOT EXISTS (
					SELECT *
					FROM dbo.FORUM_PostRelation
					WHERE DescendantPostID = pr.AncestorPostID) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')
		ELSE
			INSERT @tblUnFinishedBranch
			SELECT root.ID, 0
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR root.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR
					(@chvDomain IS NULL AND root.Visible = '1') OR
					(@chvDomain IS NOT NULL AND c.Visible = '1' AND root.Visible = '1'))
			UNION ALL
			SELECT relation.DescendantPostID, 0
			FROM dbo.FORUM_Post root
				JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
				JOIN dbo.FORUM_PostRelation relation ON root.ID = relation.AncestorPostID
				JOIN dbo.FORUM_Post rootdescendant ON relation.DescendantPostID = rootdescendant.ID
			WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
				(@chvUserIdentity IS NULL OR rootdescendant.UserIdentity = @chvUserIdentity) AND
				(@bitIncludeHidden <> '0' OR
					(@chvDomain IS NULL AND root.Visible = '1' AND relation.DescendantVisible = '1') OR
					(@chvDomain IS NOT NULL AND c.Visible = '1' AND root.Visible = '1' AND relation.DescendantVisible = '1'))
	ELSE IF @guidSingleID IS NOT NULL
		INSERT @tblUnFinishedBranch
		SELECT @guidSingleID, 0
	ELSE IF @guidID IS NOT NULL
		INSERT @tblUnFinishedBranch
		SELECT ID, 0
		FROM dbo.FORUM_Post
		WHERE ParentID = @guidID AND
			(@bitIncludeHidden <> '0' OR Visible = '1')
	ELSE IF @guidImmediateDescendantID IS NOT NULL
		INSERT @tblUnFinishedBranch
		SELECT parentchild.ID, 0
		FROM dbo.FORUM_Post child 
			JOIN dbo.FORUM_Post parentchild ON child.ParentID = parentchild.ParentID
		WHERE child.ID = @guidImmediateDescendantID AND
			(@bitIncludeHidden <> '0' OR parentchild.Visible = '1')
	ELSE IF @guidRootDescendantID IS NOT NULL
		INSERT @tblUnFinishedBranch
		SELECT rootchild.ID, 0
		FROM dbo.FORUM_PostRelation pr
			JOIN dbo.FORUM_Post rootchild ON pr.AncestorPostID = rootchild.ParentID
		WHERE pr.DescendantPostID = @guidRootDescendantID AND
			NOT EXISTS (
				SELECT *
				FROM dbo.FORUM_PostRelation
				WHERE DescendantPostID = pr.AncestorPostID) AND
			(@bitIncludeHidden <> '0' OR rootchild.Visible = '1')
	ELSE
		INSERT @tblUnFinishedBranch
		SELECT root.ID, 0
		FROM dbo.FORUM_Post root
			JOIN dbo.FORUM_Category c ON root.CategoryID = c.ID
		WHERE c.Domain = ISNULL(@chvDomain, c.Domain) AND root.CategoryID = ISNULL(@guidCategoryID, root.CategoryID) AND c.Name = ISNULL(@chvnCategoryName, c.Name) AND
			(@bitIncludeHidden <> '0' OR root.Visible = '1' OR (@chvDomain IS NOT NULL AND c.Visible = '1' AND root.Visible = '1'))

	SELECT	@intPagingTotalNumberOfPosts 	= @@ROWCOUNT

	IF @intLimitSize IS NOT NULL AND (@intLevels IS NULL OR @intLevels <> 1) AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL
		SELECT	@intOldLevels 		= @intLevels,
				@intLevels 		= 1,
				@bitFetchBranches 	= '1'

	WHILE 1 = 1
	BEGIN
		IF @intLevels <> 1 AND @chvnSearchText IS NULL AND @chvUserIdentity IS NULL
			INSERT @tblUnFinishedBranch
			SELECT descendantpdi.ID, 0
			FROM @tblUnFinishedBranch ancestor
				JOIN dbo.FORUM_PostDerivedInformation ancestorpdi ON ancestor.ID = ancestorpdi.ID
				JOIN dbo.FORUM_PostRelation relation ON ancestorpdi.ID = relation.AncestorPostID
				JOIN dbo.FORUM_PostDerivedInformation descendantpdi ON relation.DescendantPostID = descendantpdi.ID
			WHERE (@intLevels IS NULL OR @intLevels > descendantpdi.Level - ancestorpdi.Level) AND
				(@bitIncludeHidden <> '0' OR relation.DescendantVisible = '1')

		SELECT	@intHowMany 	= ISNULL(@intLimitSize, 0) * ISNULL(@intLimitPage, 1)
		SET ROWCOUNT @intHowMany

		INSERT @tblResultPost(ID, Rank)
		SELECT p.ID, unsorted.Rank
		FROM (SELECT match.ID, match.Rank,
				CASE WHEN @bitIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END AS PostCount,
				CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END AS TimeLastPost
			FROM @tblUnFinishedBranch match
				JOIN dbo.FORUM_PostDerivedInformation pdi ON match.ID = pdi.ID) unsorted
			JOIN dbo.FORUM_Post p ON unsorted.ID = p.ID
		ORDER BY
			CASE WHEN @bitLimitSortDirectionAsc <> '0' AND @intLimitSortColumn BETWEEN 8 AND 12 THEN
				CASE @intLimitSortColumn WHEN 8 THEN p.TimePosted WHEN 9 THEN p.Visible WHEN 10 THEN unsorted.PostCount WHEN 11 THEN unsorted.TimeLastPost ELSE unsorted.Rank END
			END ASC,
			CASE WHEN @bitLimitSortDirectionAsc = '0' AND @intLimitSortColumn BETWEEN 8 AND 12 THEN
				CASE @intLimitSortColumn WHEN 8 THEN p.TimePosted WHEN 9 THEN p.Visible WHEN 10 THEN unsorted.PostCount WHEN 11 THEN unsorted.TimeLastPost ELSE unsorted.Rank END
			END DESC,
			CASE WHEN @bitLimitSortDirectionAsc <> '0' AND @intLimitSortColumn BETWEEN 4 AND 7 THEN
				CASE @intLimitSortColumn WHEN 4 THEN p.UserIdentity WHEN 5 THEN p.Author WHEN 6 THEN p.Title ELSE CAST(p.Body AS NVARCHAR(4000)) END
			END ASC,
			CASE WHEN @bitLimitSortDirectionAsc = '0' AND @intLimitSortColumn BETWEEN 4 AND 7 THEN
				CASE @intLimitSortColumn WHEN 4 THEN p.UserIdentity WHEN 5 THEN p.Author WHEN 6 THEN p.Title ELSE CAST(p.Body AS NVARCHAR(4000)) END
			END DESC,
			CASE WHEN @bitLimitSortDirectionAsc <> '0' AND @intLimitSortColumn BETWEEN 1 AND 3 THEN
				CASE @intLimitSortColumn WHEN 1 THEN p.ID WHEN 2 THEN p.ParentID ELSE p.CategoryID END
			END ASC,
			CASE WHEN @bitLimitSortDirectionAsc = '0' AND @intLimitSortColumn BETWEEN 1 AND 3 THEN
				CASE @intLimitSortColumn WHEN 1 THEN p.ID WHEN 2 THEN p.ParentID ELSE p.CategoryID END
			END DESC

		SELECT 	@intNumberOfPosts 	= @@ROWCOUNT,
				@intStartRow 		= @@IDENTITY - @intNumberOfPosts + 1,
				@intStartIndex		= (@intLimitPage - 1) * @intLimitSize + @intStartRow,
				@intWhereToStart 	= ISNULL(@intStartIndex, @intStartRow)

		SET ROWCOUNT 0

		IF @bitFetchBranches = '1'
		BEGIN
			DELETE @tblUnFinishedBranch

			INSERT @tblUnFinishedBranch
			SELECT ID, 0
			FROM @tblResultPost
			WHERE Row >= @intWhereToStart

			SELECT	@intLevels 		= @intOldLevels,
					@intLimitSize 		= NULL,
					@bitFetchBranches	= '0'
		END
		ELSE
		BEGIN
			IF @bitGetOnlyMetaData = '1'
				SELECT ID, Rank
				FROM @tblResultPost
				WHERE Row >= @intWhereToStart
			ELSE
			BEGIN
				SELECT p.ID, p.ParentID, p.CategoryID, p.UserIdentity, p.Author, p.Title, p.Body, p.TimePosted, p.Visible,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.PostCountAll ELSE pdi.PostCountVisible END PostCount,
					CASE WHEN @bitIncludeHidden <> '0' THEN pdi.TimeLastPostAll ELSE pdi.TimeLastPostVisible END TimeLastPost, rp.Rank
				FROM dbo.FORUM_Post p
					JOIN dbo.FORUM_PostDerivedInformation pdi ON p.ID = pdi.ID
					JOIN @tblResultPost rp ON p.ID = rp.ID
				WHERE rp.Row >= @intWhereToStart
				ORDER BY p.TimePosted

				SELECT @intPagingTotalNumberOfPosts PagingTotalNumberOfPosts

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

