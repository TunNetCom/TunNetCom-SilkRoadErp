CREATE TABLE [dbo].[AvoirFournisseur] (
    [Num]                         INT      IDENTITY (1, 1) NOT NULL,
    [date]                        DATETIME NOT NULL,
    [fournisseurId]               INT      NULL,
    [Num_FactureAvoirFournisseur] INT      NULL,
    [Num_AvoirFournisseur]        INT      NOT NULL,
    CONSTRAINT [PK_dbo.AvoirFournisseur] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.AvoirFournisseur_dbo.FactureAvoirFournisseur_Num_FactureAvoirFournisseur] FOREIGN KEY ([Num_FactureAvoirFournisseur]) REFERENCES [dbo].[FactureAvoirFournisseur] ([Num]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AvoirFournisseur_dbo.Fournisseur_fournisseurId] FOREIGN KEY ([fournisseurId]) REFERENCES [dbo].[Fournisseur] ([id])
);

