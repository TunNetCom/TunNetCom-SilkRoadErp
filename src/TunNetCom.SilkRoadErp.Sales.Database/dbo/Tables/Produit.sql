CREATE TABLE [dbo].[Produit] (
    [refe]        NVARCHAR (50)   NOT NULL,
    [nom]         NVARCHAR (MAX)  NOT NULL,
    [qte]         INT             NOT NULL,
    [qteLimite]   INT             NOT NULL,
    [remise]      FLOAT (53)      NOT NULL,
    [remiseAchat] FLOAT (53)      NOT NULL,
    [TVA]         FLOAT (53)      NOT NULL,
    [prix]        DECIMAL (18, 2) NOT NULL,
    [prixAchat]   DECIMAL (18, 2) NOT NULL,
    [visibilite]  BIT             NOT NULL,
    CONSTRAINT [PK_dbo.Produit] PRIMARY KEY CLUSTERED ([refe] ASC)
);

