SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateAssignment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateAssignment]
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

