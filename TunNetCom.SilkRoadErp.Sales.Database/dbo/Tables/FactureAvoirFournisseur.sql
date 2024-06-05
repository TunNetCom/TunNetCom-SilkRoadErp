CREATE TABLE [dbo].[FactureAvoirFournisseur] (
    [Num]                         INT      IDENTITY (1, 1) NOT NULL,
    [Num_FactureAvoirFourSurPAge] INT      NOT NULL,
    [id_fournisseur]              INT      NOT NULL,
    [date]                        DATETIME NOT NULL,
    [Num_FactureFournisseur]      INT      NULL,
    CONSTRAINT [PK_dbo.FactureAvoirFournisseur] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.FactureAvoirFournisseur_dbo.FactureFournisseur_Num_FactureFournisseur] FOREIGN KEY ([Num_FactureFournisseur]) REFERENCES [dbo].[FactureFournisseur] ([Num]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.FactureAvoirFournisseur_dbo.Fournisseur_id_fournisseur] FOREIGN KEY ([id_fournisseur]) REFERENCES [dbo].[Fournisseur] ([id])
);

