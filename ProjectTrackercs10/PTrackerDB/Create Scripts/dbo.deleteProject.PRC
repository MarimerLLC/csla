SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[deleteProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[deleteProject]
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

