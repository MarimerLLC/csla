SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getProject]
GO

CREATE PROCEDURE getProject
	(
		@ID uniqueidentifier
	)
AS
	SELECT id,name,started,ended,description 
	FROM Projects 
	WHERE ID=@ID

	SELECT resourceid,lastname,firstname,assigned,role 
	FROM Resources,Assignments 
	WHERE ProjectID=@ID AND ResourceID=ID
	RETURN
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

