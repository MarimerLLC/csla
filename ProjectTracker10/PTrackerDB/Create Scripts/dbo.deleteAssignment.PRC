SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[deleteAssignment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[deleteAssignment]
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

