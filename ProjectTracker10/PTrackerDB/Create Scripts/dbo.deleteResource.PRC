SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[deleteResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[deleteResource]
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

