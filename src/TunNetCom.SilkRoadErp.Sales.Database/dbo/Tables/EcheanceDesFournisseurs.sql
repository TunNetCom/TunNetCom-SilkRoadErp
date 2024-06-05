CREATE TABLE [dbo].[EcheanceDesFournisseurs] (
    [id]             INT             IDENTITY (1, 1) NOT NULL,
    [dateEcheance]   DATETIME        NOT NULL,
    [numCheque]      BIGINT          NOT NULL,
    [montant]        DECIMAL (18, 2) NOT NULL,
    [fournisseur_id] INT             NOT NULL,
    CONSTRAINT [PK_dbo.EcheanceDesFournisseurs] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_dbo.EcheanceDesFournisseurs_dbo.Fournisseur_fournisseur_id] FOREIGN KEY ([fournisseur_id]) REFERENCES [dbo].[Fournisseur] ([id])
);

