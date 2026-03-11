using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Avoirs;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Avoirs;

public class PrintAvoirService(
    ILogger<PrintAvoirService> _logger,
    IAvoirsApiClient _avoirsApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintAvoirModel, PrintAvoirView> _printService)
{
    public async Task<Result<byte[]>> GenerateAvoirPdfAsync(
        int avoirNum,
        CancellationToken cancellationToken,
        bool includeHeader = true)
    {
        var fullResponse = await _avoirsApiClient.GetFullAvoirAsync(avoirNum, cancellationToken);

        if (fullResponse.IsFailed || fullResponse.Value == null)
        {
            return Result.Fail("Avoir client non trouvé");
        }

        var printModel = MapToPrintModel(fullResponse.Value);

        if (fullResponse.Value.Client != null)
        {
            var c = fullResponse.Value.Client;
            printModel.Client = new PrintAvoirClientInfo
            {
                Id = c.Id,
                Nom = c.Nom ?? "",
                Adresse = c.Adresse,
                Tel = c.Tel,
                Matricule = c.Matricule,
                Code = c.Code
            };
        }

        var appParamsResult = await FetchAppParametersAsync(cancellationToken);
        if (appParamsResult.IsFailed)
        {
            return Result.Fail("Impossible de récupérer les paramètres de l'application");
        }

        var appParams = appParamsResult.Value;
        printModel.VatRate0 = (double)appParams.VatRate0;
        printModel.VatRate7 = (double)appParams.VatRate7;
        printModel.VatRate13 = (double)appParams.VatRate13;
        printModel.VatRate19 = (double)appParams.VatRate19;
        CalculateTotalAmounts(printModel, appParams);

        var printOptions = PreparePrintOptions(printModel, appParams, includeHeader);
        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static PrintAvoirModel MapToPrintModel(FullAvoirResponse r)
    {
        return new PrintAvoirModel
        {
            Num = r.Num,
            Date = r.Date,
            ClientId = r.ClientId,
            TotalExcludingTax = r.TotalExcludingTaxAmount,
            TotalVat = r.TotalVATAmount,
            TotalTTC = r.TotalIncludingTaxAmount,
            Lines = r.Lines.Select(l => new PrintAvoirLineModel
            {
                IdLi = l.IdLi,
                RefProduit = l.RefProduit,
                DesignationLi = l.DesignationLi ?? "",
                QteLi = l.QteLi,
                PrixHt = l.PrixHt,
                Remise = l.Remise,
                TotHt = l.TotHt,
                Tva = l.Tva,
                TotTtc = l.TotTtc
            }).ToList()
        };
    }

    private static void CalculateTotalAmounts(PrintAvoirModel model, GetAppParametersResponse appParams)
    {
        model.Base19 = DecimalHelper.RoundAmount(model.Lines.Where(l => l.Tva == (double)appParams.VatRate19).Sum(l => l.TotHt));
        model.Base13 = DecimalHelper.RoundAmount(model.Lines.Where(l => l.Tva == (double)appParams.VatRate13).Sum(l => l.TotHt));
        model.Base7 = DecimalHelper.RoundAmount(model.Lines.Where(l => l.Tva == (double)appParams.VatRate7).Sum(l => l.TotHt));
        model.Tva19 = DecimalHelper.RoundAmount(model.Lines.Where(l => l.Tva == (double)appParams.VatRate19).Sum(l => l.TotTtc - l.TotHt));
        model.Tva13 = DecimalHelper.RoundAmount(model.Lines.Where(l => l.Tva == (double)appParams.VatRate13).Sum(l => l.TotTtc - l.TotHt));
        model.Tva7 = DecimalHelper.RoundAmount(model.Lines.Where(l => l.Tva == (double)appParams.VatRate7).Sum(l => l.TotTtc - l.TotHt));
        model.TotalExcludingTax = DecimalHelper.RoundAmount(model.Base19 + model.Base13 + model.Base7);
        model.TotalVat = DecimalHelper.RoundAmount(model.Tva19 + model.Tva13 + model.Tva7);
        model.TotalTTC = DecimalHelper.RoundAmount(model.TotalExcludingTax + model.TotalVat);
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintAvoirModel printModel,
        GetAppParametersResponse appParameters,
        bool includeHeader = true)
    {
        var printOptions = SilkPdfOptions.Default;
        if (!includeHeader)
            return printOptions;

        var clientInfo = "";
        if (printModel.Client != null)
        {
            clientInfo = $@"
                <div style='font-weight: bold;'>Client :</div>
                <div style='font-weight: bold;'>{printModel.Client.Nom}</div>";
            if (!string.IsNullOrEmpty(printModel.Client.Adresse))
                clientInfo += $@"<div>Adresse: {printModel.Client.Adresse}</div>";
            if (!string.IsNullOrEmpty(printModel.Client.Tel))
                clientInfo += $@"<div>Tél: {printModel.Client.Tel}</div>";
            if (!string.IsNullOrEmpty(printModel.Client.Code))
                clientInfo += $@"<div>Code TVA: {printModel.Client.Code}</div>";
            if (!string.IsNullOrEmpty(printModel.Client.Matricule))
                clientInfo += $@"<div>Matricule: {printModel.Client.Matricule}</div>";
        }

        var headerContent = $@"
<div style='font-family: Arial; font-size: 12px; width: 90%; margin: 0 auto;'>
    <table style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td style='width: 35%; text-align: left; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>{appParameters.NomSociete}</div>
                <div>{appParameters.Adresse}</div>
                <div>Tel: {appParameters.Tel}</div>
                <div>TVA: {$"{appParameters.MatriculeFiscale}/{appParameters.CodeTva}/{appParameters.CodeCategorie}/{appParameters.EtbSecondaire}"}</div>
            </td>
            <td style='width: 35%; text-align: center; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>AVOIR</div>
                <div style='font-weight: bold;'>N°: {printModel.Num}</div>
                <div>Date: {printModel.Date:dd/MM/yyyy}</div>
                <div>Page: <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
            <td style='width: 35%; text-align: left; vertical-align: top; padding-left: 50px;'>
                {clientInfo}
            </td>
        </tr>
    </table>
</div>";

        printOptions.MarginTop = "150px";
        printOptions.HeaderTemplate = headerContent;
        return printOptions;
    }

    private async Task<Result<GetAppParametersResponse>> FetchAppParametersAsync(CancellationToken cancellationToken)
    {
        var result = await _appParametersClient.GetAppParametersAsync(cancellationToken);
        if (result.IsT1)
            return Result.Fail("Impossible de récupérer les paramètres de l'application");
        _logger.LogInformation("Paramètres de l'application récupérés avec succès.");
        return Result.Ok(result.AsT0);
    }
}
