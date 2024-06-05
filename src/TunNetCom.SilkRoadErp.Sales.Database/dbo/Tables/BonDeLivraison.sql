CREATE TABLE [dbo].[BonDeLivraison] (
    [Num]         INT             IDENTITY (1, 1) NOT NULL,
    [date]        DATETIME        NOT NULL,
    [tot_H_tva]   DECIMAL (18, 2) NOT NULL,
    [tot_tva]     DECIMAL (18, 2) NOT NULL,
    [net_payer]   DECIMAL (18, 2) NOT NULL,
    [temp_bl]     TIME (7)        NOT NULL,
    [Num_Facture] INT             NULL,
    [clientId]    INT             NULL,
    CONSTRAINT [PK_dbo.BonDeLivraison] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.BonDeLivraison_dbo.Client_clientId] FOREIGN KEY ([clientId]) REFERENCES [dbo].[Client] ([Id]),
    CONSTRAINT [FK_dbo.BonDeLivraison_dbo.Facture_Num_Facture] FOREIGN KEY ([Num_Facture]) REFERENCES [dbo].[Facture] ([Num]) ON DELETE CASCADE
);

