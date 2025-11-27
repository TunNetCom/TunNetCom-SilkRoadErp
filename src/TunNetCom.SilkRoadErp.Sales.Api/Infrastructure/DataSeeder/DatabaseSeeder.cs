using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonException = System.Text.Json.JsonException;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.DataSeeder;

public class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly string _seedDataPath;

    public DatabaseSeeder(ILogger<DatabaseSeeder> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        
        // Essayer plusieurs chemins possibles pour trouver les fichiers JSON
        var possiblePaths = new[]
        {
            Path.Combine(environment.ContentRootPath, "Data", "SeedData"),
            Path.Combine(AppContext.BaseDirectory, "Data", "SeedData"),
            Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData"),
            Path.Combine(environment.ContentRootPath, "..", "..", "..", "Data", "SeedData")
        };
        
        _seedDataPath = possiblePaths.FirstOrDefault(Directory.Exists) ?? possiblePaths[0];
        
        _logger.LogInformation("ContentRootPath: {ContentRootPath}", environment.ContentRootPath);
        _logger.LogInformation("AppContext.BaseDirectory: {BaseDirectory}", AppContext.BaseDirectory);
        _logger.LogInformation("Directory.GetCurrentDirectory(): {CurrentDirectory}", Directory.GetCurrentDirectory());
        _logger.LogInformation("Chemin des données de seed sélectionné: {SeedDataPath}", _seedDataPath);
        _logger.LogInformation("Le dossier existe: {Exists}", Directory.Exists(_seedDataPath));
    }

    public async Task SeedAsync(SalesContext context)
    {
        try
        {
            _logger.LogInformation("=== DÉBUT DU SEEDING DE LA BASE DE DONNÉES ===");
            _logger.LogInformation("Chemin des données de seed: {SeedDataPath}", _seedDataPath);
            _logger.LogInformation("Vérification de l'existence du dossier: {Exists}", Directory.Exists(_seedDataPath));
            
            if (Directory.Exists(_seedDataPath))
            {
                var files = Directory.GetFiles(_seedDataPath, "*.json");
                _logger.LogInformation("Fichiers JSON trouvés dans le dossier: {Count}", files.Length);
                foreach (var file in files)
                {
                    _logger.LogInformation("  - {FileName} ({Size} bytes)", Path.GetFileName(file), new FileInfo(file).Length);
                }
            }

            await SeedAccountingYearAsync(context);
            await SeedClientsAsync(context);
            await SeedFournisseursAsync(context);
            await SeedSystemeAsync(context);
            await SeedProduitsAsync(context);
            await SeedBanquesAsync(context);
            await SeedAuthAsync(context);

            _logger.LogInformation("=== SEEDING DE LA BASE DE DONNÉES TERMINÉ AVEC SUCCÈS ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== ERREUR LORS DU SEEDING DE LA BASE DE DONNÉES ===");
            _logger.LogError("Message: {Message}", ex.Message);
            _logger.LogError("StackTrace: {StackTrace}", ex.StackTrace);
            // Ne pas faire échouer le démarrage de l'API
        }
    }

    private async Task SeedAccountingYearAsync(SalesContext context)
    {
        var count = await context.AccountingYear.CountAsync();
        _logger.LogInformation("Table AccountingYear - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table AccountingYear contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }
        
        _logger.LogInformation("Création de l'exercice comptable 2025 (actif)...");

        // S'assurer qu'aucun autre exercice n'est actif avant de créer le nouveau
        var activeYears = await context.AccountingYear
            .Where(ay => ay.IsActive)
            .ToListAsync();
        
        if (activeYears.Any())
        {
            _logger.LogInformation("Désactivation de {Count} exercice(s) comptable(s) actif(s) existant(s)...", activeYears.Count);
            foreach (var year in activeYears)
            {
                year.SetInactive();
            }
            await context.SaveChangesAsync();
        }

        // Vérifier si l'exercice 2025 existe déjà
        var existing2025 = await context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.Year == 2025);
        
        if (existing2025 != null)
        {
            _logger.LogInformation("L'exercice comptable 2025 existe déjà. Activation de cet exercice...");
            existing2025.SetActive();
            await context.SaveChangesAsync();
            _logger.LogInformation("✓ Exercice comptable 2025 activé avec succès.");
            return;
        }

        // Créer l'exercice comptable 2025 actif
        var accountingYear2025 = AccountingYear.CreateAccountingYear(2025, isActive: true);

        _logger.LogInformation("Ajout de l'exercice comptable 2025 à la base de données...");
        try
        {
            await context.AccountingYear.AddAsync(accountingYear2025);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ Exercice comptable 2025 (actif) inséré avec succès. {Saved} changements sauvegardés.", saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion de l'exercice comptable 2025: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SeedClientsAsync(SalesContext context)
    {
        var count = await context.Client.CountAsync();
        _logger.LogInformation("Table Client - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table Client contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }
        
        _logger.LogInformation("Tentative d'insertion des clients...");

        var jsonPath = Path.Combine(_seedDataPath, "clients.json");
        _logger.LogInformation("Recherche du fichier clients.json à: {JsonPath}", jsonPath);
        if (!File.Exists(jsonPath))
        {
            _logger.LogWarning("Fichier clients.json introuvable à {JsonPath}. Seeding ignoré.", jsonPath);
            return;
        }
        _logger.LogInformation("Fichier clients.json trouvé. Taille: {Size} bytes", new FileInfo(jsonPath).Length);

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        _logger.LogInformation("Fichier clients.json lu. Contenu: {Length} caractères", jsonContent.Length);
        
        // Afficher un aperçu du contenu JSON pour debug
        var preview = jsonContent.Length > 500 ? jsonContent.Substring(0, 500) + "..." : jsonContent;
        _logger.LogInformation("Aperçu du JSON (premiers 500 caractères): {Preview}", preview);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        
        List<ClientSeedData>? clientsData;
        try
        {
            clientsData = JsonSerializer.Deserialize<List<ClientSeedData>>(jsonContent, options);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erreur de désérialisation JSON: {Message}", ex.Message);
            _logger.LogError("Position de l'erreur: BytePositionInLine={BytePosition}, LineNumber={LineNumber}", ex.BytePositionInLine, ex.LineNumber);
            return;
        }

        if (clientsData == null)
        {
            _logger.LogWarning("Échec de la désérialisation du fichier clients.json - résultat null.");
            return;
        }
        
        _logger.LogInformation("Désérialisation réussie. Nombre d'éléments: {Count}", clientsData.Count);
        
        if (clientsData.Count == 0)
        {
            _logger.LogInformation("Aucune donnée client à insérer (fichier vide).");
            return;
        }
        
        _logger.LogInformation("Nombre de clients à insérer: {Count}", clientsData.Count);

        // Filtrer les clients avec nom null ou vide
        var validClients = clientsData
            .Where(c => !string.IsNullOrWhiteSpace(c.Nom))
            .ToList();
        
        var skippedCount = clientsData.Count - validClients.Count;
        if (skippedCount > 0)
        {
            _logger.LogWarning("{SkippedCount} clients ignorés car le nom est null ou vide.", skippedCount);
        }
        
        _logger.LogInformation("Nombre de clients valides à insérer: {Count}", validClients.Count);

        var clients = validClients.Select(c => Client.CreateClient(
            c.Nom!,
            c.Tel,
            c.Adresse,
            c.Matricule,
            c.Code,
            c.CodeCat,
            c.EtbSec,
            c.Mail
        )).ToList();

        _logger.LogInformation("Ajout de {Count} clients à la base de données...", clients.Count);
        try
        {
            await context.Client.AddRangeAsync(clients);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} clients insérés avec succès. {Saved} changements sauvegardés.", clients.Count, saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des clients: {Message}", ex.Message);
            throw; // Re-lancer pour voir l'erreur complète
        }
    }

    private async Task SeedFournisseursAsync(SalesContext context)
    {
        var count = await context.Fournisseur.CountAsync();
        _logger.LogInformation("Table Fournisseur - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table Fournisseur contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        var jsonPath = Path.Combine(_seedDataPath, "fournisseurs.json");
        _logger.LogInformation("Recherche du fichier fournisseurs.json à: {JsonPath}", jsonPath);
        if (!File.Exists(jsonPath))
        {
            _logger.LogWarning("Fichier fournisseurs.json introuvable à {JsonPath}. Seeding ignoré.", jsonPath);
            return;
        }
        _logger.LogInformation("Fichier fournisseurs.json trouvé. Taille: {Size} bytes", new FileInfo(jsonPath).Length);

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        _logger.LogInformation("Fichier fournisseurs.json lu. Contenu: {Length} caractères", jsonContent.Length);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        
        List<FournisseurSeedData>? fournisseursData;
        try
        {
            fournisseursData = JsonSerializer.Deserialize<List<FournisseurSeedData>>(jsonContent, options);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erreur de désérialisation JSON: {Message}", ex.Message);
            return;
        }

        if (fournisseursData == null)
        {
            _logger.LogWarning("Échec de la désérialisation du fichier fournisseurs.json - résultat null.");
            return;
        }
        
        _logger.LogInformation("Désérialisation réussie. Nombre d'éléments: {Count}", fournisseursData.Count);
        
        if (fournisseursData.Count == 0)
        {
            _logger.LogInformation("Aucune donnée fournisseur à insérer (fichier vide).");
            return;
        }
        
        _logger.LogInformation("Nombre de fournisseurs à insérer: {Count}", fournisseursData.Count);

        // Filtrer les fournisseurs avec nom ou tel null ou vide
        var validFournisseurs = fournisseursData
            .Where(f => !string.IsNullOrWhiteSpace(f.Nom) && !string.IsNullOrWhiteSpace(f.Tel))
            .ToList();
        
        var skippedCount = fournisseursData.Count - validFournisseurs.Count;
        if (skippedCount > 0)
        {
            _logger.LogWarning("{SkippedCount} fournisseurs ignorés car le nom ou le tel est null ou vide.", skippedCount);
        }
        
        _logger.LogInformation("Nombre de fournisseurs valides à insérer: {Count}", validFournisseurs.Count);

        var fournisseurs = validFournisseurs.Select(f => Fournisseur.CreateProvider(
            f.Nom!,
            f.Tel!,
            f.Fax,
            f.Matricule,
            f.Code,
            f.CodeCat,
            f.EtbSec,
            f.Mail,
            f.MailDeux,
            f.Constructeur ?? false, // Valeur par défaut si null
            f.Adresse
        )).ToList();

        _logger.LogInformation("Ajout de {Count} fournisseurs à la base de données...", fournisseurs.Count);
        try
        {
            await context.Fournisseur.AddRangeAsync(fournisseurs);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} fournisseurs insérés avec succès. {Saved} changements sauvegardés.", fournisseurs.Count, saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des fournisseurs: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SeedSystemeAsync(SalesContext context)
    {
        var count = await context.Systeme.CountAsync();
        _logger.LogInformation("Table Systeme - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table Systeme contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        var jsonPath = Path.Combine(_seedDataPath, "systeme.json");
        _logger.LogInformation("Recherche du fichier systeme.json à: {JsonPath}", jsonPath);
        if (!File.Exists(jsonPath))
        {
            _logger.LogWarning("Fichier systeme.json introuvable à {JsonPath}. Seeding ignoré.", jsonPath);
            return;
        }
        _logger.LogInformation("Fichier systeme.json trouvé. Taille: {Size} bytes", new FileInfo(jsonPath).Length);

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        _logger.LogInformation("Fichier systeme.json lu. Contenu: {Length} caractères", jsonContent.Length);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        
        SystemeSeedData? systemeData;
        try
        {
            systemeData = JsonSerializer.Deserialize<SystemeSeedData>(jsonContent, options);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erreur de désérialisation JSON: {Message}", ex.Message);
            return;
        }

        if (systemeData == null)
        {
            _logger.LogWarning("Échec de la désérialisation du fichier systeme.json - résultat null.");
            return;
        }
        
        _logger.LogInformation("Désérialisation réussie.");
        
        _logger.LogInformation("Données système à insérer trouvées.");

        // Systeme n'a pas de méthode factory, on doit créer l'entité directement
        var systeme = new Systeme
        {
            Id = systemeData.Id,
            NomSociete = systemeData.NomSociete,
            Timbre = systemeData.Timbre,
            Adresse = systemeData.Adresse,
            Tel = systemeData.Tel,
            Fax = systemeData.Fax,
            Email = systemeData.Email,
            MatriculeFiscale = systemeData.MatriculeFiscale,
            CodeTva = systemeData.CodeTva,
            CodeCategorie = systemeData.CodeCategorie,
            EtbSecondaire = systemeData.EtbSecondaire,
            PourcentageFodec = systemeData.PourcentageFodec,
            AdresseRetenu = systemeData.AdresseRetenu,
            PourcentageRetenu = systemeData.PourcentageRetenu,
            VatAmount = systemeData.VatAmount,
            DiscountPercentage = systemeData.DiscountPercentage,
            VatRate0 = systemeData.VatRate0,
            VatRate7 = systemeData.VatRate7,
            VatRate13 = systemeData.VatRate13,
            VatRate19 = systemeData.VatRate19
        };

        _logger.LogInformation("Ajout des données système à la base de données...");
        try
        {
            await context.Systeme.AddAsync(systeme);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ Données système insérées avec succès. {Saved} changements sauvegardés.", saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des données système: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SeedProduitsAsync(SalesContext context)
    {
        var count = await context.Produit.CountAsync();
        _logger.LogInformation("Table Produit - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table Produit contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        var jsonPath = Path.Combine(_seedDataPath, "produits.json");
        _logger.LogInformation("Recherche du fichier produits.json à: {JsonPath}", jsonPath);
        if (!File.Exists(jsonPath))
        {
            _logger.LogWarning("Fichier produits.json introuvable à {JsonPath}. Seeding ignoré.", jsonPath);
            return;
        }
        _logger.LogInformation("Fichier produits.json trouvé. Taille: {Size} bytes", new FileInfo(jsonPath).Length);

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        _logger.LogInformation("Fichier produits.json lu. Contenu: {Length} caractères", jsonContent.Length);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        
        List<ProduitSeedData>? produitsData;
        try
        {
            produitsData = JsonSerializer.Deserialize<List<ProduitSeedData>>(jsonContent, options);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erreur de désérialisation JSON: {Message}", ex.Message);
            return;
        }

        if (produitsData == null)
        {
            _logger.LogWarning("Échec de la désérialisation du fichier produits.json - résultat null.");
            return;
        }
        
        _logger.LogInformation("Désérialisation réussie. Nombre d'éléments: {Count}", produitsData.Count);
        
        if (produitsData.Count == 0)
        {
            _logger.LogInformation("Aucune donnée produit à insérer (fichier vide).");
            return;
        }
        
        _logger.LogInformation("Nombre de produits à insérer: {Count}", produitsData.Count);

        // Filtrer les produits avec refe null ou vide (refe est la clé primaire)
        var validProduits = produitsData
            .Where(p => !string.IsNullOrWhiteSpace(p.Refe))
            .ToList();
        
        var skippedCount = produitsData.Count - validProduits.Count;
        if (skippedCount > 0)
        {
            _logger.LogWarning("{SkippedCount} produits ignorés car la référence (refe) est null ou vide.", skippedCount);
        }
        
        _logger.LogInformation("Nombre de produits valides à insérer: {Count}", validProduits.Count);

        var produits = validProduits.Select(p => Produit.CreateProduct(
            p.Refe!,
            p.Nom,
            p.QteLimite,
            p.Remise,
            p.RemiseAchat,
            p.Tva,
            p.Prix,
            p.PrixAchat,
            p.Visibilite ?? true // Valeur par défaut si null
        )).ToList();

        _logger.LogInformation("Ajout de {Count} produits à la base de données...", produits.Count);
        try
        {
            await context.Produit.AddRangeAsync(produits);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} produits insérés avec succès. {Saved} changements sauvegardés.", produits.Count, saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des produits: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SeedBanquesAsync(SalesContext context)
    {
        var count = await context.Banque.CountAsync();
        _logger.LogInformation("Table Banque - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table Banque contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        var jsonPath = Path.Combine(_seedDataPath, "banques.json");
        _logger.LogInformation("Recherche du fichier banques.json à: {JsonPath}", jsonPath);
        if (!File.Exists(jsonPath))
        {
            _logger.LogWarning("Fichier banques.json introuvable à {JsonPath}. Seeding ignoré.", jsonPath);
            return;
        }
        _logger.LogInformation("Fichier banques.json trouvé. Taille: {Size} bytes", new FileInfo(jsonPath).Length);

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        _logger.LogInformation("Fichier banques.json lu. Contenu: {Length} caractères", jsonContent.Length);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        
        List<BanqueSeedData>? banquesData;
        try
        {
            banquesData = JsonSerializer.Deserialize<List<BanqueSeedData>>(jsonContent, options);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erreur de désérialisation JSON: {Message}", ex.Message);
            return;
        }

        if (banquesData == null)
        {
            _logger.LogWarning("Échec de la désérialisation du fichier banques.json - résultat null.");
            return;
        }
        
        _logger.LogInformation("Désérialisation réussie. Nombre d'éléments: {Count}", banquesData.Count);
        
        if (banquesData.Count == 0)
        {
            _logger.LogInformation("Aucune donnée banque à insérer (fichier vide).");
            return;
        }
        
        _logger.LogInformation("Nombre de banques à insérer: {Count}", banquesData.Count);

        // Filtrer les banques avec nom null ou vide
        var validBanques = banquesData
            .Where(b => !string.IsNullOrWhiteSpace(b.Nom))
            .ToList();
        
        var skippedCount = banquesData.Count - validBanques.Count;
        if (skippedCount > 0)
        {
            _logger.LogWarning("{SkippedCount} banques ignorées car le nom est null ou vide.", skippedCount);
        }
        
        _logger.LogInformation("Nombre de banques valides à insérer: {Count}", validBanques.Count);

        var banques = validBanques.Select(b => Domain.Entites.Banque.CreateBanque(b.Nom!)).ToList();

        _logger.LogInformation("Ajout de {Count} banques à la base de données...", banques.Count);
        try
        {
            await context.Banque.AddRangeAsync(banques);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} banques insérées avec succès. {Saved} changements sauvegardés.", banques.Count, saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des banques: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SeedAuthAsync(SalesContext context)
    {
        _logger.LogInformation("=== DÉBUT DU SEEDING AUTHENTIFICATION ===");

        // Seed Roles
        await SeedRolesAsync(context);

        // Seed Permissions
        await SeedPermissionsAsync(context);

        // Assign Permissions to Roles
        await SeedRolePermissionsAsync(context);

        // Seed Admin User
        await SeedAdminUserAsync(context);

        _logger.LogInformation("=== SEEDING AUTHENTIFICATION TERMINÉ ===");
    }

    private async Task SeedRolesAsync(SalesContext context)
    {
        var count = await context.Role.CountAsync();
        _logger.LogInformation("Table Role - Nombre d'enregistrements actuels: {Count}", count);

        if (count > 0)
        {
            _logger.LogInformation("La table Role contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        var roles = new[]
        {
            Role.CreateRole("Admin", "Administrateur avec tous les droits"),
            Role.CreateRole("Manager", "Manager avec droits de gestion"),
            Role.CreateRole("User", "Utilisateur standard")
        };

        await context.Role.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        _logger.LogInformation("✓ {Count} rôles insérés avec succès.", roles.Length);
    }

    private async Task SeedPermissionsAsync(SalesContext context)
    {
        var count = await context.Permission.CountAsync();
        _logger.LogInformation("Table Permission - Nombre d'enregistrements actuels: {Count}", count);

        if (count > 0)
        {
            _logger.LogInformation("La table Permission contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        var permissions = new[]
        {
            Permission.CreatePermission("CanViewInvoices", "Peut voir les factures"),
            Permission.CreatePermission("CanCreateInvoice", "Peut créer des factures"),
            Permission.CreatePermission("CanUpdateInvoice", "Peut modifier des factures"),
            Permission.CreatePermission("CanDeleteInvoice", "Peut supprimer des factures"),
            Permission.CreatePermission("CanViewProducts", "Peut voir les produits"),
            Permission.CreatePermission("CanCreateProduct", "Peut créer des produits"),
            Permission.CreatePermission("CanUpdateProduct", "Peut modifier des produits"),
            Permission.CreatePermission("CanDeleteProduct", "Peut supprimer des produits"),
            Permission.CreatePermission("CanViewCustomers", "Peut voir les clients"),
            Permission.CreatePermission("CanCreateCustomer", "Peut créer des clients"),
            Permission.CreatePermission("CanUpdateCustomer", "Peut modifier des clients"),
            Permission.CreatePermission("CanDeleteCustomer", "Peut supprimer des clients"),
            Permission.CreatePermission("CanViewProviders", "Peut voir les fournisseurs"),
            Permission.CreatePermission("CanCreateProvider", "Peut créer des fournisseurs"),
            Permission.CreatePermission("CanUpdateProvider", "Peut modifier des fournisseurs"),
            Permission.CreatePermission("CanDeleteProvider", "Peut supprimer des fournisseurs"),
            Permission.CreatePermission("CanManageUsers", "Peut gérer les utilisateurs"),
            Permission.CreatePermission("CanManageRoles", "Peut gérer les rôles"),
            Permission.CreatePermission("CanManagePermissions", "Peut gérer les permissions")
        };

        await context.Permission.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
        _logger.LogInformation("✓ {Count} permissions insérées avec succès.", permissions.Length);
    }

    private async Task SeedRolePermissionsAsync(SalesContext context)
    {
        var adminRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "Admin");
        var managerRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "Manager");
        var userRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "User");

        if (adminRole == null || managerRole == null || userRole == null)
        {
            _logger.LogWarning("Les rôles n'existent pas encore. Impossible d'assigner les permissions.");
            return;
        }

        // Check if role permissions already exist
        var existingCount = await context.RolePermission.CountAsync();
        if (existingCount > 0)
        {
            _logger.LogInformation("Les permissions de rôles existent déjà. Seeding ignoré.");
            return;
        }

        var allPermissions = await context.Permission.ToListAsync();
        var rolePermissions = new List<RolePermission>();

        // Admin gets all permissions
        foreach (var permission in allPermissions)
        {
            rolePermissions.Add(RolePermission.CreateRolePermission(adminRole.Id, permission.Id));
        }

        // Manager gets most permissions except user/role management
        var managerPermissions = allPermissions.Where(p => 
            !p.Name.Contains("ManageUsers") && 
            !p.Name.Contains("ManageRoles") && 
            !p.Name.Contains("ManagePermissions")).ToList();
        foreach (var permission in managerPermissions)
        {
            rolePermissions.Add(RolePermission.CreateRolePermission(managerRole.Id, permission.Id));
        }

        // User gets view permissions only
        var userPermissions = allPermissions.Where(p => p.Name.StartsWith("CanView")).ToList();
        foreach (var permission in userPermissions)
        {
            rolePermissions.Add(RolePermission.CreateRolePermission(userRole.Id, permission.Id));
        }

        await context.RolePermission.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
        _logger.LogInformation("✓ Permissions assignées aux rôles avec succès.");
    }

    private async Task SeedAdminUserAsync(SalesContext context)
    {
        _logger.LogInformation("=== DÉBUT DU SEEDING UTILISATEUR ADMIN ===");

        // Check if admin user already exists
        var existingAdmin = await context.User
            .FirstOrDefaultAsync(u => u.Username == "admin");

        if (existingAdmin != null)
        {
            _logger.LogInformation("L'utilisateur admin existe déjà (ID: {UserId}).", existingAdmin.Id);
            
            // Ensure admin has Admin role
            var adminRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole != null)
            {
                var existingUserRole = await context.UserRole
                    .FirstOrDefaultAsync(ur => ur.UserId == existingAdmin.Id && ur.RoleId == adminRole.Id);
                
                if (existingUserRole == null)
                {
                    var userRole = UserRole.CreateUserRole(existingAdmin.Id, adminRole.Id);
                    await context.UserRole.AddAsync(userRole);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("✓ Rôle Admin assigné à l'utilisateur admin existant.");
                }
            }
            return;
        }

        var passwordHasher = new PasswordHasher();
        var adminPasswordHash = passwordHasher.HashPassword("Admin123!"); // Default password - should be changed in production

        var adminUser = User.CreateUser(
            username: "admin",
            email: "admin@silkroaderp.com",
            passwordHash: adminPasswordHash,
            firstName: "Admin",
            lastName: "User",
            isActive: true
        );

        await context.User.AddAsync(adminUser);
        await context.SaveChangesAsync();

        // Assign Admin role to admin user
        var adminRole2 = await context.Role.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole2 != null)
        {
            var userRole = UserRole.CreateUserRole(adminUser.Id, adminRole2.Id);
            await context.UserRole.AddAsync(userRole);
            await context.SaveChangesAsync();
            _logger.LogInformation("✓ Rôle Admin assigné à l'utilisateur admin.");
        }
        else
        {
            _logger.LogWarning("⚠ Le rôle Admin n'existe pas. L'utilisateur admin a été créé sans rôle.");
        }

        _logger.LogInformation("✓ Utilisateur admin créé avec succès");
        _logger.LogInformation("  - Username: admin");
        _logger.LogInformation("  - Password: Admin123!");
        _logger.LogInformation("  - Email: admin@silkroaderp.com");
        _logger.LogInformation("=== FIN DU SEEDING UTILISATEUR ADMIN ===");
    }
}

