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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerDelete]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerDelete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerInsert]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerInsert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FORUM_Post_TriggerUpdate]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[FORUM_Post_TriggerUpdate]
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
	[ID] [uniqueidentifier] NOT NULL ,
	[Name] [nvarchar] (100) NOT NULL ,
	[Domain] [nvarchar] (100) NOT NULL ,
	[Visible] [bit] NOT NULL ,
	[Timestamp] [timestamp] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_CategoryAttributeType] (
	[ID] [int] NOT NULL ,
	[Name] [varchar] (100) NOT NULL ,
	[AttributeDataTypeID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_Post] (
	[ID] [uniqueidentifier] NOT NULL ,
	[ParentID] [uniqueidentifier] NULL ,
	[CategoryID] [uniqueidentifier] NULL ,
	[UserIdentity] [varchar] (255) NULL ,
	[Author] [nvarchar] (255) NULL ,
	[Title] [nvarchar] (255) NOT NULL ,
	[Body] [ntext] NOT NULL ,
	[TimePosted] [datetime] NOT NULL ,
	[Visible] [bit] NOT NULL ,
	[Timestamp] [timestamp] NOT NULL 
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
	[FullText] [ntext] NOT NULL ,
	[Timestamp] [timestamp] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[FORUM_PostRelation] (
	[DescendantPostID] [uniqueidentifier] NOT NULL ,
	[AncestorPostID] [uniqueidentifier] NOT NULL ,
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
		[DescendantPostID],
		[AncestorPostID]
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
	)  ON [PRIMARY] ,
	CONSTRAINT [CK_FORUM_AttributeDataType] CHECK (((not([AssemblyName] is not null and [AssemblyPath] is not null))))
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

 CREATE  INDEX [IX_FORUM_CategoryAttributeType_AttributeDataTypeID] ON [dbo].[FORUM_CategoryAttributeType]([AttributeDataTypeID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FORUM_Post] ADD 
	CONSTRAINT [DF_FORUM_Post_ID] DEFAULT (newid()) FOR [ID],
	CONSTRAINT [DF_FORUM_Post_TimePosted] DEFAULT (getutcdate()) FOR [TimePosted],
	CONSTRAINT [DF_FORUM_Post_Visible] DEFAULT (1) FOR [Visible],
	CONSTRAINT [CK_FORUM_Post] CHECK (((not([ParentID] is null and [CategoryID] is null))) and ((not([ParentID] is not null and [CategoryID] is not null))))
GO

 CREATE  INDEX [IX_FORUM_Post_UserIdentity] ON [dbo].[FORUM_Post]([UserIdentity]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_FORUM_Post_ParentID] ON [dbo].[FORUM_Post]([ParentID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_FORUM_Post_CategoryID] ON [dbo].[FORUM_Post]([CategoryID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FORUM_PostAttributeType] ADD 
	CONSTRAINT [PK_FORUM_PostAttributeType] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_FORUM_PostAttributeType_AttributeDataTypeID] ON [dbo].[FORUM_PostAttributeType]([AttributeDataTypeID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_FORUM_CategoryAttribute_CategoryAttributeTypeID] ON [dbo].[FORUM_CategoryAttribute]([CategoryAttributeTypeID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_FORUM_PostAttribute_PostAttributeTypeID] ON [dbo].[FORUM_PostAttribute]([PostAttributeTypeID]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_FORUM_PostFullText_Timestamp] ON [dbo].[FORUM_PostFullText]([Timestamp]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FORUM_PostRelation] ADD 
	CONSTRAINT [CK_FORUM_PostRelation] CHECK ([AncestorPostID] <> [DescendantPostID])
GO

 CREATE  INDEX [IX_FORUM_PostRelation_AncestorPostID] ON [dbo].[FORUM_PostRelation]([AncestorPostID]) ON [PRIMARY]
GO

if (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) <> 1 
exec sp_fulltext_database N'enable' 

GO

if not exists (select * from dbo.sysfulltextcatalogs where name = N'FORUM')
exec sp_fulltext_catalog N'FORUM', N'create' 

GO

exec sp_fulltext_table N'[dbo].[FORUM_PostFullText]', N'create', N'FORUM', N'PK_FORUM_PostFullText'
GO

exec sp_fulltext_column N'[dbo].[FORUM_PostFullText]', N'FullText', N'add', 0  
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

		DECLARE	@tblInserted	TABLE (
			ID 		UNIQUEIDENTIFIER,
			Row 		INT IDENTITY PRIMARY KEY)

		DECLARE 	@guidID			UNIQUEIDENTIFIER,
				@chvnAuthorTitleFullText 	NVARCHAR(512),
				@binTextPtr 			BINARY(16),
				@intHowMany			INT,
				@intCounter			INT

		-- insert into fulltext index
		INSERT dbo.FORUM_PostFullText(ID, FullText)
		SELECT i.ID, post.Body
		FROM inserted i
			JOIN dbo.FORUM_Post post ON i.ID = post.ID

		INSERT @tblInserted(ID)
		SELECT ID
		FROM inserted

		SELECT	@intHowMany	= @@ROWCOUNT,
				@intCounter	= 1

		-- insert author and title into fulltext index
		WHILE @intCounter <= @intHowMany
		BEGIN
			SELECT	@guidID			= i.ID,
					@chvnAuthorTitleFullText	= ISNULL(i.Author + ' ', '') + i.Title + ' ',
					@binTextPtr			= TEXTPTR(pft.FullText)
			FROM @tblInserted ti
				JOIN inserted i ON ti.ID = i.ID
				JOIN dbo.FORUM_Post post ON i.ID = post.ID
				JOIN dbo.FORUM_PostFullText pft ON i.ID = pft.ID
			WHERE ti.Row = @intCounter

			UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtr 0 0 @chvnAuthorTitleFullText

			-- ensure that full text index is conscious about the change
			UPDATE dbo.FORUM_PostFullText
			SET FullText = FullText
			WHERE ID = @guidID

			IF @intCounter % 1024 = 0
				EXEC dbo.sp_invalidate_textptr

			SELECT	@intCounter	= @intCounter + 1
		END

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
			DECLARE	@tblInserted	TABLE (
				ID 		UNIQUEIDENTIFIER,
				Row 		INT IDENTITY PRIMARY KEY)

			DECLARE 	@guidID			UNIQUEIDENTIFIER,
					@chvnAuthorTitleFullText	NVARCHAR(512),
					@binTextPtrSource		BINARY(16),
					@binTextPtrDestination		BINARY(16),
					@intHowMany			INT,
					@intCounter			INT

			INSERT @tblInserted(ID)
			SELECT ID
			FROM inserted

			SELECT	@intHowMany	= @@ROWCOUNT,
					@intCounter	= 1

			WHILE @intCounter <= @intHowMany
			BEGIN
				SELECT	@guidID			= i.ID,
						@chvnAuthorTitleFullText	= ISNULL(i.Author + ' ', '') + i.Title + ' ',
						@binTextPtrSource		= TEXTPTR(post.Body),
						@binTextPtrDestination		= TEXTPTR(pft.FullText)
				FROM @tblInserted ti
					JOIN inserted i ON ti.ID = i.ID
					JOIN dbo.FORUM_Post post ON i.ID = post.ID
					JOIN dbo.FORUM_PostFullText pft ON i.ID = pft.ID
				WHERE ti.Row = @intCounter

				UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtrDestination 0 NULL dbo.FORUM_Post.Body @binTextPtrSource
				UPDATETEXT dbo.FORUM_PostFullText.FullText @binTextPtrDestination 0 0 @chvnAuthorTitleFullText

				-- ensure that full text index is conscious about the change
				UPDATE dbo.FORUM_PostFullText
				SET FullText = FullText
				WHERE ID = @guidID

				IF @intCounter % 512 = 0
					EXEC dbo.sp_invalidate_textptr

				SELECT	@intCounter	= @intCounter + 1
			END
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