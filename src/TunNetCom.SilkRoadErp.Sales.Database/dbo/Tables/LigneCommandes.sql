CREATE TABLE [dbo].[LigneCommandes] (
    [Id_li]          INT             IDENTITY (1, 1) NOT NULL,
    [Num_commande]   INT             NOT NULL,
    [Ref_Produit]    NVARCHAR (50)   NOT NULL,
    [designation_li] NVARCHAR (MAX)  NULL,
    [qte_li]         INT             NOT NULL,
    [prix_HT]        DECIMAL (18, 2) NOT NULL,
    [remise]         FLOAT (53)      NOT NULL,
    [tot_HT]         DECIMAL (18, 2) NOT NULL,
    [tva]            FLOAT (53)      NOT NULL,
    [tot_TTC]        DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.LigneCommandes] PRIMARY KEY CLUSTERED ([Id_li] ASC),
    CONSTRAINT [FK_dbo.LigneCommandes_dbo.Commandes_Num_commande] FOREIGN KEY ([Num_commande]) REFERENCES [dbo].[Commandes] ([Num]),
    CONSTRAINT [FK_dbo.LigneCommandes_dbo.Produit_Ref_Produit] FOREIGN KEY ([Ref_Produit]) REFERENCES [dbo].[Produit] ([refe])
);

