SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
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
ALTER PROC dbo.FORUM_FilterPosts
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