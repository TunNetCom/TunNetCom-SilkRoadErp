CREATE TABLE [dbo].[LigneBL] (
    [Id_li]          INT             IDENTITY (1, 1) NOT NULL,
    [Num_BL]         INT             NOT NULL,
    [Ref_Produit]    NVARCHAR (50)   NOT NULL,
    [designation_li] NVARCHAR (MAX)  NOT NULL,
    [qte_li]         INT             NOT NULL,
    [prix_HT]        DECIMAL (18, 2) NOT NULL,
    [remise]         FLOAT (53)      NOT NULL,
    [tot_HT]         DECIMAL (18, 2) NOT NULL,
    [tva]            FLOAT (53)      NOT NULL,
    [tot_TTC]        DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.LigneBL] PRIMARY KEY CLUSTERED ([Id_li] ASC),
    CONSTRAINT [FK_dbo.LigneBL_dbo.BonDeLivraison_Num_BL] FOREIGN KEY ([Num_BL]) REFERENCES [dbo].[BonDeLivraison] ([Num]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.LigneBL_dbo.Produit_Ref_Produit] FOREIGN KEY ([Ref_Produit]) REFERENCES [dbo].[Produit] ([refe])
);

