SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateProject]
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

