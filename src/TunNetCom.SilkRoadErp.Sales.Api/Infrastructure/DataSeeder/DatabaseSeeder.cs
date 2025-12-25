using System.Text.Json;
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
            await SeedFamilleProduitAsync(context);
            await SeedSousFamilleProduitAsync(context);
            await SeedProduitsAsync(context);
            await SeedBanquesAsync(context);
            await SeedInstallationTechniciansAsync(context);
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
            f.Adresse,
            null, // TauxRetenu
            null, // RibCodeEtab
            null, // RibCodeAgence
            null, // RibNumeroCompte
            null  // RibCle
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

    private async Task SeedFamilleProduitAsync(SalesContext context)
    {
        var count = await context.FamilleProduit.CountAsync();
        _logger.LogInformation("Table FamilleProduit - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table FamilleProduit contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        var familles = new[]
        {
            FamilleProduit.CreateFamilleProduit("Cables"),
            FamilleProduit.CreateFamilleProduit("Lighting"),
            FamilleProduit.CreateFamilleProduit("Somef")
        };

        _logger.LogInformation("Ajout de {Count} familles de produits à la base de données...", familles.Length);
        try
        {
            await context.FamilleProduit.AddRangeAsync(familles);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} familles de produits insérées avec succès. {Saved} changements sauvegardés.", familles.Length, saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des familles de produits: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SeedSousFamilleProduitAsync(SalesContext context)
    {
        var count = await context.SousFamilleProduit.CountAsync();
        _logger.LogInformation("Table SousFamilleProduit - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table SousFamilleProduit contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        // Récupérer les familles pour mapper les sous-familles
        var familleCables = await context.FamilleProduit.FirstOrDefaultAsync(f => f.Nom == "Cables");
        var familleLighting = await context.FamilleProduit.FirstOrDefaultAsync(f => f.Nom == "Lighting");
        var familleSomef = await context.FamilleProduit.FirstOrDefaultAsync(f => f.Nom == "Somef");

        if (familleCables == null || familleLighting == null || familleSomef == null)
        {
            _logger.LogError("✗ ERREUR: Impossible de trouver toutes les familles de produits. Assurez-vous que SeedFamilleProduitAsync a été exécuté avant.");
            throw new InvalidOperationException("Les familles de produits doivent être créées avant les sous-familles.");
        }

        var sousFamilles = new[]
        {
            // Cables
            SousFamilleProduit.CreateSousFamilleProduit("Fil", familleCables.Id),
            SousFamilleProduit.CreateSousFamilleProduit("Cable souple", familleCables.Id),
            SousFamilleProduit.CreateSousFamilleProduit("Cable réseaux", familleCables.Id),
            SousFamilleProduit.CreateSousFamilleProduit("Cable CCTV", familleCables.Id),
            // Lighting
            SousFamilleProduit.CreateSousFamilleProduit("Spot led", familleLighting.Id),
            // Somef
            SousFamilleProduit.CreateSousFamilleProduit("Systeme 44", familleSomef.Id),
            SousFamilleProduit.CreateSousFamilleProduit("Systeme 43", familleSomef.Id),
            SousFamilleProduit.CreateSousFamilleProduit("Systeme 45", familleSomef.Id)
        };

        _logger.LogInformation("Ajout de {Count} sous-familles de produits à la base de données...", sousFamilles.Length);
        try
        {
            await context.SousFamilleProduit.AddRangeAsync(sousFamilles);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} sous-familles de produits insérées avec succès. {Saved} changements sauvegardés.", sousFamilles.Length, saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des sous-familles de produits: {Message}", ex.Message);
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

    private async Task SeedInstallationTechniciansAsync(SalesContext context)
    {
        var count = await context.InstallationTechnician.CountAsync();
        _logger.LogInformation("Table InstallationTechnician - Nombre d'enregistrements actuels: {Count}", count);
        
        // On insère seulement si la table est vide
        if (count > 0)
        {
            _logger.LogInformation("La table InstallationTechnician contient déjà {Count} enregistrement(s). Seeding ignoré.", count);
            return;
        }

        _logger.LogInformation("Création des installateurs...");

        var technicians = new[]
        {
            Domain.Entites.InstallationTechnician.CreateInstallationTechnician(
                nom: "Makrem Bouraui",
                tel: "56440436",
                tel2: null,
                tel3: null,
                email: null,
                description: null,
                photo: null
            ),
            Domain.Entites.InstallationTechnician.CreateInstallationTechnician(
                nom: "Monji Zakraoui",
                tel: "97264230",
                tel2: null,
                tel3: null,
                email: null,
                description: null,
                photo: null
            )
        };

        _logger.LogInformation("Ajout de {Count} installateurs à la base de données...", technicians.Length);
        try
        {
            await context.InstallationTechnician.AddRangeAsync(technicians);
            var saved = await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} installateurs insérés avec succès. {Saved} changements sauvegardés.", technicians.Length, saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ ERREUR lors de l'insertion des installateurs: {Message}", ex.Message);
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

        // Seed Manager User
        await SeedManagerUserAsync(context);

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

        // Récupérer toutes les permissions depuis la classe Permissions
        var allPermissionsFromClass = Permissions.GetAllPermissions();
        _logger.LogInformation("Total permissions définies dans la classe Permissions: {Count}", allPermissionsFromClass.Count);

        // Vérifier les permissions existantes
        var existingPermissions = await context.Permission
            .Select(p => p.Name)
            .ToListAsync();

        var permissionsToAdd = allPermissionsFromClass
            .Where(p => !existingPermissions.Contains(p.Name))
            .ToList();

        if (permissionsToAdd.Count == 0)
        {
            _logger.LogInformation("Toutes les permissions sont déjà présentes dans la base de données.");
        }
        else
        {
            _logger.LogInformation("Ajout de {Count} nouvelle(s) permission(s): {Names}", 
                permissionsToAdd.Count, 
                string.Join(", ", permissionsToAdd.Select(p => p.Name)));

            var permissions = permissionsToAdd
                .Select(p => Permission.CreatePermission(p.Name, p.Description))
                .ToList();

            await context.Permission.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
            _logger.LogInformation("✓ {Count} permissions insérées avec succès.", permissions.Count);
        }
    }

    private async Task SeedRolePermissionsAsync(SalesContext context)
    {
        _logger.LogInformation("=== DÉBUT ASSIGNATION PERMISSIONS AUX RÔLES ===");
        
        var adminRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "Admin");
        var managerRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "Manager");
        var userRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "User");

        if (adminRole == null || managerRole == null || userRole == null)
        {
            _logger.LogWarning("Les rôles n'existent pas encore. Impossible d'assigner les permissions.");
            return;
        }

        _logger.LogInformation("Rôles trouvés - Admin: {AdminId}, Manager: {ManagerId}, User: {UserId}", 
            adminRole.Id, managerRole.Id, userRole.Id);

        var allPermissions = await context.Permission.ToListAsync();
        _logger.LogInformation("Total permissions disponibles: {Count}", allPermissions.Count);
        
        var rolePermissions = new List<RolePermission>();

        // Get existing role permissions to avoid duplicates
        var existingAdminPermissions = await context.RolePermission
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();
        var existingManagerPermissions = await context.RolePermission
            .Where(rp => rp.RoleId == managerRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();
        var existingUserPermissions = await context.RolePermission
            .Where(rp => rp.RoleId == userRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        _logger.LogInformation("Permissions existantes - Admin: {AdminCount}, Manager: {ManagerCount}, User: {UserCount}", 
            existingAdminPermissions.Count, existingManagerPermissions.Count, existingUserPermissions.Count);

        // Admin gets ALL permissions (only add missing ones)
        int adminPermissionsAdded = 0;
        foreach (var permission in allPermissions)
        {
            if (!existingAdminPermissions.Contains(permission.Id))
            {
                rolePermissions.Add(RolePermission.CreateRolePermission(adminRole.Id, permission.Id));
                adminPermissionsAdded++;
                _logger.LogDebug("Ajout permission {PermissionName} au rôle Admin", permission.Name);
            }
        }
        _logger.LogInformation("Admin: {AddedCount} nouvelles permissions à ajouter", adminPermissionsAdded);

        // Manager gets most permissions except user/role management (only add missing ones)
        var managerPermissions = allPermissions.Where(p => 
            !p.Name.Contains("ManageUsers") && 
            !p.Name.Contains("ManageRoles") && 
            !p.Name.Contains("ManagePermissions")).ToList();
        
        int managerPermissionsAdded = 0;
        foreach (var permission in managerPermissions)
        {
            if (!existingManagerPermissions.Contains(permission.Id))
            {
                rolePermissions.Add(RolePermission.CreateRolePermission(managerRole.Id, permission.Id));
                managerPermissionsAdded++;
                _logger.LogDebug("Ajout permission {PermissionName} au rôle Manager", permission.Name);
            }
        }
        _logger.LogInformation("Manager: {AddedCount} nouvelles permissions à ajouter", managerPermissionsAdded);

        // User gets view permissions only (only add missing ones)
        var userPermissions = allPermissions.Where(p => p.Name.StartsWith("CanView")).ToList();
        int userPermissionsAdded = 0;
        foreach (var permission in userPermissions)
        {
            if (!existingUserPermissions.Contains(permission.Id))
            {
                rolePermissions.Add(RolePermission.CreateRolePermission(userRole.Id, permission.Id));
                userPermissionsAdded++;
                _logger.LogDebug("Ajout permission {PermissionName} au rôle User", permission.Name);
            }
        }
        _logger.LogInformation("User: {AddedCount} nouvelles permissions à ajouter", userPermissionsAdded);

        if (rolePermissions.Count > 0)
        {
            await context.RolePermission.AddRangeAsync(rolePermissions);
            await context.SaveChangesAsync();
            _logger.LogInformation("✓✓✓ {Count} permission(s) ajoutée(s) aux rôles avec succès.", rolePermissions.Count);
            _logger.LogInformation("    - Admin: {AdminCount} permissions", adminPermissionsAdded);
            _logger.LogInformation("    - Manager: {ManagerCount} permissions", managerPermissionsAdded);
            _logger.LogInformation("    - User: {UserCount} permissions", userPermissionsAdded);
        }
        else
        {
            _logger.LogInformation("✓ Toutes les permissions sont déjà assignées aux rôles.");
        }
        
        // Vérification finale
        var finalAdminCount = await context.RolePermission.CountAsync(rp => rp.RoleId == adminRole.Id);
        _logger.LogInformation("VÉRIFICATION FINALE: Admin a maintenant {Count} permissions sur {Total} disponibles", 
            finalAdminCount, allPermissions.Count);
        
        if (finalAdminCount < allPermissions.Count)
        {
            _logger.LogWarning("⚠️ ATTENTION: Admin n'a pas toutes les permissions! ({Current}/{Total})", 
                finalAdminCount, allPermissions.Count);
        }
        
        _logger.LogInformation("=== FIN ASSIGNATION PERMISSIONS AUX RÔLES ===");
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

    private async Task SeedManagerUserAsync(SalesContext context)
    {
        _logger.LogInformation("=== DÉBUT DU SEEDING UTILISATEUR MANAGER ===");

        // Check if manager user already exists
        var existingManager = await context.User
            .FirstOrDefaultAsync(u => u.Username == "Nieze");

        if (existingManager != null)
        {
            _logger.LogInformation("L'utilisateur Nieze existe déjà (ID: {UserId}).", existingManager.Id);
            
            // Ensure manager has Manager role
            var managerRole = await context.Role.FirstOrDefaultAsync(r => r.Name == "Manager");
            if (managerRole != null)
            {
                var existingUserRole = await context.UserRole
                    .FirstOrDefaultAsync(ur => ur.UserId == existingManager.Id && ur.RoleId == managerRole.Id);
                
                if (existingUserRole == null)
                {
                    var userRole = UserRole.CreateUserRole(existingManager.Id, managerRole.Id);
                    await context.UserRole.AddAsync(userRole);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("✓ Rôle Manager assigné à l'utilisateur Nieze existant.");
                }
            }
            return;
        }

        var passwordHasher = new PasswordHasher();
        var managerPasswordHash = passwordHasher.HashPassword("Manager123!"); // Default password - should be changed in production

        var managerUser = User.CreateUser(
            username: "Nieze",
            email: "nieze@silkroaderp.com",
            passwordHash: managerPasswordHash,
            firstName: "Nieze",
            lastName: "Manager",
            isActive: true
        );

        await context.User.AddAsync(managerUser);
        await context.SaveChangesAsync();

        // Assign Manager role to manager user
        var managerRole2 = await context.Role.FirstOrDefaultAsync(r => r.Name == "Manager");
        if (managerRole2 != null)
        {
            var userRole = UserRole.CreateUserRole(managerUser.Id, managerRole2.Id);
            await context.UserRole.AddAsync(userRole);
            await context.SaveChangesAsync();
            _logger.LogInformation("✓ Rôle Manager assigné à l'utilisateur Nieze.");
        }
        else
        {
            _logger.LogWarning("⚠ Le rôle Manager n'existe pas. L'utilisateur Nieze a été créé sans rôle.");
        }

        _logger.LogInformation("✓ Utilisateur manager Nieze créé avec succès");
        _logger.LogInformation("  - Username: Nieze");
        _logger.LogInformation("  - Password: Manager123!");
        _logger.LogInformation("  - Email: nieze@silkroaderp.com");
        _logger.LogInformation("=== FIN DU SEEDING UTILISATEUR MANAGER ===");
    }
}

