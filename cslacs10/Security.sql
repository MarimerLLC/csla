IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Security')
	DROP DATABASE [Security]
GO

CREATE DATABASE [Security]  ON (NAME = N'Security', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\Security.mdf' , SIZE = 1, FILEGROWTH = 10%) LOG ON (NAME = N'Security_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\Security_log.LDF' , FILEGROWTH = 10%)
GO

exec sp_dboption N'Security', N'autoclose', N'false'
GO

exec sp_dboption N'Security', N'bulkcopy', N'false'
GO

exec sp_dboption N'Security', N'trunc. log', N'false'
GO

exec sp_dboption N'Security', N'torn page detection', N'true'
GO

exec sp_dboption N'Security', N'read only', N'false'
GO

exec sp_dboption N'Security', N'dbo use', N'false'
GO

exec sp_dboption N'Security', N'single', N'false'
GO

exec sp_dboption N'Security', N'autoshrink', N'false'
GO

exec sp_dboption N'Security', N'ANSI null default', N'false'
GO

exec sp_dboption N'Security', N'recursive triggers', N'false'
GO

exec sp_dboption N'Security', N'ANSI nulls', N'false'
GO

exec sp_dboption N'Security', N'concat null yields null', N'false'
GO

exec sp_dboption N'Security', N'cursor close on commit', N'false'
GO

exec sp_dboption N'Security', N'default to local cursor', N'false'
GO

exec sp_dboption N'Security', N'quoted identifier', N'false'
GO

exec sp_dboption N'Security', N'ANSI warnings', N'false'
GO

exec sp_dboption N'Security', N'auto create statistics', N'true'
GO

exec sp_dboption N'Security', N'auto update statistics', N'true'
GO

use [Security]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Roles_Users]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Roles] DROP CONSTRAINT FK_Roles_Users
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Login]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Login]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Roles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Roles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Users]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Users]
GO

CREATE TABLE [dbo].[Roles] (
	[Username] [varchar] (20) NOT NULL ,
	[Role] [varchar] (20) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Users] (
	[Username] [varchar] (20) NOT NULL ,
	[Password] [varchar] (20) NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Roles] WITH NOCHECK ADD 
	CONSTRAINT [PK_Roles] PRIMARY KEY  CLUSTERED 
	(
		[Username],
		[Role]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Users] WITH NOCHECK ADD 
	CONSTRAINT [PK_Users] PRIMARY KEY  CLUSTERED 
	(
		[Username]
	)  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_Roles] ON [dbo].[Roles]([Username]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Roles] ADD 
	CONSTRAINT [FK_Roles_Users] FOREIGN KEY 
	(
		[Username]
	) REFERENCES [dbo].[Users] (
		[Username]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE Login
	(
		@User varchar(20),
		@pw varchar(20)
	)
AS
	SELECT Username 
	FROM Users 
	WHERE Username=@User AND Password=@pw;

	SELECT R.Role 
	FROM Users AS U INNER JOIN Roles AS R ON
      R.UserName = U.UserName
	WHERE U.Username = @User and U.Password = @pw
	
	RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

