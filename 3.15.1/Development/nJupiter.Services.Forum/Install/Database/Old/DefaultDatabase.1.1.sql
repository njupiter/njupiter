if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_CategoryAttributeType_FORUM_AttributeDataType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_CategoryAttributeType] DROP CONSTRAINT FK_FORUM_CategoryAttributeType_FORUM_AttributeDataType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_PostAttributeType_FORUM_AttributeDataType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_PostAttributeType] DROP CONSTRAINT FK_FORUM_PostAttributeType_FORUM_AttributeDataType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_CategoryAttribute_FORUM_Category]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_CategoryAttribute] DROP CONSTRAINT FK_FORUM_CategoryAttribute_FORUM_Category
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_Post_FORUM_Category]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_Post] DROP CONSTRAINT FK_FORUM_Post_FORUM_Category
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_CategoryAttribute_FORUM_CategoryAttributeType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_CategoryAttribute] DROP CONSTRAINT FK_FORUM_CategoryAttribute_FORUM_CategoryAttributeType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_Post_FORUM_Post]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_Post] DROP CONSTRAINT FK_FORUM_Post_FORUM_Post
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_PostAttribute_FORUM_Post]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_PostAttribute] DROP CONSTRAINT FK_FORUM_PostAttribute_FORUM_Post
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_PostDerivedInformation_FORUM_Post]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_PostDerivedInformation] DROP CONSTRAINT FK_FORUM_PostDerivedInformation_FORUM_Post
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_PostFullText_FORUM_Post]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_PostFullText] DROP CONSTRAINT FK_FORUM_PostFullText_FORUM_Post
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_PostRelation_FORUM_Post_Ancestor]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_PostRelation] DROP CONSTRAINT FK_FORUM_PostRelation_FORUM_Post_Ancestor
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_PostRelation_FORUM_Post_Descendant]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_PostRelation] DROP CONSTRAINT FK_FORUM_PostRelation_FORUM_Post_Descendant
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_FORUM_PostAttribute_FORUM_PostAttributeType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[FORUM_PostAttribute] DROP CONSTRAINT FK_FORUM_PostAttribute_FORUM_PostAttributeType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerUpdate]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerUpdate]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerInsert]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerInsert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerDelete]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerDelete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_CategoryAttribute]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_CategoryAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_PostAttribute]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_PostAttribute]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_PostDerivedInformation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_PostDerivedInformation]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_PostFullText]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_PostFullText]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_PostRelation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_PostRelation]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_CategoryAttributeType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_CategoryAttributeType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_Post]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_PostAttributeType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_PostAttributeType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_AttributeDataType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_AttributeDataType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Category]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[FORUM_Category]
GO

CREATE TABLE [dbo].[FORUM_AttributeDataType] (
	[ID] [int] NOT NULL ,
	[TypeName] [nvarchar] (260) NOT NULL ,
	[AssemblyName] [nvarchar] (260) NULL ,
	[AssemblyPath] [nvarchar] (260) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_Category] (
	[ID]  uniqueidentifier ROWGUIDCOL  NOT NULL ,
	[Name] [nvarchar] (100) NOT NULL ,
	[Domain] [varchar] (100) NOT NULL ,
	[Visible] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_CategoryAttributeType] (
	[ID] [int] NOT NULL ,
	[Name] [varchar] (100) NOT NULL ,
	[AttributeDataTypeID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_Post] (
	[ID]  uniqueidentifier ROWGUIDCOL  NOT NULL ,
	[ParentID] [uniqueidentifier] NULL ,
	[CategoryID] [uniqueidentifier] NULL ,
	[UserIdentity] [varchar] (255) NULL ,
	[Author] [nvarchar] (255) NULL ,
	[Title] [nvarchar] (255) NOT NULL ,
	[Body] [ntext] NOT NULL ,
	[TimePosted] [datetime] NOT NULL ,
	[Visible] [bit] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_PostAttributeType] (
	[ID] [int] NOT NULL ,
	[Name] [varchar] (100) NOT NULL ,
	[AttributeDataTypeID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_CategoryAttribute] (
	[CategoryID] [uniqueidentifier] NOT NULL ,
	[CategoryAttributeTypeID] [int] NOT NULL ,
	[Value] [nvarchar] (3500) NULL ,
	[ExtendedValue] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_PostAttribute] (
	[PostID] [uniqueidentifier] NOT NULL ,
	[PostAttributeTypeID] [int] NOT NULL ,
	[Value] [nvarchar] (3500) NULL ,
	[ExtendedValue] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_PostDerivedInformation] (
	[ID] [uniqueidentifier] NOT NULL ,
	[PostCountAll] [int] NOT NULL ,
	[PostCountVisible] [int] NOT NULL ,
	[TimeLastPostAll] [datetime] NOT NULL ,
	[TimeLastPostVisible] [datetime] NOT NULL ,
	[Level] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_PostFullText] (
	[ID] [uniqueidentifier] NOT NULL ,
	[FullText] [ntext] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_PostRelation] (
	[AncestorPostID] [uniqueidentifier] NOT NULL ,
	[DescendantPostID] [uniqueidentifier] NOT NULL ,
	[DescendantVisible] [bit] NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FORUM_AttributeDataType] WITH NOCHECK ADD 
	CONSTRAINT [PK_FORUM_AttributeDataType] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_Post] WITH NOCHECK ADD 
	CONSTRAINT [PK_FORUM_Post] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_CategoryAttribute] WITH NOCHECK ADD 
	CONSTRAINT [PK_FORUM_CategoryAttribute] PRIMARY KEY  CLUSTERED 
	(
		[CategoryID],
		[CategoryAttributeTypeID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_PostAttribute] WITH NOCHECK ADD 
	CONSTRAINT [PK_FORUM_PostAttribute] PRIMARY KEY  CLUSTERED 
	(
		[PostID],
		[PostAttributeTypeID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_PostDerivedInformation] WITH NOCHECK ADD 
	CONSTRAINT [PK_FORUM_PostDerivedInformation] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_PostFullText] WITH NOCHECK ADD 
	CONSTRAINT [PK_FORUM_PostFullText] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_PostRelation] WITH NOCHECK ADD 
	CONSTRAINT [PK_FORUM_PostRelation] PRIMARY KEY  CLUSTERED 
	(
		[AncestorPostID],
		[DescendantPostID]
	)  ON [PRIMARY] 
GO

 CREATE  UNIQUE  CLUSTERED  INDEX [IX_FORUM_Category_Name_Domain_unique] ON [dbo].[FORUM_Category]([Name], [Domain]) ON [PRIMARY]
GO

 CREATE  UNIQUE  CLUSTERED  INDEX [IX_FORUM_CategoryAttributeType_Name_unique] ON [dbo].[FORUM_CategoryAttributeType]([Name]) ON [PRIMARY]
GO

 CREATE  UNIQUE  CLUSTERED  INDEX [IX_FORUM_PostAttributeType_Name_unique] ON [dbo].[FORUM_PostAttributeType]([Name]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FORUM_AttributeDataType] ADD 
	CONSTRAINT [IX_FORUM_AttributeDataType_TypeName_unique] UNIQUE  NONCLUSTERED 
	(
		[TypeName]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_Category] ADD 
	CONSTRAINT [DF_FORUM_Category_ID] DEFAULT (newid()) FOR [ID],
	CONSTRAINT [DF_FORUM_Category_Visible] DEFAULT (1) FOR [Visible],
	CONSTRAINT [PK_FORUM_Category] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_CategoryAttributeType] ADD 
	CONSTRAINT [PK_FORUM_CategoryAttributeType] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_Post] ADD 
	CONSTRAINT [DF_FORUM_Post_ID] DEFAULT (newid()) FOR [ID],
	CONSTRAINT [DF_FORUM_Post_TimePosted] DEFAULT (getdate()) FOR [TimePosted],
	CONSTRAINT [DF_FORUM_Post_Visible] DEFAULT (1) FOR [Visible],
	CONSTRAINT [CK_FORUM_Post] CHECK (((not([ParentID] is null and [CategoryID] is null))) and ((not([ParentID] is not null and [CategoryID] is not null))))
GO

 CREATE  INDEX [IX_FORUM_Post_UserIdentity] ON [dbo].[FORUM_Post]([UserIdentity]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FORUM_PostAttributeType] ADD 
	CONSTRAINT [PK_FORUM_PostAttributeType] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FORUM_PostRelation] ADD 
	CONSTRAINT [CK_FORUM_PostRelation] CHECK ([AncestorPostID] <> [DescendantPostID])
GO

 CREATE  INDEX [IX_FORUM_PostRelation_AncestorPostID] ON [dbo].[FORUM_PostRelation]([AncestorPostID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_FORUM_PostRelation_DescendantPostID] ON [dbo].[FORUM_PostRelation]([DescendantPostID]) ON [PRIMARY]
GO

if (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) <> 1 
exec sp_fulltext_database N'enable' 

GO

if not exists (select * from dbo.sysfulltextcatalogs where name = N'FORUM')
exec sp_fulltext_catalog N'FORUM', N'create' 

GO

exec sp_fulltext_table N'[dbo].[FORUM_PostFullText]', N'create', N'FORUM', N'PK_FORUM_PostFullText'
GO

exec sp_fulltext_column N'[dbo].[FORUM_PostFullText]', N'FullText', N'add', 1033  
GO

exec sp_fulltext_table N'[dbo].[FORUM_PostFullText]', N'activate'  
GO

ALTER TABLE [dbo].[FORUM_CategoryAttributeType] ADD 
	CONSTRAINT [FK_FORUM_CategoryAttributeType_FORUM_AttributeDataType] FOREIGN KEY 
	(
		[AttributeDataTypeID]
	) REFERENCES [dbo].[FORUM_AttributeDataType] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[FORUM_Post] ADD 
	CONSTRAINT [FK_FORUM_Post_FORUM_Category] FOREIGN KEY 
	(
		[CategoryID]
	) REFERENCES [dbo].[FORUM_Category] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_FORUM_Post_FORUM_Post] FOREIGN KEY 
	(
		[ParentID]
	) REFERENCES [dbo].[FORUM_Post] (
		[ID]
	)
GO

ALTER TABLE [dbo].[FORUM_PostAttributeType] ADD 
	CONSTRAINT [FK_FORUM_PostAttributeType_FORUM_AttributeDataType] FOREIGN KEY 
	(
		[AttributeDataTypeID]
	) REFERENCES [dbo].[FORUM_AttributeDataType] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[FORUM_CategoryAttribute] ADD 
	CONSTRAINT [FK_FORUM_CategoryAttribute_FORUM_Category] FOREIGN KEY 
	(
		[CategoryID]
	) REFERENCES [dbo].[FORUM_Category] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_FORUM_CategoryAttribute_FORUM_CategoryAttributeType] FOREIGN KEY 
	(
		[CategoryAttributeTypeID]
	) REFERENCES [dbo].[FORUM_CategoryAttributeType] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[FORUM_PostAttribute] ADD 
	CONSTRAINT [FK_FORUM_PostAttribute_FORUM_Post] FOREIGN KEY 
	(
		[PostID]
	) REFERENCES [dbo].[FORUM_Post] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_FORUM_PostAttribute_FORUM_PostAttributeType] FOREIGN KEY 
	(
		[PostAttributeTypeID]
	) REFERENCES [dbo].[FORUM_PostAttributeType] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[FORUM_PostDerivedInformation] ADD 
	CONSTRAINT [FK_FORUM_PostDerivedInformation_FORUM_Post] FOREIGN KEY 
	(
		[ID]
	) REFERENCES [dbo].[FORUM_Post] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[FORUM_PostFullText] ADD 
	CONSTRAINT [FK_FORUM_PostFullText_FORUM_Post] FOREIGN KEY 
	(
		[ID]
	) REFERENCES [dbo].[FORUM_Post] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[FORUM_PostRelation] ADD 
	CONSTRAINT [FK_FORUM_PostRelation_FORUM_Post_Ancestor] FOREIGN KEY 
	(
		[AncestorPostID]
	) REFERENCES [dbo].[FORUM_Post] (
		[ID]
	),
	CONSTRAINT [FK_FORUM_PostRelation_FORUM_Post_Descendant] FOREIGN KEY 
	(
		[DescendantPostID]
	) REFERENCES [dbo].[FORUM_Post] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

SET QUOTED_IDENTIFIER ON 
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
				SELECT ISNULL(i.Author + ' ', '') + i.Title + ' ', TEXTPTR(post.Body), TEXTPTR(pft.FullText)
				FROM inserted i
					JOIN dbo.FORUM_Post post ON i.ID = post.ID
					JOIN dbo.FORUM_PostFullText pft ON i.ID = pft.ID

			DECLARE 	@chvnAuthorTitleFullText	NVARCHAR(512),
					@binTextPtrSource		BINARY(16),
					@binTextPtrDestination		BINARY(16)

			OPEN curPost
			FETCH curPost INTO @chvnAuthorTitleFullText, @binTextPtrSource, @binTextPtrDestination
			WHILE @@FETCH_STATUS = 0
			BEGIN
				UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtrDestination 0 NULL dbo.FORUM_Post.Body @binTextPtrSource
				UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtrDestination 0 0 @chvnAuthorTitleFullText

				FETCH curPost INTO @chvnAuthorTitleFullText, @binTextPtrSource, @binTextPtrDestination
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
					SELECT COUNT(*) PostCountVisible, MAX(ancestordescendant.TimePosted) TimeLastPostVisible, pderived.ID
					FROM inserted i
						JOIN dbo.FORUM_PostRelation relation ON i.ID = relation.DescendantPostID
						JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
						JOIN dbo.FORUM_PostRelation ancestordescendantrelation ON relation.AncestorPostID = ancestordescendantrelation.AncestorPostID
						JOIN dbo.FORUM_Post ancestordescendant ON ancestordescendantrelation.DescendantPostID = ancestordescendant.ID
					WHERE ancestordescendantrelation.DescendantVisible = '1'
					GROUP BY pderived.ID) derivedinfo ON pderived.ID = derivedinfo.ID
		END
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

CREATE TRIGGER dbo.FORUM_Post_TriggerInsert
ON dbo.FORUM_Post
FOR INSERT
AS
	IF @@ROWCOUNT > 0
	BEGIN
		SET NOCOUNT ON

		-- insert into fulltext index
		INSERT dbo.FORUM_PostFullText
		SELECT i.ID, post.Body
		FROM inserted i
			JOIN dbo.FORUM_Post post ON i.ID = post.ID

		-- insert author and title into fulltext index
		DECLARE curPost 	CURSOR FAST_FORWARD LOCAL FOR
			SELECT ISNULL(i.Author + ' ', '') + i.Title + ' ', TEXTPTR(pft.FullText)
			FROM inserted i
				JOIN dbo.FORUM_PostFullText pft ON i.ID = pft.ID

		DECLARE 	@chvnAuthorTitleFullText 	NVARCHAR(512),
				@binTextPtr 			BINARY(16)

		OPEN curPost
		FETCH curPost INTO @chvnAuthorTitleFullText, @binTextPtr
		WHILE @@FETCH_STATUS = 0
		BEGIN
			UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtr 0 0 @chvnAuthorTitleFullText

			FETCH curPost INTO @chvnAuthorTitleFullText, @binTextPtr
		END
		CLOSE curPost
		DEALLOCATE curPost

		-- insert into derived information
		-- (make me one level higher than my parent, if I have one, else 0)
		INSERT dbo.FORUM_PostDerivedInformation
		SELECT i.ID, 0, 0, i.TimePosted, i.TimePosted, ISNULL(pderived.Level + 1, 0)
		FROM inserted i
			LEFT JOIN dbo.FORUM_PostDerivedInformation pderived ON i.ParentID = pderived.ID

		-- insert into relations (my parent and me)
		-- (make me visible or not for my parent)
		INSERT dbo.FORUM_PostRelation
		SELECT ParentID, ID, Visible
		FROM inserted
		WHERE ParentID IS NOT NULL

		-- insert into relations (my parent's ancestors and me)
		-- (make me invisible for my parent's ancestors if I or my parent is invisible,
		-- else make me visible for all ancestors that does not have invisible children in our line of ancestry,
		-- that is, between my parent and the ancestors)
		INSERT dbo.FORUM_PostRelation
		SELECT relation.AncestorPostID, i.ID,
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

SET QUOTED_IDENTIFIER ON 
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
				SELECT COUNT(*) PostCountAll, MAX(parentdescendant.TimePosted) TimeLastPostAll, pderived.ID
				FROM deleted d
					JOIN dbo.FORUM_PostDerivedInformation pderived ON d.ParentID = pderived.ID
					JOIN dbo.FORUM_PostRelation parentdescendantrelation ON pderived.ID = parentdescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post parentdescendant ON parentdescendantrelation.DescendantPostID = parentdescendant.ID
				GROUP BY pderived.ID) derivedinfoall ON pderived.ID = derivedinfoall.ID
			LEFT JOIN (
				SELECT COUNT(*) PostCountVisible, MAX(parentdescendant.TimePosted) TimeLastPostVisible, pderived.ID
				FROM deleted d
					JOIN dbo.FORUM_PostDerivedInformation pderived ON d.ParentID = pderived.ID
					JOIN dbo.FORUM_PostRelation parentdescendantrelation ON pderived.ID = parentdescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post parentdescendant ON parentdescendantrelation.DescendantPostID = parentdescendant.ID
				WHERE parentdescendantrelation.DescendantVisible = '1'
				GROUP BY pderived.ID) derivedinfovisible ON pderived.ID = derivedinfovisible.ID

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
				SELECT COUNT(*) PostCountAll, MAX(ancestordescendant.TimePosted) TimeLastPostAll, pderived.ID
				FROM deleted d
					JOIN dbo.FORUM_PostRelation relation ON d.ParentID = relation.DescendantPostID
					JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
					JOIN dbo.FORUM_PostRelation ancestordescendantrelation ON relation.AncestorPostID = ancestordescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post ancestordescendant ON ancestordescendantrelation.DescendantPostID = ancestordescendant.ID
				GROUP BY pderived.ID) derivedinfoall ON pderived.ID = derivedinfoall.ID
			LEFT JOIN (
				SELECT COUNT(*) PostCountVisible, MAX(ancestordescendant.TimePosted) TimeLastPostVisible, pderived.ID
				FROM deleted d
					JOIN dbo.FORUM_PostRelation relation ON d.ParentID = relation.DescendantPostID
					JOIN dbo.FORUM_PostDerivedInformation pderived ON relation.AncestorPostID = pderived.ID
					JOIN dbo.FORUM_PostRelation ancestordescendantrelation ON relation.AncestorPostID = ancestordescendantrelation.AncestorPostID
					JOIN dbo.FORUM_Post ancestordescendant ON ancestordescendantrelation.DescendantPostID = ancestordescendant.ID
				WHERE ancestordescendantrelation.DescendantVisible = '1'
				GROUP BY pderived.ID) derivedinfovisible ON pderived.ID = derivedinfovisible.ID
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

exec sp_fulltext_table @tabname=N'[dbo].[FORUM_PostFullText]', @action=N'start_change_tracking'
GO 

exec sp_fulltext_table @tabname=N'[dbo].[FORUM_PostFullText]', @action=N'start_background_updateindex'
GO

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

SET QUOTED_IDENTIFIER ON 
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
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
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

SET QUOTED_IDENTIFIER ON 
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

SET QUOTED_IDENTIFIER ON 
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

SET QUOTED_IDENTIFIER ON 
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

SET QUOTED_IDENTIFIER ON 
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
Revision history:
	2005-04-11	Kai de Leeuw 	Created
*/
CREATE PROC dbo.FORUM_SaveCategory
	@guidID 	UNIQUEIDENTIFIER,
	@chvnName 	NVARCHAR(100),
	@bitVisible 	BIT,
	@chvDomain 	VARCHAR(100)		= NULL
AS
	SET NOCOUNT ON

	UPDATE dbo.FORUM_Category
	SET 	Name 	= @chvnName,
		Visible 	= @bitVisible,
		Domain	= ISNULL(@chvDomain, Domain)
	WHERE ID = @guidID

	IF @@ROWCOUNT = 0
		INSERT dbo.FORUM_Category (ID, Name, Visible, Domain)
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
Revision history:
	2005-04-11	Kai de Leeuw 	Created
	2005-04-13	Kai de Leeuw 	Made it possible to move root posts between categories
	2006-03-03	Kai de Leeuw	Added the possibility to specify a domain and categoryname
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

	UPDATE dbo.FORUM_Post
	SET 	UserIdentity 	= @chvUserIdentity,
		Author 		= @chvnAuthor,
		Title 		= @chvnTitle,
		Body 		= @txtnBody,
		Visible 		= @bitVisible,
		CategoryID 	=
		CASE WHEN ParentID IS NULL THEN
			ISNULL(@guidCategoryID,
			CASE WHEN @chvDomain IS NOT NULL AND @chvnCategoryName IS NOT NULL THEN
				(SELECT ID
				FROM dbo.FORUM_Category
				WHERE Domain = @chvDomain AND Name = @chvnCategoryName)
			ELSE
				CategoryID
			END)
		END
	WHERE ID = @guidID

	IF @@ROWCOUNT = 0
		INSERT dbo.FORUM_Post(ID, ParentID, CategoryID, UserIdentity, Author, Title, Body, Visible)
		SELECT @guidID, @guidParentID, ISNULL(@guidCategoryID,
		CASE WHEN @chvDomain IS NOT NULL AND @chvnCategoryName IS NOT NULL THEN
			(SELECT ID
			FROM dbo.FORUM_Category
			WHERE Domain = @chvDomain AND Name = @chvnCategoryName)
		END), @chvUserIdentity, @chvnAuthor, @chvnTitle, @txtnBody, @bitVisible
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

-- SELECT 'INSERT INTO FORUM_AttributeDataType(ID, Name) VALUES (' + CAST(ID AS CHAR(1)) + ',''' + Name + ''')'
-- FROM FORUM_AttributeDataType

INSERT INTO FORUM_AttributeDataType(ID, TypeName) VALUES (1,'nJupiter.Services.Forum.BoolAttribute')
INSERT INTO FORUM_AttributeDataType(ID, TypeName) VALUES (2,'nJupiter.Services.Forum.IntAttribute')
INSERT INTO FORUM_AttributeDataType(ID, TypeName) VALUES (3,'nJupiter.Services.Forum.StringAttribute')
INSERT INTO FORUM_AttributeDataType(ID, TypeName) VALUES (4,'nJupiter.Services.Forum.DateTimeAttribute')
INSERT INTO FORUM_AttributeDataType(ID, TypeName) VALUES (5,'nJupiter.Services.Forum.BinaryAttribute')

-- SELECT 'INSERT INTO FORUM_CategoryAttributeType(ID, Name, AttributeDataTypeID) VALUES (' + CAST(ID AS CHAR(1)) + ',''' + Name + ''',' + CAST(AttributeDataTypeID AS CHAR(1)) + ')'
-- FROM FORUM_CategoryAttributeType

/*
INSERT INTO FORUM_CategoryAttributeType(ID, Name, AttributeDataTypeID) VALUES (1,'CategoryType',2)
*/

-- SELECT 'INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (' + CAST(ID AS CHAR(1)) + ',''' + Name + ''',' + CAST(AttributeDataTypeID AS CHAR(1)) + ')'
-- FROM FORUM_PostAttributeType

/*
INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (1,'EmailAddress',3)
INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (2,'IPAddress',3)
INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (3,'HostName',3)
INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (4,'Blocked',1)
INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (5,'BlockedBy',3)
INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (6,'TimeBlocked',4)
INSERT INTO FORUM_PostAttributeType(ID, Name, AttributeDataTypeID) VALUES (7,'AbuseCount',2)
*/