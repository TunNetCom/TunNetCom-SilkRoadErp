CREATE TABLE [dbo].[AvoirFinancierFournisseurs] (
    [Num]         INT             NOT NULL,
    [NumSurPage]  INT             NOT NULL,
    [date]        DATETIME        NOT NULL,
    [Description] NVARCHAR (MAX)  NULL,
    [tot_ttc]     DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.AvoirFinancierFournisseurs] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.AvoirFinancierFournisseurs_dbo.FactureFournisseur_Num] FOREIGN KEY ([Num]) REFERENCES [dbo].[FactureFournisseur] ([Num]) ON DELETE CASCADE
);

