SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[getResources]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[getResources]
GO

CREATE PROCEDURE getResources
AS
	SELECT id,LastName,FirstName 
	FROM Resources
                        	
	RETURN 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

