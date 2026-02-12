using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintRestesALivrer;

public class PrintRestesALivrerService(
    ILogger<PrintRestesALivrerService> _logger,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintRestesALivrerModel, PrintRestesALivrerView> _printService)
{
    public async Task<Result<byte[]>> GeneratePdfAsync(
        RestesALivrerParClientResponse data,
        int decimalPlaces,
        CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);
        if (appParametersResult.IsT1)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }

        var appParameters = appParametersResult.AsT0;
        var model = MapToModel(data, appParameters, decimalPlaces);

        var printOptions = PreparePrintOptions(model, appParameters);
        var pdfBytes = await _printService.GeneratePdfAsync(model, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static PrintRestesALivrerModel MapToModel(
        RestesALivrerParClientResponse data,
        GetAppParametersResponse appParameters,
        int decimalPlaces)
    {
        var clients = data.Clients.Select(c => new ClientRestesALivrerPrintItem
        {
            ClientNom = c.ClientNom,
            Solde = c.Solde,
            Lignes = c.LignesRestesALivrer.Select(l => new LigneResteaLivrerPrint
            {
                RefProduit = l.RefProduit,
                Designation = l.DesignationLi,
                QuantiteRestante = l.QuantiteRestante
            }).ToList()
        }).ToList();

        return new PrintRestesALivrerModel
        {
            Company = new PrintCompanyInfo
            {
                Name = appParameters.NomSociete,
                Adresse = appParameters.Adresse,
                Tel = appParameters.Tel,
                Matricule = $"{appParameters.MatriculeFiscale}/{appParameters.CodeTva}/{appParameters.CodeCategorie}/{appParameters.EtbSecondaire}"
            },
            Clients = clients,
            GeneratedAt = DateTime.Now,
            DecimalPlaces = decimalPlaces
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintRestesALivrerModel model,
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
                <div>Restes à livrer par client</div>
                <div>Page <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
        </tr>
    </table>
</div>";
        return options;
    }
}
