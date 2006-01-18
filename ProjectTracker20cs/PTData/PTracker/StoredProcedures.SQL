/****** Object:  StoredProcedure [dbo].[addAssignment]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addAssignment]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[addAssignment]

GO

/****** Object:  StoredProcedure [dbo].[updateRole]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateRole]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[updateRole]

GO

/****** Object:  StoredProcedure [dbo].[addProject]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addProject]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[addProject]

GO

/****** Object:  StoredProcedure [dbo].[addResource]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addResource]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[addResource]

GO

/****** Object:  StoredProcedure [dbo].[addRole]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addRole]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[addRole]

GO

/****** Object:  StoredProcedure [dbo].[deleteAssignment]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteAssignment]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[deleteAssignment]

GO

/****** Object:  StoredProcedure [dbo].[deleteProject]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteProject]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[deleteProject]

GO

/****** Object:  StoredProcedure [dbo].[deleteResource]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteResource]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[deleteResource]

GO

/****** Object:  StoredProcedure [dbo].[deleteRole]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteRole]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[deleteRole]

GO

/****** Object:  StoredProcedure [dbo].[existsProject]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[existsProject]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[existsProject]

GO

/****** Object:  StoredProcedure [dbo].[existsResource]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[existsResource]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[existsResource]

GO

/****** Object:  StoredProcedure [dbo].[getProject]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getProject]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[getProject]

GO

/****** Object:  StoredProcedure [dbo].[getProjects]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getProjects]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[getProjects]

GO

/****** Object:  StoredProcedure [dbo].[getResource]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getResource]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[getResource]

GO

/****** Object:  StoredProcedure [dbo].[getResources]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getResources]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[getResources]

GO

/****** Object:  StoredProcedure [dbo].[getRoles]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getRoles]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[getRoles]

GO

/****** Object:  StoredProcedure [dbo].[updateAssignment]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateAssignment]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[updateAssignment]

GO

/****** Object:  StoredProcedure [dbo].[updateProject]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateProject]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[updateProject]

GO

/****** Object:  StoredProcedure [dbo].[updateResource]    Script Date: 01/17/2006 21:24:05 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateResource]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[updateResource]

GO

/****** Object:  StoredProcedure [dbo].[addAssignment]    Script Date: 01/17/2006 21:24:05 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addAssignment]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE addAssignment

	(

		@projectID uniqueidentifier,

		@resourceID varchar(10),

		@assigned datetime,

		@role int,

		@newLastChanged timestamp output

	)

AS

	INSERT INTO Assignments 

	(ProjectId,ResourceId,Assigned,Role) 

	VALUES 

	(@projectId,@resourceId,@assigned,@role)

	

	SELECT @newLastChanged = LastChanged 

  FROM Assignments 

  WHERE ProjectId=@projectId AND ResourceId=@resourceId

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[updateRole]    Script Date: 01/17/2006 21:24:06 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateRole]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[updateRole]

	(

		@id int,

		@name varchar(50),

		@lastChanged timestamp,

		@newLastChanged timestamp output

	)

AS

	UPDATE Roles 

	SET 

		Name=@name

	WHERE Id=@id

	  AND LastChanged=@lastChanged

  IF @@ROWCOUNT = 0

    RAISERROR(''Row has been edited by another user'', 16, 1)            	



	SELECT @newLastChanged = LastChanged 

	FROM Roles WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[addProject]    Script Date: 01/17/2006 21:24:06 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE addProject

	(

		@id uniqueidentifier,

		@name varchar(50),

		@started datetime,

		@ended datetime,

		@description text,

		@newLastChanged timestamp output

	)

AS

	INSERT INTO Projects 

	(Id,Name,Started,Ended,Description) 

	VALUES 

	(@id,@name,@started,@ended,@description)



	SELECT @newLastChanged = LastChanged 

	FROM Projects WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[addResource]    Script Date: 01/17/2006 21:24:07 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE addResource

	(

		@lastName varchar(50),

		@firstName varchar(50),

		@newId int output,

		@newLastChanged timestamp output

	)

AS

	INSERT INTO Resources 

	(LastName,FirstName) 

	VALUES 

	(@lastName,@firstName)

	

	SELECT @newId = Id, @newLastChanged = LastChanged 

	FROM Resources WHERE Id=SCOPE_IDENTITY()

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[addRole]    Script Date: 01/17/2006 21:24:08 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[addRole]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[addRole]

	(

		@id int,

		@name varchar(50),

		@newLastChanged timestamp output

	)

AS

	INSERT INTO Roles 

	(Id,Name) 

	VALUES 

	(@id,@name)



	SELECT @newLastChanged = LastChanged 

	FROM Roles WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[deleteAssignment]    Script Date: 01/17/2006 21:24:08 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteAssignment]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE deleteAssignment

	(

		@projectId uniqueidentifier,

		@resourceId int

	)

AS

	DELETE Assignments 

	WHERE ProjectId=@projectId AND ResourceId=@resourceId

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[deleteProject]    Script Date: 01/17/2006 21:24:08 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE deleteProject

	(

		@id uniqueidentifier

	)

AS

	DELETE Assignments 

	WHERE ProjectId=@id

	

	DELETE Projects 

	WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[deleteResource]    Script Date: 01/17/2006 21:24:09 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE deleteResource

	(

		@id int

	)

AS

	DELETE Assignments 

	WHERE ResourceId=@id



	DELETE Resources 

	WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[deleteRole]    Script Date: 01/17/2006 21:24:09 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteRole]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[deleteRole]

	(

		@id int

	)

AS

	DELETE Roles 

	WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[existsProject]    Script Date: 01/17/2006 21:24:10 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[existsProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE dbo.existsProject

	(

	@id uniqueidentifier

	)

AS

	SELECT COUNT(*) FROM Projects WHERE Id=@id

	RETURN

' 

END

GO

/****** Object:  StoredProcedure [dbo].[existsResource]    Script Date: 01/17/2006 21:24:10 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[existsResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE dbo.existsResource

	(

	@id int

	)

AS

	SELECT COUNT(*) FROM Resources WHERE Id=@id

	RETURN



' 

END

GO

/****** Object:  StoredProcedure [dbo].[getProject]    Script Date: 01/17/2006 21:24:10 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getProject

	(

		@id uniqueidentifier

	)

AS

	SELECT Id,Name,Started,Ended,Description,LastChanged

	FROM Projects 

	WHERE Id=@id



	SELECT ResourceId,LastName,FirstName,Assigned,Role,Assignments.LastChanged AS LastChanged

	FROM Resources,Assignments 

	WHERE ProjectId=@id AND ResourceId=Id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[getProjects]    Script Date: 01/17/2006 21:24:11 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getProjects]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getProjects

AS

	SELECT     Id, Name

	FROM         Projects

	RETURN ' 

END

GO

/****** Object:  StoredProcedure [dbo].[getResource]    Script Date: 01/17/2006 21:24:11 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getResource

	(

		@id int

	)

AS

	SELECT Id,LastName,FirstName,LastChanged 

	FROM Resources 

	WHERE Id=@id



	SELECT ProjectId,Name,Assigned,Role,Assignments.LastChanged AS LastChanged

	FROM Projects,Assignments 

	WHERE ResourceId=@id AND ProjectId=Id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[getResources]    Script Date: 01/17/2006 21:24:11 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getResources]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getResources

AS

	SELECT Id,LastName,FirstName 

	FROM Resources

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[getRoles]    Script Date: 01/17/2006 21:24:12 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getRoles]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[getRoles]

AS

	SELECT Id,Name,LastChanged

	FROM Roles

	RETURN ' 

END

GO

/****** Object:  StoredProcedure [dbo].[updateAssignment]    Script Date: 01/17/2006 21:24:12 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateAssignment]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE updateAssignment

	(

		@projectId uniqueidentifier,

		@resourceId int,

		@assigned datetime,

		@role int,

		@lastChanged timestamp,

		@newLastChanged timestamp output

	)

AS

	UPDATE Assignments 

	SET 

		Assigned=@assigned,

		Role=@role

	WHERE ProjectId=@projectId AND ResourceId=@resourceId 

	  AND LastChanged=@lastChanged

  IF @@ROWCOUNT = 0

    RAISERROR(''Row has been edited by another user'', 16, 1)            	



	SELECT @newLastChanged = LastChanged 

	FROM Assignments 

	WHERE ProjectId=@projectId AND ResourceId=@resourceId

	RETURN 

' 

END

GO

/****** Object:  StoredProcedure [dbo].[updateProject]    Script Date: 01/17/2006 21:24:13 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE updateProject

	(

		@id uniqueidentifier,

		@name varchar(50),

		@started datetime,

		@ended datetime,

		@description text,

		@lastChanged timestamp,

		@newLastChanged timestamp output

	)

AS

	UPDATE Projects 

	SET 

		Name=@name,

		Started=@started,

		Ended=@ended,

		Description=@description

	WHERE Id=@id 

	  AND LastChanged=@lastChanged

  IF @@ROWCOUNT = 0

    RAISERROR(''Row has been edited by another user'', 16, 1)            	



	SELECT @newLastChanged = LastChanged 

	FROM Projects WHERE Id=@id

	RETURN 

' 

END

GO

/****** Object:  StoredProcedure [dbo].[updateResource]    Script Date: 01/17/2006 21:24:13 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE updateResource

	(

		@id int,

		@lastName varchar(50),

		@firstName varchar(50),

		@lastChanged timestamp,

		@newLastChanged timestamp output

	)

AS

	UPDATE Resources 

	SET 

		LastName=@lastName,

		FirstName=@firstName

	WHERE Id=@id

	  AND LastChanged=@lastChanged

  IF @@ROWCOUNT = 0

    RAISERROR(''Row has been edited by another user'', 16, 1)            	

	

	SELECT @newLastChanged = LastChanged 

	FROM Resources WHERE Id=@id

	RETURN' 

END

GO

	(

		@id int

	)

AS

	DELETE Assignments 

	WHERE ResourceId=@id


	DELETE Resources 

	WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[deleteRole]    Script Date: 01/18/2006 09:28:52 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[deleteRole]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[deleteRole]

	(

		@id int

	)

AS

	DELETE Roles 

	WHERE Id=@id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[existsProject]    Script Date: 01/18/2006 09:28:53 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[existsProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE dbo.existsProject

	(

	@id uniqueidentifier

	)

AS

	SELECT COUNT(*) 

	FROM Projects WHERE Id=@id

	RETURN

' 

END

GO

/****** Object:  StoredProcedure [dbo].[existsResource]    Script Date: 01/18/2006 09:28:53 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[existsResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE dbo.existsResource

	(

	@id int

	)

AS

	SELECT COUNT(*) 

	FROM Resources WHERE Id=@id

	RETURN


' 

END

GO

/****** Object:  StoredProcedure [dbo].[getProject]    Script Date: 01/18/2006 09:28:54 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getProject

	(

		@id uniqueidentifier

	)

AS

	SELECT Id,Name,Started,Ended,Description,LastChanged

	FROM Projects 

	WHERE Id=@id


	SELECT ResourceId,LastName,FirstName,Assigned,Role,Assignments.LastChanged AS LastChanged

	FROM Resources,Assignments 

	WHERE ProjectId=@id AND ResourceId=Id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[getProjects]    Script Date: 01/18/2006 09:28:54 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getProjects]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getProjects

AS

	SELECT     Id, Name

	FROM         Projects

	RETURN ' 

END

GO

/****** Object:  StoredProcedure [dbo].[getResource]    Script Date: 01/18/2006 09:28:54 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getResource

	(

		@id int

	)

AS

	SELECT Id,LastName,FirstName,LastChanged 

	FROM Resources 

	WHERE Id=@id


	SELECT ProjectId,Name,Assigned,Role,Assignments.LastChanged AS LastChanged

	FROM Projects,Assignments 

	WHERE ResourceId=@id AND ProjectId=Id

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[getResources]    Script Date: 01/18/2006 09:28:55 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getResources]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE getResources

AS

	SELECT Id,LastName,FirstName 

	FROM Resources

	RETURN' 

END

GO

/****** Object:  StoredProcedure [dbo].[getRoles]    Script Date: 01/18/2006 09:28:55 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[getRoles]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[getRoles]

AS

	SELECT Id,Name,LastChanged

	FROM Roles

	RETURN ' 

END

GO

/****** Object:  StoredProcedure [dbo].[updateAssignment]    Script Date: 01/18/2006 09:28:55 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateAssignment]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE updateAssignment

	(

		@projectId uniqueidentifier,

		@resourceId int,

		@assigned datetime,

		@role int,

		@lastChanged timestamp,

		@newLastChanged timestamp output

	)

AS

	UPDATE Assignments 

	SET 

		Assigned=@assigned,

		Role=@role

	WHERE ProjectId=@projectId AND ResourceId=@resourceId 

	  AND LastChanged=@lastChanged

  IF @@ROWCOUNT = 0

    RAISERROR(''Row has been edited by another user'', 16, 1)            	


	SELECT @newLastChanged = LastChanged 

	FROM Assignments 

	WHERE ProjectId=@projectId AND ResourceId=@resourceId

	RETURN 

' 

END

GO

/****** Object:  StoredProcedure [dbo].[updateProject]    Script Date: 01/18/2006 09:28:56 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateProject]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE updateProject

	(

		@id uniqueidentifier,

		@name varchar(50),

		@started datetime,

		@ended datetime,

		@description varchar(MAX),

		@lastChanged timestamp,

		@newLastChanged timestamp output

	)

AS

	UPDATE Projects 

	SET 

		Name=@name,

		Started=@started,

		Ended=@ended,

		Description=@description

	WHERE Id=@id 

	  AND LastChanged=@lastChanged

  IF @@ROWCOUNT = 0

    RAISERROR(''Row has been edited by another user'', 16, 1)            	


	SELECT @newLastChanged = LastChanged 

	FROM Projects WHERE Id=@id

	RETURN 

' 

END

GO

/****** Object:  StoredProcedure [dbo].[updateResource]    Script Date: 01/18/2006 09:28:57 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[updateResource]') AND type in (N'P', N'PC'))

BEGIN

EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE updateResource

	(

		@id int,

		@lastName varchar(50),

		@firstName varchar(50),

		@lastChanged timestamp,

		@newLastChanged timestamp output

	)

AS

	UPDATE Resources 

	SET 

		LastName=@lastName,

		FirstName=@firstName

	WHERE Id=@id

	  AND LastChanged=@lastChanged

  IF @@ROWCOUNT = 0

    RAISERROR(''Row has been edited by another user'', 16, 1)            	

	

	SELECT @newLastChanged = LastChanged 

	FROM Resources WHERE Id=@id

	RETURN' 

END

GO

