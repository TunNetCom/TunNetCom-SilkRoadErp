# Script d'extraction des données de seed

## ExtractSeedData.ps1

Ce script PowerShell extrait les données des tables `Client`, `Fournisseur`, `Systeme` et `Produit` de la base de données source et génère les fichiers JSON nécessaires pour le data seeder.

### Utilisation

1. Ouvrir PowerShell
2. Naviguer vers le dossier du script :
   ```powershell
   cd src\TunNetCom.SilkRoadErp.Sales.Api\Scripts
   ```
3. Exécuter le script :
   ```powershell
   .\ExtractSeedData.ps1
   ```

### Configuration

Le script se connecte à la base de données suivante :
- **Serveur** : LAPTOP-UR7S8C4K
- **Base de données** : SteDataBaseWebAll
- **Authentification** : Windows Authentication (Integrated Security)

Pour modifier la chaîne de connexion, éditer la variable `$connectionString` dans le script.

### Fichiers générés

Le script génère les fichiers suivants dans `Data/SeedData/` :
- `clients.json` - Liste des clients
- `fournisseurs.json` - Liste des fournisseurs
- `systeme.json` - Données système (un seul enregistrement)
- `produits.json` - Liste des produits

### Notes

- Les fichiers JSON existants seront écrasés
- Le script gère automatiquement les valeurs NULL
- Les décimales sont préservées avec leur précision d'origine


