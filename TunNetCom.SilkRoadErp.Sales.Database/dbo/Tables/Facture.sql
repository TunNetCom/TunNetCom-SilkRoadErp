CREATE TABLE [dbo].[Facture] (
    [Num]       INT      IDENTITY (1, 1) NOT NULL,
    [id_client] INT      NOT NULL,
    [date]      DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.Facture] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.Facture_dbo.Client_id_client] FOREIGN KEY ([id_client]) REFERENCES [dbo].[Client] ([Id])
);

