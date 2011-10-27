exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_change_tracking'
GO

exec sp_fulltext_table @tabname=N'[dbo].[USER_Property]', @action=N'start_background_updateindex'
GO