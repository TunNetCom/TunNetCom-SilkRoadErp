/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- =============================================
-- Script de pré-déploiement : Mise à jour des colonnes décimales
-- Description: Modifie la précision des colonnes décimales de DECIMAL(18, 2) à DECIMAL(18, 3)
--              et divise les prix de la table Produit par 1000
-- Date: 2025-01-XX
-- =============================================

-- Vérifier si les modifications ont déjà été appliquées
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'dbo' 
    AND TABLE_NAME = 'Produit' 
    AND COLUMN_NAME = 'prix' 
    AND NUMERIC_PRECISION = 18 
    AND NUMERIC_SCALE = 3
)
BEGIN
    PRINT 'Les colonnes décimales ont déjà été mises à jour. Aucune modification nécessaire.';
    RETURN;
END

PRINT 'Début de la mise à jour des colonnes décimales...';

BEGIN TRANSACTION;

BEGIN TRY
    -- Étape 1 : Modifier le type de données des colonnes pour avoir 3 chiffres après la virgule
    -- Modifier la colonne prix de DECIMAL(18, 2) à DECIMAL(18, 3)
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Produit' AND COLUMN_NAME = 'prix' 
               AND NUMERIC_SCALE = 2)
    BEGIN
        PRINT 'Modification de la colonne Produit.prix en DECIMAL(18, 3)...';
        ALTER TABLE [dbo].[Produit]
        ALTER COLUMN [prix] DECIMAL(18, 3) NOT NULL;
    END

    -- Modifier la colonne prixAchat de DECIMAL(18, 2) à DECIMAL(18, 3)
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Produit' AND COLUMN_NAME = 'prixAchat' 
               AND NUMERIC_SCALE = 2)
    BEGIN
        PRINT 'Modification de la colonne Produit.prixAchat en DECIMAL(18, 3)...';
        ALTER TABLE [dbo].[Produit]
        ALTER COLUMN [prixAchat] DECIMAL(18, 3) NOT NULL;
    END

    -- Modifier les colonnes décimales des tables Ligne* pour avoir 3 chiffres après la virgule
    -- LigneBL
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBL')
    BEGIN
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBL' AND COLUMN_NAME = 'prix_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneBL.prix_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneBL] ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBL' AND COLUMN_NAME = 'tot_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneBL.tot_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneBL] ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBL' AND COLUMN_NAME = 'tot_TTC' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneBL.tot_TTC en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneBL] ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;
        END
    END

    -- LigneCommandes
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneCommandes')
    BEGIN
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneCommandes' AND COLUMN_NAME = 'prix_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneCommandes.prix_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneCommandes] ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneCommandes' AND COLUMN_NAME = 'tot_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneCommandes.tot_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneCommandes] ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneCommandes' AND COLUMN_NAME = 'tot_TTC' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneCommandes.tot_TTC en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneCommandes] ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;
        END
    END

    -- LigneDevis
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneDevis')
    BEGIN
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneDevis' AND COLUMN_NAME = 'prix_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneDevis.prix_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneDevis] ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneDevis' AND COLUMN_NAME = 'tot_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneDevis.tot_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneDevis] ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneDevis' AND COLUMN_NAME = 'tot_TTC' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneDevis.tot_TTC en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneDevis] ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;
        END
    END

    -- LigneBonReception
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBonReception')
    BEGIN
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBonReception' AND COLUMN_NAME = 'prix_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneBonReception.prix_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneBonReception] ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBonReception' AND COLUMN_NAME = 'tot_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneBonReception.tot_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneBonReception] ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneBonReception' AND COLUMN_NAME = 'tot_TTC' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneBonReception.tot_TTC en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneBonReception] ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;
        END
    END

    -- LigneAvoirs
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirs')
    BEGIN
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirs' AND COLUMN_NAME = 'prix_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneAvoirs.prix_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneAvoirs] ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirs' AND COLUMN_NAME = 'tot_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneAvoirs.tot_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneAvoirs] ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirs' AND COLUMN_NAME = 'tot_TTC' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneAvoirs.tot_TTC en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneAvoirs] ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;
        END
    END

    -- LigneAvoirFournisseur
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirFournisseur')
    BEGIN
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirFournisseur' AND COLUMN_NAME = 'prix_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneAvoirFournisseur.prix_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneAvoirFournisseur] ALTER COLUMN [prix_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirFournisseur' AND COLUMN_NAME = 'tot_HT' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneAvoirFournisseur.tot_HT en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneAvoirFournisseur] ALTER COLUMN [tot_HT] DECIMAL(18, 3) NOT NULL;
        END
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LigneAvoirFournisseur' AND COLUMN_NAME = 'tot_TTC' 
                   AND NUMERIC_SCALE = 2)
        BEGIN
            PRINT 'Modification de la colonne LigneAvoirFournisseur.tot_TTC en DECIMAL(18, 3)...';
            ALTER TABLE [dbo].[LigneAvoirFournisseur] ALTER COLUMN [tot_TTC] DECIMAL(18, 3) NOT NULL;
        END
    END

    -- Étape 2 : Ajouter les colonnes VatAmount et DiscountPercentage à la table Systeme si elles n'existent pas
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Systeme' AND COLUMN_NAME = 'VatAmount')
    BEGIN
        PRINT 'Ajout de la colonne Systeme.VatAmount...';
        ALTER TABLE [dbo].[Systeme]
        ADD [VatAmount] DECIMAL(18, 2) NOT NULL DEFAULT 19;
        
        -- Note: Les enregistrements existants recevront automatiquement la valeur par défaut 19
        PRINT 'Colonne VatAmount ajoutée à la table Systeme avec valeur par défaut 19.';
    END
    ELSE
    BEGIN
        PRINT 'La colonne Systeme.VatAmount existe déjà.';
    END

    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Systeme' AND COLUMN_NAME = 'DiscountPercentage')
    BEGIN
        PRINT 'Ajout de la colonne Systeme.DiscountPercentage...';
        ALTER TABLE [dbo].[Systeme]
        ADD [DiscountPercentage] DECIMAL(18, 2) NOT NULL DEFAULT 15.95;
        
        -- Note: Les enregistrements existants recevront automatiquement la valeur par défaut 15.95
        PRINT 'Colonne DiscountPercentage ajoutée à la table Systeme avec valeur par défaut 15.95.';
    END
    ELSE
    BEGIN
        PRINT 'La colonne Systeme.DiscountPercentage existe déjà.';
    END

    -- Étape 3 : Diviser les prix de la table Produit par 1000 (uniquement si pas déjà divisés)
    -- Vérifier si les prix sont encore en milliers (valeur > 1000 pour la plupart)
    IF EXISTS (SELECT 1 FROM [dbo].[Produit] WHERE [prix] > 1000 OR [prixAchat] > 1000)
    BEGIN
        PRINT 'Division des prix de la table Produit par 1000...';
        UPDATE [dbo].[Produit]
        SET [prix] = [prix] / 1000.0
        WHERE [prix] IS NOT NULL AND [prix] > 1000;

        UPDATE [dbo].[Produit]
        SET [prixAchat] = [prixAchat] / 1000.0
        WHERE [prixAchat] IS NOT NULL AND [prixAchat] > 1000;
    END
    ELSE
    BEGIN
        PRINT 'Les prix semblent déjà avoir été divisés par 1000. Aucune mise à jour nécessaire.';
    END

    COMMIT TRANSACTION;
    PRINT 'Mise à jour réussie : Toutes les colonnes décimales ont été modifiées en DECIMAL(18, 3), les colonnes VatAmount et DiscountPercentage ont été ajoutées à Systeme, et les prix ont été divisés par 1000.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT 'Erreur lors de la mise à jour : ' + @ErrorMessage;
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

