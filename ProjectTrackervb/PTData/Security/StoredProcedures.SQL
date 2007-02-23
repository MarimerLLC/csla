/****** Object:  StoredProcedure [dbo].[Login]    Script Date: 12/06/2005 11:21:14 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Login]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[Login]

GO

/****** Object:  StoredProcedure [dbo].[Login]    Script Date: 12/06/2005 11:21:14 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Login]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE Login

	(

		@user varchar(20),

		@pw varchar(20)

	)

AS

	SELECT Username 

	FROM Users 

	WHERE Username=@user AND Password=@pw;



	SELECT R.Role 

	FROM Users AS U INNER JOIN Roles AS R ON

      R.UserName = U.UserName

	WHERE U.Username = @user and U.Password = @pw

	RETURN' 

END

GO

