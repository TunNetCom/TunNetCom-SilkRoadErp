CREATE TABLE [dbo].[BonDeReception] (
    [Num]                     INT      IDENTITY (1, 1) NOT NULL,
    [Num_Bon_fournisseur]     BIGINT   NOT NULL,
    [date_livraison]          DATETIME NOT NULL,
    [id_fournisseur]          INT      NOT NULL,
    [date]                    DATETIME NOT NULL,
    [Num_Facture_fournisseur] INT      NULL,
    CONSTRAINT [PK_dbo.BonDeReception] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.BonDeReception_dbo.FactureFournisseur_Num_Facture_fournisseur] FOREIGN KEY ([Num_Facture_fournisseur]) REFERENCES [dbo].[FactureFournisseur] ([Num]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.BonDeReception_dbo.Fournisseur_id_fournisseur] FOREIGN KEY ([id_fournisseur]) REFERENCES [dbo].[Fournisseur] ([id])
);

