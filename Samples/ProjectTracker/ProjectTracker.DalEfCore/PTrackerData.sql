CREATE TABLE [dbo].[Projects] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Started]     DATETIME      NULL,
    [Ended]       DATETIME      NULL,
    [Description] VARCHAR (MAX) NULL,
    [LastChanged] ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Resources] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [LastName]    VARCHAR (50) NOT NULL,
    [FirstName]   VARCHAR (50) NOT NULL,
    [LastChanged] ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Resources] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Roles] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [LastChanged] ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TABLE [dbo].[Assignments] (
    [ProjectId]   INT        NOT NULL,
    [ResourceId]  INT        NOT NULL,
    [Assigned]    DATETIME   NOT NULL,
    [RoleId]      INT        NOT NULL,
    [LastChanged] ROWVERSION NOT NULL,
    CONSTRAINT [PK_Assignments] PRIMARY KEY CLUSTERED ([ProjectId] ASC, [ResourceId] ASC),
    CONSTRAINT [FK_Assignment_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([Id]),
    CONSTRAINT [FK_Assignment_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resources] ([Id]),
    CONSTRAINT [FK_Assignment_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_Assignment_Resource]
    ON [dbo].[Assignments]([ResourceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_Assignment_Role]
    ON [dbo].[Assignments]([RoleId] ASC);

GO

