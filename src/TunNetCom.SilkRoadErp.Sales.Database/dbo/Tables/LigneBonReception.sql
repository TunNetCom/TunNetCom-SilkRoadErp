CREATE TABLE [dbo].[LigneBonReception] (
    [Id_ligne]       INT             IDENTITY (1, 1) NOT NULL,
    [Num_BonRec]     INT             NOT NULL,
    [Ref_Produit]    NVARCHAR (50)   NOT NULL,
    [designation_li] NVARCHAR (MAX)  NOT NULL,
    [qte_li]         INT             NOT NULL,
    [prix_HT]        DECIMAL (18, 2) NOT NULL,
    [remise]         FLOAT (53)      NOT NULL,
    [tot_HT]         DECIMAL (18, 2) NOT NULL,
    [tva]            FLOAT (53)      NOT NULL,
    [tot_TTC]        DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.LigneBonReception] PRIMARY KEY CLUSTERED ([Id_ligne] ASC),
    CONSTRAINT [FK_dbo.LigneBonReception_dbo.BonDeReception_Num_BonRec] FOREIGN KEY ([Num_BonRec]) REFERENCES [dbo].[BonDeReception] ([Num]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.LigneBonReception_dbo.Produit_Ref_Produit] FOREIGN KEY ([Ref_Produit]) REFERENCES [dbo].[Produit] ([refe])
);

