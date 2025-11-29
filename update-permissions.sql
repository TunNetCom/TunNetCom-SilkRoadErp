-- Script pour ajouter les permissions manquantes de gestion des utilisateurs
-- À exécuter sur la base de données si elle existe déjà

-- Insérer les permissions si elles n'existent pas
IF NOT EXISTS (SELECT 1 FROM Permission WHERE Name = 'CanViewUsers')
BEGIN
    INSERT INTO Permission (Name, Description) VALUES ('CanViewUsers', 'Peut voir les utilisateurs');
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE Name = 'CanCreateUser')
BEGIN
    INSERT INTO Permission (Name, Description) VALUES ('CanCreateUser', 'Peut créer des utilisateurs');
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE Name = 'CanUpdateUser')
BEGIN
    INSERT INTO Permission (Name, Description) VALUES ('CanUpdateUser', 'Peut modifier des utilisateurs');
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE Name = 'CanDeleteUser')
BEGIN
    INSERT INTO Permission (Name, Description) VALUES ('CanDeleteUser', 'Peut supprimer des utilisateurs');
END

IF NOT EXISTS (SELECT 1 FROM Permission WHERE Name = 'CanManageUsers')
BEGIN
    INSERT INTO Permission (Name, Description) VALUES ('CanManageUsers', 'Peut gérer les utilisateurs');
END

-- Assigner toutes les permissions utilisateur au rôle Admin
DECLARE @AdminRoleId INT = (SELECT Id FROM Role WHERE Name = 'Admin');
DECLARE @ViewUsersPermId INT = (SELECT Id FROM Permission WHERE Name = 'CanViewUsers');
DECLARE @CreateUserPermId INT = (SELECT Id FROM Permission WHERE Name = 'CanCreateUser');
DECLARE @UpdateUserPermId INT = (SELECT Id FROM Permission WHERE Name = 'CanUpdateUser');
DECLARE @DeleteUserPermId INT = (SELECT Id FROM Permission WHERE Name = 'CanDeleteUser');
DECLARE @ManageUsersPermId INT = (SELECT Id FROM Permission WHERE Name = 'CanManageUsers');

IF @AdminRoleId IS NOT NULL AND @ViewUsersPermId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @AdminRoleId AND PermissionId = @ViewUsersPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@AdminRoleId, @ViewUsersPermId);
    
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @AdminRoleId AND PermissionId = @CreateUserPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@AdminRoleId, @CreateUserPermId);
    
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @AdminRoleId AND PermissionId = @UpdateUserPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@AdminRoleId, @UpdateUserPermId);
    
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @AdminRoleId AND PermissionId = @DeleteUserPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@AdminRoleId, @DeleteUserPermId);
    
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @AdminRoleId AND PermissionId = @ManageUsersPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@AdminRoleId, @ManageUsersPermId);
END

-- Assigner les permissions de lecture au rôle Manager
DECLARE @ManagerRoleId INT = (SELECT Id FROM Role WHERE Name = 'Manager');

IF @ManagerRoleId IS NOT NULL AND @ViewUsersPermId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @ManagerRoleId AND PermissionId = @ViewUsersPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@ManagerRoleId, @ViewUsersPermId);
    
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @ManagerRoleId AND PermissionId = @CreateUserPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@ManagerRoleId, @CreateUserPermId);
    
    IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE RoleId = @ManagerRoleId AND PermissionId = @UpdateUserPermId)
        INSERT INTO RolePermission (RoleId, PermissionId) VALUES (@ManagerRoleId, @UpdateUserPermId);
END

PRINT 'Permissions utilisateur mises à jour avec succès!';


