IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'PTracker')
	DROP DATABASE [PTracker]
GO

CREATE DATABASE [PTracker]  ON (NAME = N'PTracker', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\PTracker.mdf' , SIZE = 1, FILEGROWTH = 10%) LOG ON (NAME = N'PTracker_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\PTracker_log.LDF' , FILEGROWTH = 10%)
GO

exec sp_dboption N'PTracker', N'autoclose', N'false'
GO

exec sp_dboption N'PTracker', N'bulkcopy', N'false'
GO

exec sp_dboption N'PTracker', N'trunc. log', N'false'
GO

exec sp_dboption N'PTracker', N'torn page detection', N'true'
GO

exec sp_dboption N'PTracker', N'read only', N'false'
GO

exec sp_dboption N'PTracker', N'dbo use', N'false'
GO

exec sp_dboption N'PTracker', N'single', N'false'
GO

exec sp_dboption N'PTracker', N'autoshrink', N'false'
GO

exec sp_dboption N'PTracker', N'ANSI null default', N'false'
GO

exec sp_dboption N'PTracker', N'recursive triggers', N'false'
GO

exec sp_dboption N'PTracker', N'ANSI nulls', N'false'
GO

exec sp_dboption N'PTracker', N'concat null yields null', N'false'
GO

exec sp_dboption N'PTracker', N'cursor close on commit', N'false'
GO

exec sp_dboption N'PTracker', N'default to local cursor', N'false'
GO

exec sp_dboption N'PTracker', N'quoted identifier', N'false'
GO

exec sp_dboption N'PTracker', N'ANSI warnings', N'false'
GO

exec sp_dboption N'PTracker', N'auto create statistics', N'true'
GO

exec sp_dboption N'PTracker', N'auto update statistics', N'true'
GO

use [PTracker]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Assignments_Projects]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT FK_Assignments_Projects
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Assignements_Resources]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT FK_Assignements_Resources
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_Assignements_Roles]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT FK_Assignements_Roles
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[addAssignment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[addAssignment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[addProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[addProject]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[addResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[addResource]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[deleteAssignment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[deleteAssignment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[deleteProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[deleteProject]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[deleteResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[deleteResource]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getProject]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getProjects]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getProjects]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getResource]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getResources]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getResources]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateAssignment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateAssignment]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateProject]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateResource]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Assignments]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Assignments]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Projects]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Projects]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Resources]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Resources]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Roles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Roles]
GO

CREATE TABLE [dbo].[Assignments] (
	[ProjectID] [uniqueidentifier] NOT NULL ,
	[ResourceID] [varchar] (10) NOT NULL ,
	[Assigned] [datetime] NOT NULL ,
	[Role] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Projects] (
	[ID] [uniqueidentifier] NOT NULL ,
	[Name] [varchar] (50) NOT NULL ,
	[Started] [datetime] NULL ,
	[Ended] [datetime] NULL ,
	[Description] [text] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Resources] (
	[ID] [varchar] (10) NOT NULL ,
	[LastName] [varchar] (50) NOT NULL ,
	[FirstName] [varchar] (50) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Roles] (
	[ID] [int] NOT NULL ,
	[Name] [varchar] (50) NOT NULL 
) ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE addAssignment
	(
		@ProjectID uniqueidentifier,
		@ResourceID varchar(10),
		@Assigned datetime,
		@Role int
	)
AS
	INSERT INTO Assignments 
	(ProjectID,ResourceID,Assigned,Role) 
	VALUES 
	(@ProjectID,@ResourceID,@Assigned,@Role)
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE addProject
	(
		@ID uniqueidentifier,
		@Name varchar(50),
		@Started datetime,
		@Ended datetime,
		@Description text
	)
AS
	INSERT INTO Projects 
	(ID,Name,Started,Ended,Description) 
	VALUES 
	(@ID,@Name,@Started,@Ended,@Description)
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE addResource
	(
		@ID varchar(10),
		@LastName varchar(50),
		@FirstName varchar(50)
	)
AS
	INSERT INTO Resources 
	(ID,LastName,FirstName) 
	VALUES 
	(@ID,@LastName,@FirstName)
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE deleteAssignment
	(
		@ProjectID uniqueidentifier,
		@ResourceID varchar(10)
	)
AS
	DELETE Assignments 
	WHERE ProjectID=@ProjectID AND ResourceID=@ResourceID
                        	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE deleteProject
	(
		@ID uniqueidentifier
	)
AS
	DELETE Assignments 
	WHERE ProjectID=@ID
	
	DELETE Projects 
	WHERE ID=@ID
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE deleteResource
	(
		@ID varchar(10)
	)
AS
	DELETE Assignments 
	WHERE ResourceID=@ID

	DELETE Resources 
	WHERE ID=@ID
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE getProject
	(
		@ID uniqueidentifier
	)
AS
	SELECT id,name,started,ended,description 
	FROM Projects 
	WHERE ID=@ID

	SELECT resourceid,lastname,firstname,assigned,role 
	FROM Resources,Assignments 
	WHERE ProjectID=@ID AND ResourceID=ID
	RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE getProjects
AS
	SELECT     ID, Name
	FROM         Projects
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE getResource
	(
		@ID varchar(10)
	)
AS
	SELECT id,lastname,firstname 
	FROM Resources 
	WHERE ID=@ID

	SELECT projectid,name,assigned,role 
	FROM Projects,Assignments 
	WHERE ResourceID=@ID AND ProjectID=ID
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE getResources
AS
	SELECT id,LastName,FirstName 
	FROM Resources
                        	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE updateAssignment
	(
		@ProjectID uniqueidentifier,
		@ResourceID varchar(10),
		@Assigned datetime,
		@Role int
	)
AS
	UPDATE Assignments 
	SET 
		Assigned=@Assigned,
		Role=@Role
	WHERE ProjectID=@ProjectID AND ResourceID=@ResourceID
            	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE updateProject
	(
		@ID uniqueidentifier,
		@Name varchar(50),
		@Started datetime,
		@Ended datetime,
		@Description text
	)
AS
	UPDATE Projects 
	SET 
		Name=@Name,
		Started=@Started,
		Ended=@Ended,
		Description=@Description
	WHERE ID=@ID
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE updateResource
	(
		@ID varchar(10),
		@LastName varchar(50),
		@FirstName varchar(50)
	)
AS
	UPDATE Resources 
	SET 
		LastName=@LastName,
		FirstName=@FirstName
	WHERE ID=@ID
	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

