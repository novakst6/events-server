CREATE TABLE [dbo].[Event] (
    [Id]       VARCHAR (50) DEFAULT (newid()) NOT NULL,
    [Name]     VARCHAR (50) NOT NULL,
    [FromDate] DATETIME     NOT NULL,
    [ToDate]   DATETIME     NOT NULL,
    [Created]  DATETIME     DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);