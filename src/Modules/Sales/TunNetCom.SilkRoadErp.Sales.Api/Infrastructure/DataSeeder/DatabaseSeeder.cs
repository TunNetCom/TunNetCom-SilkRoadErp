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

            await SeedSystemeAsync(context);

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
        // Note: Timbre, PourcentageFodec, PourcentageRetenu, VatRate0, VatRate7, VatRate13, VatRate19
        // ont été migrés vers AccountingYear et ne sont plus dans Systeme
        var systeme = new Systeme
        {
            Id = systemeData.Id,
            NomSociete = systemeData.NomSociete,
            Adresse = systemeData.Adresse,
            Tel = systemeData.Tel,
            Fax = systemeData.Fax,
            Email = systemeData.Email,
            MatriculeFiscale = systemeData.MatriculeFiscale,
            CodeTva = systemeData.CodeTva,
            CodeCategorie = systemeData.CodeCategorie,
            EtbSecondaire = systemeData.EtbSecondaire,
            AdresseRetenu = systemeData.AdresseRetenu,
            DiscountPercentage = systemeData.DiscountPercentage,
            BloquerVenteStockInsuffisant = systemeData.BloquerVenteStockInsuffisant,
            DecimalPlaces = systemeData.DecimalPlaces,
            Rib = systemeData.Rib
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
}
