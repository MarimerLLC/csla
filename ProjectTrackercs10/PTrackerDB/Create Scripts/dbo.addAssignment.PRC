SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[addAssignment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[addAssignment]
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

