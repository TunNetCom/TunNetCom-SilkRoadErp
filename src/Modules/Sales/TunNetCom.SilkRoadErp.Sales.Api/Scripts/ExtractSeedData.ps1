# Script PowerShell pour extraire les données de la base de données source
# et générer les fichiers JSON pour le data seeder

$connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=SteDataBaseWebAll;Data Source=LAPTOP-UR7S8C4K;Encrypt=False;TrustServerCertificate=False;"

# Déterminer le chemin du dossier Data/SeedData
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptPath
$seedDataPath = Join-Path $projectRoot "Data\SeedData"

# Créer le dossier s'il n'existe pas
if (-not (Test-Path $seedDataPath)) {
    New-Item -ItemType Directory -Path $seedDataPath -Force | Out-Null
    Write-Host "Dossier créé: $seedDataPath" -ForegroundColor Green
}

# Fonction pour convertir DBNull en null PowerShell
function Convert-DbValue {
    param($value)
    if ($value -eq [DBNull]::Value) {
        return $null
    }
    return $value
}

# Fonction pour exporter une table vers JSON
function Export-TableToJson {
    param(
        [string]$tableName,
        [string]$outputFile,
        [string]$query
    )
    
    try {
        Write-Host "Extraction de la table: $tableName" -ForegroundColor Cyan
        
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
        $dataset = New-Object System.Data.DataSet
        $adapter.Fill($dataset) | Out-Null
        
        $connection.Close()
        
        $rows = @()
        foreach ($row in $dataset.Tables[0].Rows) {
            $rowData = @{}
            foreach ($column in $dataset.Tables[0].Columns) {
                $value = Convert-DbValue $row[$column.ColumnName]
                $rowData[$column.ColumnName] = $value
            }
            $rows += $rowData
        }
        
        # Convertir en JSON avec formatage
        $json = $rows | ConvertTo-Json -Depth 10 -Compress:$false
        
        # Écrire dans le fichier
        $outputPath = Join-Path $seedDataPath $outputFile
        $json | Out-File -FilePath $outputPath -Encoding UTF8
        
        Write-Host "  ✓ Fichier créé: $outputFile ($($rows.Count) enregistrements)" -ForegroundColor Green
    }
    catch {
        Write-Host "  ✗ Erreur lors de l'extraction de $tableName : $_" -ForegroundColor Red
    }
}

# Fonction pour exporter Systeme (un seul enregistrement)
function Export-SystemeToJson {
    param(
        [string]$outputFile
    )
    
    try {
        Write-Host "Extraction de la table: Systeme" -ForegroundColor Cyan
        
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        $query = "SELECT * FROM [dbo].[Systeme]"
        $command = $connection.CreateCommand()
        $command.CommandText = $query
        
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
            $systemeData = @{
                id = [int]$reader["Id"]
                nomSociete = Convert-DbValue $reader["NomSociete"]
                timbre = [decimal]$reader["Timbre"]
                adresse = Convert-DbValue $reader["adresse"]
                tel = Convert-DbValue $reader["tel"]
                fax = Convert-DbValue $reader["fax"]
                email = Convert-DbValue $reader["email"]
                matriculeFiscale = Convert-DbValue $reader["matriculeFiscale"]
                codeTva = Convert-DbValue $reader["codeTVA"]
                codeCategorie = Convert-DbValue $reader["codeCategorie"]
                etbSecondaire = Convert-DbValue $reader["etbSecondaire"]
                pourcentageFodec = [decimal]$reader["pourcentageFodec"]
                adresseRetenu = Convert-DbValue $reader["adresseRetenu"]
                pourcentageRetenu = [double]$reader["pourcentageRetenu"]
                vatAmount = [decimal]$reader["VatAmount"]
                discountPercentage = [decimal]$reader["DiscountPercentage"]
            }
            
            $json = $systemeData | ConvertTo-Json -Depth 10 -Compress:$false
            
            $outputPath = Join-Path $seedDataPath $outputFile
            $json | Out-File -FilePath $outputPath -Encoding UTF8
            
            Write-Host "  ✓ Fichier créé: $outputFile" -ForegroundColor Green
        }
        else {
            Write-Host "  ⚠ Aucun enregistrement trouvé dans Systeme" -ForegroundColor Yellow
        }
        
        $reader.Close()
        $connection.Close()
    }
    catch {
        Write-Host "  ✗ Erreur lors de l'extraction de Systeme : $_" -ForegroundColor Red
    }
}

Write-Host "========================================" -ForegroundColor Magenta
Write-Host "Extraction des données de seed" -ForegroundColor Magenta
Write-Host "========================================" -ForegroundColor Magenta
Write-Host ""

# Extraire Client
Export-TableToJson -tableName "Client" -outputFile "clients.json" -query @"
SELECT 
    [nom],
    [tel],
    [adresse],
    [matricule],
    [code],
    [code_cat] as codeCat,
    [etb_sec] as etbSec,
    [mail]
FROM [dbo].[Client]
"@

# Extraire Fournisseur
Export-TableToJson -tableName "Fournisseur" -outputFile "fournisseurs.json" -query @"
SELECT 
    [nom],
    [tel],
    [fax],
    [matricule],
    [code],
    [code_cat] as codeCat,
    [etb_sec] as etbSec,
    [mail],
    [mail_deux] as mailDeux,
    [constructeur],
    [adresse]
FROM [dbo].[Fournisseur]
"@

# Extraire Systeme (un seul enregistrement)
Export-SystemeToJson -outputFile "systeme.json"

# Extraire Produit
Export-TableToJson -tableName "Produit" -outputFile "produits.json" -query @"
SELECT 
    [refe],
    [nom],
    [qte],
    [qteLimite],
    [remise],
    [remiseAchat],
    [TVA] as tva,
    [prix],
    [prixAchat],
    [visibilite]
FROM [dbo].[Produit]
"@

Write-Host ""
Write-Host "========================================" -ForegroundColor Magenta
Write-Host "Extraction terminée!" -ForegroundColor Magenta
Write-Host "Fichiers générés dans: $seedDataPath" -ForegroundColor Magenta
Write-Host "========================================" -ForegroundColor Magenta

