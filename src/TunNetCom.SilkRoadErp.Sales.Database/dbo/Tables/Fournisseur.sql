CREATE TABLE [dbo].[Fournisseur] (
    [id]           INT            IDENTITY (1, 1) NOT NULL,
    [nom]          NVARCHAR (50)  NOT NULL,
    [tel]          NVARCHAR (50)  NOT NULL,
    [fax]          NVARCHAR (50)  NULL,
    [matricule]    NVARCHAR (50)  NULL,
    [code]         NVARCHAR (50)  NULL,
    [code_cat]     NVARCHAR (50)  NULL,
    [etb_sec]      NVARCHAR (50)  NULL,
    [mail]         NVARCHAR (MAX) NULL,
    [mail_deux]    NVARCHAR (MAX) NULL,
    [constructeur] BIT            NOT NULL,
    [adresse]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Fournisseur] PRIMARY KEY CLUSTERED ([id] ASC)
);

