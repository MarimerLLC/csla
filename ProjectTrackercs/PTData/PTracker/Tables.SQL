IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Assignments_Projects]') AND parent_object_id = OBJECT_ID(N'[dbo].[Assignments]'))

ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT [FK_Assignments_Projects]

GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Assignments_Resources]') AND parent_object_id = OBJECT_ID(N'[dbo].[Assignments]'))

ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT [FK_Assignments_Resources]

GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Assignments_Roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[Assignments]'))

ALTER TABLE [dbo].[Assignments] DROP CONSTRAINT [FK_Assignments_Roles]

GO

/****** Object:  Table [dbo].[Assignments]    Script Date: 12/06/2005 11:11:16 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Assignments]') AND type in (N'U'))

DROP TABLE [dbo].[Assignments]

GO

/****** Object:  Table [dbo].[Roles]    Script Date: 12/06/2005 11:11:16 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))

DROP TABLE [dbo].[Roles]

GO

/****** Object:  Table [dbo].[Projects]    Script Date: 12/06/2005 11:11:16 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Projects]') AND type in (N'U'))

DROP TABLE [dbo].[Projects]

GO

/****** Object:  Table [dbo].[Resources]    Script Date: 12/06/2005 11:11:16 ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Resources]') AND type in (N'U'))

DROP TABLE [dbo].[Resources]

GO

/****** Object:  Table [dbo].[Assignments]    Script Date: 12/06/2005 11:11:16 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Assignments]') AND type in (N'U'))

BEGIN

CREATE TABLE [dbo].[Assignments](

	[ProjectId] [uniqueidentifier] NOT NULL,

	[ResourceId] [int] NOT NULL,

	[Assigned] [datetime] NOT NULL,

	[Role] [int] NOT NULL,

	[LastChanged] [timestamp] NOT NULL,

 CONSTRAINT [PK_Assignments] PRIMARY KEY CLUSTERED 

(

	[ProjectId] ASC,

	[ResourceId] ASC

)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]

END

GO

/****** Object:  Table [dbo].[Roles]    Script Date: 12/06/2005 11:11:16 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

SET ANSI_PADDING ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))

BEGIN

CREATE TABLE [dbo].[Roles](

	[Id] [int] NOT NULL,

	[Name] [varchar](50) NOT NULL,

	[LastChanged] [timestamp] NOT NULL,

 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 

(

	[Id] ASC

)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]

END

GO

SET ANSI_PADDING OFF

GO

/****** Object:  Table [dbo].[Projects]    Script Date: 12/06/2005 11:11:16 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

SET ANSI_PADDING ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Projects]') AND type in (N'U'))

BEGIN

CREATE TABLE [dbo].[Projects](

	[Id] [uniqueidentifier] NOT NULL,

	[Name] [varchar](50) NOT NULL,

	[Started] [datetime] NULL,

	[Ended] [datetime] NULL,

	[Description] [varchar](max) NULL,

	[LastChanged] [timestamp] NOT NULL,

 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 

(

	[Id] ASC

)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]

END

GO

SET ANSI_PADDING OFF

GO

/****** Object:  Table [dbo].[Resources]    Script Date: 12/06/2005 11:11:16 ******/

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

SET ANSI_PADDING ON

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Resources]') AND type in (N'U'))

BEGIN

CREATE TABLE [dbo].[Resources](

	[Id] [int] IDENTITY(1,1) NOT NULL,

	[LastName] [varchar](50) NULL,

	[FirstName] [varchar](50) NULL,

	[LastChanged] [timestamp] NOT NULL,

 CONSTRAINT [PK_Resources] PRIMARY KEY CLUSTERED 

(

	[Id] ASC

)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]

END

GO

SET ANSI_PADDING OFF

GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Assignments_Projects]') AND parent_object_id = OBJECT_ID(N'[dbo].[Assignments]'))

ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD  CONSTRAINT [FK_Assignments_Projects] FOREIGN KEY([ProjectId])

REFERENCES [dbo].[Projects] ([Id])

GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Assignments_Resources]') AND parent_object_id = OBJECT_ID(N'[dbo].[Assignments]'))

ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD  CONSTRAINT [FK_Assignments_Resources] FOREIGN KEY([ResourceId])

REFERENCES [dbo].[Resources] ([Id])

GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Assignments_Roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[Assignments]'))

ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD  CONSTRAINT [FK_Assignments_Roles] FOREIGN KEY([Role])

REFERENCES [dbo].[Roles] ([Id])

GO

