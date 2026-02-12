using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.ClotureCaisse;

public class PrintClotureCaisseService(
    ILogger<PrintClotureCaisseService> _logger,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintClotureCaisseModel, PrintClotureCaisseView> _printService)
{
    public async Task<Result<byte[]>> GeneratePdfAsync(
        DateTime dateDuJour,
        IEnumerable<PaiementClientResponse> paiements,
        decimal sommeEspece,
        decimal sommeCheque,
        decimal sommeTraite,
        decimal sommeVirement,
        decimal sommeTpe,
        IEnumerable<GetDeliveryNoteBaseInfos> blList,
        decimal totalBl,
        IEnumerable<AvoirBaseInfo> avoirsList,
        decimal totalAvoirs,
        int decimalPlaces,
        CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);
        if (appParametersResult.IsT1)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }

        var appParameters = appParametersResult.AsT0;
        var model = MapToModel(
            dateDuJour,
            paiements.ToList(),
            sommeEspece,
            sommeCheque,
            sommeTraite,
            sommeVirement,
            sommeTpe,
            blList.ToList(),
            totalBl,
            avoirsList.ToList(),
            totalAvoirs,
            appParameters,
            decimalPlaces);

        var printOptions = PreparePrintOptions(model, appParameters);
        var pdfBytes = await _printService.GeneratePdfAsync(model, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static PrintClotureCaisseModel MapToModel(
        DateTime dateDuJour,
        List<PaiementClientResponse> paiements,
        decimal sommeEspece,
        decimal sommeCheque,
        decimal sommeTraite,
        decimal sommeVirement,
        decimal sommeTpe,
        List<GetDeliveryNoteBaseInfos> blList,
        decimal totalBl,
        List<AvoirBaseInfo> avoirsList,
        decimal totalAvoirs,
        GetAppParametersResponse appParameters,
        int decimalPlaces)
    {
        var paiementsRows = paiements.Select(p => new PrintClotureCaissePaiementRow
        {
            ClientNom = p.ClientNom ?? string.Empty,
            DatePaiement = p.DatePaiement,
            MethodePaiement = p.MethodePaiement,
            Montant = p.Montant
        }).ToList();

        var blRows = blList.Select(b => new PrintClotureCaisseBlRow
        {
            Number = b.Number,
            Date = b.Date.DateTime,
            CustomerName = b.CustomerName,
            NetAmount = b.NetAmount
        }).ToList();

        var avoirsRows = avoirsList.Select(a => new PrintClotureCaisseAvoirRow
        {
            Num = a.Num,
            Date = a.Date.DateTime,
            ClientName = a.ClientName,
            TotalIncludingTaxAmount = a.TotalIncludingTaxAmount
        }).ToList();

        return new PrintClotureCaisseModel
        {
            Company = new PrintCompanyInfo
            {
                Name = appParameters.NomSociete,
                Adresse = appParameters.Adresse,
                Tel = appParameters.Tel,
                Matricule = $"{appParameters.MatriculeFiscale}/{appParameters.CodeTva}/{appParameters.CodeCategorie}/{appParameters.EtbSecondaire}"
            },
            DateDuJour = dateDuJour,
            GeneratedAt = DateTime.Now,
            DecimalPlaces = decimalPlaces,
            PaiementsRows = paiementsRows,
            SommeEspece = sommeEspece,
            SommeCheque = sommeCheque,
            SommeTraite = sommeTraite,
            SommeVirement = sommeVirement,
            SommeTpe = sommeTpe,
            TotalPaiements = paiements.Sum(p => p.Montant),
            BlRows = blRows,
            TotalBL = totalBl,
            AvoirsRows = avoirsRows,
            TotalAvoirs = totalAvoirs,
            Resultat = totalBl - totalAvoirs
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintClotureCaisseModel model,
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
                <div>Clôture de caisse - {model.DateDuJour.ToString("dd/MM/yyyy")}</div>
                <div>Page <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
        </tr>
    </table>
</div>";
        return options;
    }
}
