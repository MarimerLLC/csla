SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[addResource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[addResource]
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

