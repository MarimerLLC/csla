IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Roles_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[Roles]'))

ALTER TABLE [dbo].[Roles] DROP CONSTRAINT [FK_Roles_Users]

GO

/****** Object:  Table [dbo].[Roles]    Script Date: 12/06/2005 11:21:00 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))

DROP TABLE [dbo].[Roles]

GO

/****** Object:  Table [dbo].[Users]    Script Date: 12/06/2005 11:21:00 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))

DROP TABLE [dbo].[Users]

GO

/****** Object:  Table [dbo].[Roles]    Script Date: 12/06/2005 11:21:00 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

SET ANSI_PADDING ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))

BEGIN

CREATE TABLE [dbo].[Roles](

	[Username] [varchar](20) NOT NULL,

	[Role] [varchar](20) NOT NULL,

 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 

(

	[Username] ASC,

	[Role] ASC

)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]

END

GO

SET ANSI_PADDING OFF

GO

/****** Object:  Table [dbo].[Users]    Script Date: 12/06/2005 11:21:00 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

SET ANSI_PADDING ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))

BEGIN

CREATE TABLE [dbo].[Users](

	[Username] [varchar](20) NOT NULL,

	[Password] [varchar](20) NULL,

 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 

(

	[Username] ASC

)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]

END

GO

SET ANSI_PADDING OFF

GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Roles_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[Roles]'))

ALTER TABLE [dbo].[Roles]  WITH CHECK ADD  CONSTRAINT [FK_Roles_Users] FOREIGN KEY([Username])

REFERENCES [dbo].[Users] ([Username])

GO

