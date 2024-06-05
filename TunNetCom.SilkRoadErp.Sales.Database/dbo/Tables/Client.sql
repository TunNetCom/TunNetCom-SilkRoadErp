CREATE TABLE [dbo].[Client] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [nom]       NVARCHAR (50)  NOT NULL,
    [tel]       NVARCHAR (50)  NULL,
    [adresse]   NVARCHAR (50)  NULL,
    [matricule] NVARCHAR (MAX) NULL,
    [code]      NVARCHAR (MAX) NULL,
    [code_cat]  NVARCHAR (MAX) NULL,
    [etb_sec]   NVARCHAR (MAX) NULL,
    [mail]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Client] PRIMARY KEY CLUSTERED ([Id] ASC)
);

