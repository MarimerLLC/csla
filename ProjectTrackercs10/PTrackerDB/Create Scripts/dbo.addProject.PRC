SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[addProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[addProject]
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

