SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getResource]
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

