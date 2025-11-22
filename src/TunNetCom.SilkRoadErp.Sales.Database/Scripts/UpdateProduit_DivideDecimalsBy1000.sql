-- Script SQL pour diviser les colonnes décimales de la table Produit par 1000
-- et modifier la précision à 3 chiffres après la virgule
-- Colonnes concernées : prix, prixAchat
-- Date: 2025-01-XX

BEGIN TRANSACTION;

BEGIN TRY
    -- Sauvegarder les valeurs actuelles avant modification (optionnel, pour vérification)
    -- SELECT refe, prix, prixAchat INTO Produit_Backup_YYYYMMDD FROM Produit;

    -- Étape 1 : Modifier le type de données des colonnes pour avoir 3 chiffres après la virgule
    -- Modifier la colonne prix de DECIMAL(18, 2) à DECIMAL(18, 3)
    ALTER TABLE [dbo].[Produit]
    ALTER COLUMN [prix] DECIMAL(18, 3) NOT NULL;

    -- Modifier la colonne prixAchat de DECIMAL(18, 2) à DECIMAL(18, 3)
    ALTER TABLE [dbo].[Produit]
    ALTER COLUMN [prixAchat] DECIMAL(18, 3) NOT NULL;

    -- Modifier les colonnes décimales des tables Ligne* pour avoir 3 chiffres après la virgule
    -- LigneBL
    ALTER TABLE [dbo].[LigneBL]
    ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneBL]
    ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneBL]
    ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;

    -- LigneCommandes
    ALTER TABLE [dbo].[LigneCommandes]
    ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneCommandes]
    ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneCommandes]
    ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;

    -- LigneDevis
    ALTER TABLE [dbo].[LigneDevis]
    ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneDevis]
    ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneDevis]
    ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;

    -- LigneBonReception
    ALTER TABLE [dbo].[LigneBonReception]
    ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneBonReception]
    ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneBonReception]
    ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;

    -- LigneAvoirs
    ALTER TABLE [dbo].[LigneAvoirs]
    ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneAvoirs]
    ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneAvoirs]
    ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;

    -- LigneAvoirFournisseur
    ALTER TABLE [dbo].[LigneAvoirFournisseur]
    ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneAvoirFournisseur]
    ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
    ALTER TABLE [dbo].[LigneAvoirFournisseur]
    ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;

    -- Étape 2 : Diviser la colonne prix par 1000
    UPDATE [dbo].[Produit]
    SET [prix] = [prix] / 1000.0
    WHERE [prix] IS NOT NULL;

    -- Étape 3 : Diviser la colonne prixAchat par 1000
    UPDATE [dbo].[Produit]
    SET [prixAchat] = [prixAchat] / 1000.0
    WHERE [prixAchat] IS NOT NULL;

    -- Vérification : Afficher quelques exemples pour confirmer la modification
    SELECT TOP 10 
        refe,
        prix AS prix_apres_modification,
        prixAchat AS prixAchat_apres_modification
    FROM [dbo].[Produit]
    ORDER BY refe;

    COMMIT TRANSACTION;
    PRINT 'Mise à jour réussie : Toutes les colonnes décimales ont été modifiées en DECIMAL(18, 3) et les colonnes prix/prixAchat de Produit ont été divisées par 1000.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Erreur lors de la mise à jour : ' + ERROR_MESSAGE();
    THROW;
END CATCH;

