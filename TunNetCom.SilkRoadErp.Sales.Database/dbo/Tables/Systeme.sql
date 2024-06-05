CREATE TABLE [dbo].[Systeme] (
    [Id]                INT             NOT NULL,
    [NomSociete]        NVARCHAR (50)   NOT NULL,
    [Timbre]            DECIMAL (18, 2) NOT NULL,
    [adresse]           NVARCHAR (MAX)  NOT NULL,
    [tel]               NVARCHAR (MAX)  NOT NULL,
    [fax]               NVARCHAR (MAX)  NULL,
    [email]             NVARCHAR (MAX)  NULL,
    [matriculeFiscale]  NVARCHAR (MAX)  NULL,
    [codeTVA]           NVARCHAR (MAX)  NOT NULL,
    [codeCategorie]     NVARCHAR (MAX)  NULL,
    [etbSecondaire]     NVARCHAR (MAX)  NULL,
    [pourcentageFodec]  DECIMAL (18, 2) NOT NULL,
    [adresseRetenu]     NVARCHAR (MAX)  NULL,
    [pourcentageRetenu] FLOAT (53)      NOT NULL,
    CONSTRAINT [PK_dbo.Systeme] PRIMARY KEY CLUSTERED ([Id] ASC)
);

