SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateResource]
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

