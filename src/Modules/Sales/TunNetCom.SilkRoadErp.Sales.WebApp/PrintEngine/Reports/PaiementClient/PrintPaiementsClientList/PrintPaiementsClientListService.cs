using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.PaiementClient.PrintPaiementsClientList;

public class PrintPaiementsClientListService(
    ILogger<PrintPaiementsClientListService> _logger,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintPaiementsClientListModel, PrintPaiementsClientListView> _printService)
{
    public async Task<Result<byte[]>> GeneratePdfAsync(
        IEnumerable<PaiementClientResponse> paiements,
        int decimalPlaces,
        CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);
        if (appParametersResult.IsT1)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }

        var appParameters = appParametersResult.AsT0;
        var list = paiements.ToList();
        var model = MapToModel(list, appParameters, decimalPlaces);

        var printOptions = PreparePrintOptions(model, appParameters);
        var pdfBytes = await _printService.GeneratePdfAsync(model, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static PrintPaiementsClientListModel MapToModel(
        List<PaiementClientResponse> paiements,
        GetAppParametersResponse appParameters,
        int decimalPlaces)
    {
        var rows = paiements.Select(p => new PrintPaiementClientRow
        {
            ClientNom = p.ClientNom ?? string.Empty,
            Montant = p.Montant,
            DatePaiement = p.DatePaiement,
            MethodePaiement = p.MethodePaiement,
            DateEcheance = p.DateEcheance
        }).ToList();

        return new PrintPaiementsClientListModel
        {
            Company = new PrintCompanyInfo
            {
                Name = appParameters.NomSociete,
                Adresse = appParameters.Adresse,
                Tel = appParameters.Tel,
                Matricule = $"{appParameters.MatriculeFiscale}/{appParameters.CodeTva}/{appParameters.CodeCategorie}/{appParameters.EtbSecondaire}"
            },
            Rows = rows,
            GeneratedAt = DateTime.Now,
            DecimalPlaces = decimalPlaces,
            TotalMontant = rows.Sum(r => r.Montant)
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintPaiementsClientListModel model,
        GetAppParametersResponse appParameters)
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
                <div>Liste des Paiements Clients</div>
                <div>Page <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
        </tr>
    </table>
</div>";
        return options;
    }
}
