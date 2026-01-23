using System.Linq;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Soldes;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintSoldeClient;

public class PrintSoldeClientService(
    ILogger<PrintSoldeClientService> _logger,
    ISoldesApiClient _soldesApiClient,
    ICustomersApiClient _customersApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintSoldeClientModel, PrintSoldeClientView> _printService)
{
    public async Task<Result<byte[]>> GenerateSoldeClientPdfAsync(int clientId, bool showOnlyMissingProducts, CancellationToken cancellationToken)
    {
        var soldeResponse = await _soldesApiClient.GetSoldeClientAsync(clientId, null, cancellationToken);
        if (soldeResponse.IsFailed)
        {
            return Result.Fail(soldeResponse.Errors);
        }

        var solde = soldeResponse.Value;
        var customer = await _customersApiClient.GetCustomerByIdAsync(clientId, cancellationToken);

        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);
        if (appParametersResult.IsT1)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }

        var appParameters = appParametersResult.AsT0;
        var model = MapToModel(solde, customer, appParameters, showOnlyMissingProducts);

        var printOptions = PreparePrintOptions(model, appParameters);
        var pdfBytes = await _printService.GeneratePdfAsync(model, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static PrintSoldeClientModel MapToModel(
        SoldeClientResponse solde,
        CustomerResponse? customer,
        GetAppParametersResponse appParameters,
        bool showOnlyMissingProducts)
    {
        return new PrintSoldeClientModel
        {
            Company = new PrintCompanyInfo
            {
                Name = appParameters.NomSociete,
                Adresse = appParameters.Adresse,
                Tel = appParameters.Tel,
                Matricule = $"{appParameters.MatriculeFiscale}/{appParameters.CodeTva}/{appParameters.CodeCategorie}/{appParameters.EtbSecondaire}"
            },
            Client = new PrintSoldeClientInfo
            {
                Code = customer?.Code ?? solde.ClientId.ToString(),
                Name = solde.ClientNom,
                Contact = customer?.Tel ?? customer?.Mail ?? "-",
                Adresse = customer?.Adresse ?? string.Empty,
                Matricule = customer?.Matricule ?? string.Empty
            },
            Summary = new PrintSoldeSummary
            {
                TotalFactures = solde.TotalFactures,
                TotalBonsLivraisonNonFactures = solde.TotalBonsLivraisonNonFactures,
                TotalAvoirs = solde.TotalAvoirs,
                TotalFacturesAvoir = solde.TotalFacturesAvoir,
                TotalPaiements = solde.TotalPaiements,
                Solde = solde.Solde
            },
            Documents = solde.Documents.Select(d => MapDocument(d, showOnlyMissingProducts)).ToList(),
            Payments = solde.Paiements.Select(p => new PrintSoldeClientPayment
            {
                NumeroTransactionBancaire = p.NumeroTransactionBancaire,
                DatePaiement = p.DatePaiement,
                Montant = p.Montant,
                Methode = p.MethodePaiement,
                NumeroChequeTraite = p.NumeroChequeTraite,
                BanqueNom = p.BanqueNom,
                DateEcheance = p.DateEcheance
            }).ToList(),
            GeneratedAt = DateTime.Now,
            DecimalPlaces = AmountHelper.DEFAULT_DECIMAL_PLACES,
            ShowOnlyMissingProducts = showOnlyMissingProducts
        };
    }

    private static PrintSoldeClientDocument MapDocument(DocumentSoldeClient document, bool showOnlyMissingProducts)
    {
        return new PrintSoldeClientDocument
        {
            Type = document.Type,
            Numero = document.Numero,
            Date = document.Date,
            Montant = document.Montant,
            HasMissingQuantities = document.HasQuantitesNonLivrees,
            DeliveryNotes = document.BonsLivraison?.Select(bl => MapDeliveryNote(bl, showOnlyMissingProducts)).ToList() ?? new List<PrintSoldeClientDeliveryNote>()
        };
    }

    private static PrintSoldeClientDeliveryNote MapDeliveryNote(DocumentSoldeClient deliveryNote, bool showOnlyMissingProducts)
    {
        var lines = deliveryNote.LignesBl?.ToList() ?? new List<LigneBlSoldeClient>();
        
        if (showOnlyMissingProducts)
        {
            lines = lines.Where(line => line.QuantiteNonLivree > 0).ToList();
        }

        return new PrintSoldeClientDeliveryNote
        {
            Numero = deliveryNote.Numero,
            Date = deliveryNote.Date,
            Montant = deliveryNote.Montant,
            Lines = lines.Select(line => new PrintSoldeClientDeliveryLine
            {
                RefProduit = line.RefProduit,
                Designation = line.DesignationLi,
                Quantite = line.QteLi,
                QuantiteLivree = line.QteLivree,
                QuantiteNonLivree = line.QuantiteNonLivree
            }).ToList()
        };
    }

    private static SilkPdfOptions PreparePrintOptions(PrintSoldeClientModel model, GetAppParametersResponse appParameters)
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
                <div>Rapport solde client</div>
                <div>Client: {model.Client.Name}</div>
                <div>Code: {model.Client.Code}</div>
                <div>Page <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
        </tr>
    </table>
</div>";
        return options;
    }
}

