exec sp_fulltext_table @tabname=N'[dbo].[FORUM_PostFullText]', @action=N'start_change_tracking'
GO 

exec sp_fulltext_table @tabname=N'[dbo].[FORUM_PostFullText]', @action=N'start_background_updateindex'
GO