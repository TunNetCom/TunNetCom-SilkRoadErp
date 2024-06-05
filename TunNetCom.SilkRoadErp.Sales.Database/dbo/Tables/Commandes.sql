CREATE TABLE [dbo].[Commandes] (
    [Num]           INT      IDENTITY (1, 1) NOT NULL,
    [date]          DATETIME NOT NULL,
    [fournisseurId] INT      NULL,
    CONSTRAINT [PK_dbo.Commandes] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.Commandes_dbo.Fournisseur_fournisseurId] FOREIGN KEY ([fournisseurId]) REFERENCES [dbo].[Fournisseur] ([id])
);

