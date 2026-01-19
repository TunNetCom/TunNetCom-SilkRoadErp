using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Banque;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.PaiementFournisseur.PrintTraite;

public class PrintTraiteService(
    ILogger<PrintTraiteService> _logger,
    IPaiementFournisseurApiClient _paiementFournisseurApiClient,
    IProvidersApiClient _providersApiClient,
    IAppParametersClient _appParametersClient,
    IBanqueApiClient _banqueApiClient,
    IPrintPdfService<PrintTraiteModel, PrintTraiteView> _printService)
{
    public async Task<Result<byte[]>> GenerateTraitePdfAsync(
        int paiementId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Traite PDF for PaiementFournisseur {PaiementId}", paiementId);

        // Récupérer le paiement
        var paiementResult = await _paiementFournisseurApiClient.GetPaiementFournisseurAsync(
            paiementId,
            cancellationToken);

        if (paiementResult.IsFailed)
        {
            _logger.LogError("Failed to retrieve PaiementFournisseur {PaiementId}", paiementId);
            return Result.Fail("paiement_fournisseur_not_found");
        }

        var paiement = paiementResult.Value;

        // Vérifier que c'est bien une Traite
        if (paiement.MethodePaiement != "Traite")
        {
            _logger.LogWarning("PaiementFournisseur {PaiementId} is not a Traite (MethodePaiement: {MethodePaiement})", 
                paiementId, paiement.MethodePaiement);
            return Result.Fail("paiement_is_not_traite");
        }

        // Récupérer le fournisseur (tiré)
        var fournisseurResult = await _providersApiClient.GetAsync(
            paiement.FournisseurId,
            cancellationToken);

        if (fournisseurResult.IsT1 || fournisseurResult.AsT0 == null)
        {
            _logger.LogError("Failed to retrieve Fournisseur {FournisseurId}", paiement.FournisseurId);
            return Result.Fail("fournisseur_not_found");
        }

        var fournisseur = fournisseurResult.AsT0;

        // Récupérer les paramètres de l'application (tireur)
        var appParametersResult = await FetchAppParametersAsync(cancellationToken);
        if (appParametersResult.IsFailed)
        {
            _logger.LogError("Failed to retrieve app parameters");
            return Result.Fail("failed_to_retrieve_app_parameters");
        }

        var appParameters = appParametersResult.Value;

        // Récupérer la banque si disponible
        BanqueResponse? banque = null;
        if (paiement.BanqueId.HasValue)
        {
            try
            {
                var banques = await _banqueApiClient.GetBanquesAsync(cancellationToken);
                banque = banques.FirstOrDefault(b => b.Id == paiement.BanqueId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve banque {BanqueId}, continuing without banque info", 
                    paiement.BanqueId.Value);
            }
        }

        // Créer le modèle
        var printModel = new PrintTraiteModel
        {
            PaiementId = paiement.Id,
            NumeroPaiement = paiement.NumeroTransactionBancaire,
            Montant = paiement.Montant,
            DateCreation = paiement.DatePaiement,
            DateEcheance = paiement.DateEcheance,
            NumeroChequeTraite = paiement.NumeroChequeTraite,
            
            Tireur = new TireurModel
            {
                Nom = appParameters.NomSociete,
                Adresse = appParameters.Adresse,
                Tel = appParameters.Tel,
                MatriculeFiscale = appParameters.MatriculeFiscale,
                CodeTva = appParameters.CodeTva,
                CodeCategorie = appParameters.CodeCategorie,
                EtbSecondaire = appParameters.EtbSecondaire
            },
            
            Tire = new TireModel
            {
                Id = fournisseur.Id,
                Nom = fournisseur.Nom,
                Adresse = fournisseur.Adresse,
                Tel = fournisseur.Tel,
                Matricule = fournisseur.Matricule,
                Code = fournisseur.Code,
                // Utiliser le RIB de l'entreprise (Tireur) depuis les paramètres système
                RibCodeEtab = appParameters.RibCodeEtab,
                RibCodeAgence = appParameters.RibCodeAgence,
                RibNumeroCompte = appParameters.RibNumeroCompte,
                RibCle = appParameters.RibCle
            },
            
            Banque = new BanqueModel
            {
                Nom = appParameters.BanqueEntreprise ?? ""
            }
        };

        // Convertir le montant en lettres
        printModel.MontantEnLettres = ConvertMontantToLetters(printModel.Montant);

        // Préparer les options PDF
        var printOptions = PreparePrintOptions();

        // Générer le PDF
        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        _logger.LogInformation("Successfully generated Traite PDF for PaiementFournisseur {PaiementId}", paiementId);
        return Result.Ok(pdfBytes);
    }

    private static string ConvertMontantToLetters(decimal montant)
    {
        // Utiliser la même logique que les autres documents via AmountHelper
        return AmountHelper.ConvertFloatToFrenchToWords(montant, "");
    }

    private static SilkPdfOptions PreparePrintOptions()
    {
        // Format A4 portrait (210mm x 297mm)
        var printOptions = new SilkPdfOptions
        {
            Width = "210mm",
            Height = "297mm",
            PreferCSSPageSize = true,
            PrintBackground = true,
            MarginTop = "0mm",
            MarginBottom = "0mm",
            MarginLeft = "0mm",
            MarginRight = "0mm"
        };

        return printOptions;
    }

    private async Task<Result<GetAppParametersResponse>> FetchAppParametersAsync(CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);

        if (appParametersResult.IsT1)
        {
            return Result.Fail("Failed to retrieve app parameters");
        }

        var getAppParametersResponse = appParametersResult.AsT0;
        _logger.LogInformation("Successfully retrieved app parameters.");

        return Result.Ok(getAppParametersResponse);
    }
}

