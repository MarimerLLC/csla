SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getProjects]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getProjects]
GO

CREATE PROCEDURE getProjects
AS
	SELECT     ID, Name
	FROM         Projects
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

