using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Soldes;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintSoldeFournisseur;

public class PrintSoldeFournisseurService(
    ILogger<PrintSoldeFournisseurService> _logger,
    ISoldesApiClient _soldesApiClient,
    IProvidersApiClient _providersApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintSoldeFournisseurModel, PrintSoldeFournisseurView> _printService)
{
    public async Task<Result<byte[]>> GenerateSoldeFournisseurPdfAsync(int fournisseurId, CancellationToken cancellationToken)
    {
        var soldeResponse = await _soldesApiClient.GetSoldeFournisseurAsync(fournisseurId, null, cancellationToken);
        if (soldeResponse.IsFailed)
        {
            return Result.Fail(soldeResponse.Errors);
        }

        ProviderResponse? provider = null;
        var providerResponse = await _providersApiClient.GetAsync(fournisseurId, cancellationToken);
        if (providerResponse.IsT0)
        {
            provider = providerResponse.AsT0;
        }

        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);
        if (appParametersResult.IsT1)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }

        var appParameters = appParametersResult.AsT0;
        var model = MapToModel(soldeResponse.Value, provider, appParameters);
        var printOptions = PreparePrintOptions(model, appParameters);
        var pdfBytes = await _printService.GeneratePdfAsync(model, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static PrintSoldeFournisseurModel MapToModel(
        SoldeFournisseurResponse solde,
        ProviderResponse? provider,
        GetAppParametersResponse appParameters)
    {
        return new PrintSoldeFournisseurModel
        {
            Company = new PrintCompanyInfo
            {
                Name = appParameters.NomSociete,
                Adresse = appParameters.Adresse,
                Tel = appParameters.Tel,
                Matricule = $"{appParameters.MatriculeFiscale}/{appParameters.CodeTva}/{appParameters.CodeCategorie}/{appParameters.EtbSecondaire}"
            },
            Provider = new PrintSoldeFournisseurInfo
            {
                Code = provider?.Code ?? solde.FournisseurId.ToString(),
                Name = solde.FournisseurNom,
                Contact = provider?.Tel ?? provider?.Mail ?? "-",
                Adresse = provider?.Adresse ?? string.Empty,
                Matricule = provider?.Matricule ?? string.Empty
            },
            Summary = new PrintSoldeFournisseurSummary
            {
                TotalFactures = solde.TotalFactures,
                TotalBonsReceptionNonFactures = solde.TotalBonsReceptionNonFactures,
                TotalFacturesAvoir = solde.TotalFacturesAvoir,
                TotalPaiements = solde.TotalPaiements,
                Solde = solde.Solde
            },
            Documents = solde.Documents.Select(d => new PrintSoldeFournisseurDocument
            {
                Type = d.Type,
                Numero = d.Numero,
                Date = d.Date,
                Montant = d.Montant
            }).ToList(),
            Payments = solde.Paiements.Select(p => new PrintSoldeFournisseurPayment
            {
                NumeroTransactionBancaire = p.NumeroTransactionBancaire,
                DatePaiement = p.DatePaiement,
                Montant = p.Montant,
                Methode = p.MethodePaiement,
                Factures = p.Factures?.ToList() ?? new List<FactureRattacheeSolde>()
            }).ToList(),
            GeneratedAt = DateTime.Now,
            DecimalPlaces = AmountHelper.DEFAULT_DECIMAL_PLACES
        };
    }

    private static SilkPdfOptions PreparePrintOptions(PrintSoldeFournisseurModel model, GetAppParametersResponse appParameters)
    {
        var options = SilkPdfOptions.Default;
        options.MarginTop = "80px";
        options.HeaderTemplate = $@"
<div style='font-family: Arial; font-size: 11px; width: 90%; margin: 0 auto;'>
    <table style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td style='width: 50%; text-align: left;'>
                <div style='font-weight: bold;'>{appParameters.NomSociete}</div>
                <div>{appParameters.Adresse}</div>
                <div>Tel: {appParameters.Tel}</div>
            </td>
            <td style='width: 50%; text-align: right;'>
                <div>Rapport solde fournisseur</div>
                <div>Fournisseur: {model.Provider.Name}</div>
                <div>Code: {model.Provider.Code}</div>
                <div>Page <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
        </tr>
    </table>
</div>";
        return options;
    }
}

