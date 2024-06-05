CREATE TABLE [dbo].[LigneDevis] (
    [Id_li]          INT             IDENTITY (1, 1) NOT NULL,
    [Num_devis]      INT             NOT NULL,
    [Ref_produit]    NVARCHAR (50)   NOT NULL,
    [Designation_li] NVARCHAR (MAX)  NOT NULL,
    [qte_li]         INT             NOT NULL,
    [prix_HT]        DECIMAL (18, 2) NOT NULL,
    [remise]         FLOAT (53)      NOT NULL,
    [tot_HT]         DECIMAL (18, 2) NOT NULL,
    [tva]            FLOAT (53)      NOT NULL,
    [tot_TTC]        DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.LigneDevis] PRIMARY KEY CLUSTERED ([Id_li] ASC),
    CONSTRAINT [FK_dbo.LigneDevis_dbo.Devis_Num_devis] FOREIGN KEY ([Num_devis]) REFERENCES [dbo].[Devis] ([Num]),
    CONSTRAINT [FK_dbo.LigneDevis_dbo.Produit_Ref_produit] FOREIGN KEY ([Ref_produit]) REFERENCES [dbo].[Produit] ([refe])
);

