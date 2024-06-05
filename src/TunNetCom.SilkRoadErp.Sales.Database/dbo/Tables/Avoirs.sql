CREATE TABLE [dbo].[Avoirs] (
    [Num]      INT      IDENTITY (1, 1) NOT NULL,
    [date]     DATETIME NOT NULL,
    [clientId] INT      NULL,
    CONSTRAINT [PK_dbo.Avoirs] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.Avoirs_dbo.Client_clientId] FOREIGN KEY ([clientId]) REFERENCES [dbo].[Client] ([Id])
);

