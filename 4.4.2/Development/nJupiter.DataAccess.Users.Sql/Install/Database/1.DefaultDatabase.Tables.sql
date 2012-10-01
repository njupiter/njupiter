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

 CREATE  UNIQUE  INDEX [IX_USER_Property_Timestamp] ON [dbo].[USER_Property]([Timestamp]) ON [PRIMARY]
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