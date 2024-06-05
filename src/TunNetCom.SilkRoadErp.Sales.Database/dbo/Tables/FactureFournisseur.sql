CREATE TABLE [dbo].[FactureFournisseur] (
    [Num]                        INT      IDENTITY (1, 1) NOT NULL,
    [id_fournisseur]             INT      NOT NULL,
    [paye]                       BIT      NOT NULL,
    [NumFactureFournisseur]      BIGINT   NOT NULL,
    [dateFacturationFournisseur] DATETIME NOT NULL,
    [date]                       DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.FactureFournisseur] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.FactureFournisseur_dbo.Fournisseur_id_fournisseur] FOREIGN KEY ([id_fournisseur]) REFERENCES [dbo].[Fournisseur] ([id])
);

