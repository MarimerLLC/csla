SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Login]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Login]
GO

CREATE PROCEDURE Login
	(
		@User varchar(20),
		@pw varchar(20)
	)
AS
	SELECT Username 
	FROM Users 
	WHERE Username=@User AND Password=@pw;

	SELECT R.Role 
	FROM Users AS U INNER JOIN Roles AS R ON
      R.UserName = U.UserName
	WHERE U.Username = @User and U.Password = @pw
	
	RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

