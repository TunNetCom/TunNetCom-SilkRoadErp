-- Script SQL pour corriger les prix qui ont été divisés par 1000 deux fois
-- Ce script multiplie les prix par 1000 une fois pour revenir à la valeur correcte
-- Colonnes concernées : prix, prixAchat
-- Date: 2025-01-XX

BEGIN TRANSACTION;

BEGIN TRY
    -- Sauvegarder les valeurs actuelles avant modification (optionnel, pour vérification)
    -- SELECT refe, prix, prixAchat INTO Produit_Backup_BeforeFix_YYYYMMDD FROM Produit;

    -- Afficher quelques exemples AVANT la correction
    PRINT 'Valeurs AVANT la correction :';
    SELECT TOP 10 
        refe,
        prix AS prix_avant_correction,
        prixAchat AS prixAchat_avant_correction
    FROM [dbo].[Produit]
    ORDER BY refe;

    -- Multiplier la colonne prix par 1000 pour corriger la double division
    UPDATE [dbo].[Produit]
    SET [prix] = [prix] * 1000.0
    WHERE [prix] IS NOT NULL;

    -- Multiplier la colonne prixAchat par 1000 pour corriger la double division
    UPDATE [dbo].[Produit]
    SET [prixAchat] = [prixAchat] * 1000.0
    WHERE [prixAchat] IS NOT NULL;

    -- Vérification : Afficher quelques exemples APRÈS la correction
    PRINT 'Valeurs APRÈS la correction :';
    SELECT TOP 10 
        refe,
        prix AS prix_apres_correction,
        prixAchat AS prixAchat_apres_correction
    FROM [dbo].[Produit]
    ORDER BY refe;

    COMMIT TRANSACTION;
    PRINT 'Correction réussie : Les colonnes prix et prixAchat ont été multipliées par 1000 pour corriger la double division.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Erreur lors de la correction : ' + ERROR_MESSAGE();
    THROW;
END CATCH;

