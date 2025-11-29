-- Script pour corriger immédiatement les permissions de l'admin
-- Exécutez ce script pour que l'admin ait toutes les permissions

DECLARE @AdminRoleId INT;
DECLARE @PermissionsAdded INT = 0;

-- Récupérer l'ID du rôle Admin
SELECT @AdminRoleId = Id FROM Role WHERE Name = 'Admin';

IF @AdminRoleId IS NULL
BEGIN
    PRINT 'ERREUR: Le rôle Admin n''existe pas!';
    RETURN;
END

PRINT 'Rôle Admin trouvé (ID: ' + CAST(@AdminRoleId AS VARCHAR) + ')';

-- Compter les permissions existantes
DECLARE @TotalPermissions INT;
SELECT @TotalPermissions = COUNT(*) FROM Permission;
PRINT 'Total permissions dans la base: ' + CAST(@TotalPermissions AS VARCHAR);

-- Compter les permissions déjà assignées à Admin
DECLARE @AdminCurrentPermissions INT;
SELECT @AdminCurrentPermissions = COUNT(*) 
FROM RolePermission 
WHERE RoleId = @AdminRoleId;
PRINT 'Permissions actuelles de Admin: ' + CAST(@AdminCurrentPermissions AS VARCHAR);

-- Assigner TOUTES les permissions au rôle Admin
INSERT INTO RolePermission (RoleId, PermissionId)
SELECT @AdminRoleId, p.Id
FROM Permission p
WHERE NOT EXISTS (
    SELECT 1 
    FROM RolePermission rp 
    WHERE rp.RoleId = @AdminRoleId 
    AND rp.PermissionId = p.Id
);

SET @PermissionsAdded = @@ROWCOUNT;

PRINT '✓ ' + CAST(@PermissionsAdded AS VARCHAR) + ' permission(s) ajoutée(s) au rôle Admin';

-- Vérifier le résultat
SELECT @AdminCurrentPermissions = COUNT(*) 
FROM RolePermission 
WHERE RoleId = @AdminRoleId;
PRINT '✓ Admin a maintenant ' + CAST(@AdminCurrentPermissions AS VARCHAR) + ' permissions';

-- Afficher quelques permissions pour vérification
PRINT '';
PRINT 'Quelques permissions de Admin:';
SELECT TOP 10 p.Name 
FROM Permission p
JOIN RolePermission rp ON p.Id = rp.PermissionId
WHERE rp.RoleId = @AdminRoleId
ORDER BY p.Name;

-- Vérifier spécifiquement les permissions utilisateur
PRINT '';
PRINT 'Permissions utilisateur de Admin:';
SELECT p.Name 
FROM Permission p
JOIN RolePermission rp ON p.Id = rp.PermissionId
WHERE rp.RoleId = @AdminRoleId
AND p.Name LIKE '%User%'
ORDER BY p.Name;


