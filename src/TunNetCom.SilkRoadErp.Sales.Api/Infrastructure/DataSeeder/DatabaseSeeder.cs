using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

    public async Task SeedAsync(SalesContext context, bool forceSeed = false)
    {
        try
        {
            _logger.LogInformation("=== DÉBUT DU SEEDING DE LA BASE DE DONNÉES ===");
            _logger.LogInformation("ForceSeed: {ForceSeed}", forceSeed);
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

            await SeedClientsAsync(context, forceSeed);
            await SeedFournisseursAsync(context, forceSeed);
            await SeedSystemeAsync(context, forceSeed);
            await SeedProduitsAsync(context, forceSeed);

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

    private async Task SeedClientsAsync(SalesContext context, bool forceSeed = false)
    {
        var count = await context.Client.CountAsync();
        _logger.LogInformation("Table Client - Nombre d'enregistrements actuels: {Count}", count);
        
        // Si forceSeed est true, on force toujours l'insertion
        // Sinon, on insère seulement si la table est vide
        if (count > 0 && !forceSeed)
        {
            _logger.LogInformation("La table Client contient déjà {Count} enregistrement(s). Seeding ignoré. Utilisez forceSeed=true pour forcer le seeding.", count);
            return;
        }
        
        if (count > 0 && forceSeed)
        {
            _logger.LogWarning("La table Client contient déjà {Count} enregistrement(s). Le seeding forcé va ajouter des données supplémentaires.", count);
        }
        
        _logger.LogInformation("Tentative d'insertion des clients (forceSeed={ForceSeed}, count={Count})", forceSeed, count);

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

    private async Task SeedFournisseursAsync(SalesContext context, bool forceSeed = false)
    {
        var count = await context.Fournisseur.CountAsync();
        _logger.LogInformation("Table Fournisseur - Nombre d'enregistrements actuels: {Count}", count);
        
        if (count > 0 && !forceSeed)
        {
            _logger.LogInformation("La table Fournisseur contient déjà {Count} enregistrement(s). Seeding ignoré. Utilisez forceSeed=true pour forcer le seeding.", count);
            return;
        }
        
        if (count > 0 && forceSeed)
        {
            _logger.LogWarning("La table Fournisseur contient déjà {Count} enregistrement(s). Le seeding forcé va ajouter des données supplémentaires.", count);
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

    private async Task SeedSystemeAsync(SalesContext context, bool forceSeed = false)
    {
        var count = await context.Systeme.CountAsync();
        _logger.LogInformation("Table Systeme - Nombre d'enregistrements actuels: {Count}", count);
        
        if (count > 0 && !forceSeed)
        {
            _logger.LogInformation("La table Systeme contient déjà {Count} enregistrement(s). Seeding ignoré. Utilisez forceSeed=true pour forcer le seeding.", count);
            return;
        }
        
        if (count > 0 && forceSeed)
        {
            _logger.LogWarning("La table Systeme contient déjà {Count} enregistrement(s). Le seeding forcé va remplacer les données existantes.", count);
            // Pour Systeme, on supprime l'ancien enregistrement avant d'ajouter le nouveau
            var existing = await context.Systeme.FirstOrDefaultAsync();
            if (existing != null)
            {
                _logger.LogInformation("Suppression de l'enregistrement Systeme existant...");
                context.Systeme.Remove(existing);
                await context.SaveChangesAsync();
                _logger.LogInformation("Enregistrement Systeme existant supprimé.");
            }
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
            DiscountPercentage = systemeData.DiscountPercentage
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

    private async Task SeedProduitsAsync(SalesContext context, bool forceSeed = false)
    {
        var count = await context.Produit.CountAsync();
        _logger.LogInformation("Table Produit - Nombre d'enregistrements actuels: {Count}", count);
        
        if (count > 0 && !forceSeed)
        {
            _logger.LogInformation("La table Produit contient déjà {Count} enregistrement(s). Seeding ignoré. Utilisez forceSeed=true pour forcer le seeding.", count);
            return;
        }
        
        if (count > 0 && forceSeed)
        {
            _logger.LogWarning("La table Produit contient déjà {Count} enregistrement(s). Le seeding forcé va ajouter des données supplémentaires.", count);
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
            p.Qte,
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
}

