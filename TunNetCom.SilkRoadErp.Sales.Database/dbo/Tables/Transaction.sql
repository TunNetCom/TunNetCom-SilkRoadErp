CREATE TABLE [dbo].[Transaction] (
    [Num_BL]  INT             NOT NULL,
    [type]    INT             NOT NULL,
    [date_tr] DATETIME        NOT NULL,
    [montant] DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.Transaction] PRIMARY KEY CLUSTERED ([Num_BL] ASC),
    CONSTRAINT [FK_dbo.Transaction_dbo.BonDeLivraison_Num_BL] FOREIGN KEY ([Num_BL]) REFERENCES [dbo].[BonDeLivraison] ([Num])
);

