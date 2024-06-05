CREATE TABLE [dbo].[Devis] (
    [Num]       INT             IDENTITY (1, 1) NOT NULL,
    [id_client] INT             NOT NULL,
    [date]      DATETIME        NOT NULL,
    [tot_H_tva] DECIMAL (18, 2) NOT NULL,
    [tot_tva]   DECIMAL (18, 2) NOT NULL,
    [tot_ttc]   DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.Devis] PRIMARY KEY CLUSTERED ([Num] ASC),
    CONSTRAINT [FK_dbo.Devis_dbo.Client_id_client] FOREIGN KEY ([id_client]) REFERENCES [dbo].[Client] ([Id]) ON DELETE CASCADE
);

