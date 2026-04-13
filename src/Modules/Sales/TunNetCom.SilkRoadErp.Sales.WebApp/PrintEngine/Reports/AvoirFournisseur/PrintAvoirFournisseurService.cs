using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.AvoirFournisseur;

public class PrintAvoirFournisseurService(
    ILogger<PrintAvoirFournisseurService> _logger,
    IAvoirFournisseurApiClient _avoirFournisseurApiClient,
    IProvidersApiClient _providersApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintAvoirFournisseurModel, PrintAvoirFournisseurView> _printService)
{
    public async Task<Result<byte[]>> GenerateAvoirFournisseurPdfAsync(
        int avoirFournisseurId,
        CancellationToken cancellationToken,
        bool includeHeader = true)
    {
        var fullResponse = await _avoirFournisseurApiClient.GetFullAvoirFournisseurAsync(avoirFournisseurId, cancellationToken);

        if (fullResponse.IsFailed || fullResponse.Value == null)
        {
            return Result.Fail("Avoir fournisseur non trouvé");
        }

        var printModel = MapToPrintModel(fullResponse.Value);

        if (printModel.ProviderId.HasValue)
        {
            var providerResult = await _providersApiClient.GetAsync(printModel.ProviderId.Value, cancellationToken);
            providerResult.Switch(
                provider =>
                {
                    printModel.Provider = new PrintAvoirFournisseurProviderInfo
                    {
                        Id = provider.Id,
                        Nom = provider.Nom,
                        Adresse = provider.Adresse,
                        Tel = provider.Tel,
                        Matricule = provider.Matricule,
                        Code = provider.Code
                    };
                },
                _ => { /* Provider not found */ });
        }
        else if (fullResponse.Value.Fournisseur != null)
        {
            var f = fullResponse.Value.Fournisseur;
            printModel.Provider = new PrintAvoirFournisseurProviderInfo
            {
                Id = f.Id,
                Nom = f.Name ?? "",
                Adresse = f.Adress,
                Tel = f.Phone,
                Matricule = f.RegistrationNumber,
                Code = f.Code
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

    private static PrintAvoirFournisseurModel MapToPrintModel(FullAvoirFournisseurResponse r)
    {
        return new PrintAvoirFournisseurModel
        {
            Id = r.Id,
            NumAvoirChezFournisseur = r.NumAvoirChezFournisseur,
            Date = r.Date,
            ProviderId = r.FournisseurId,
            Statut = r.Statut.ToString(),
            StatutLibelle = r.StatutLibelle,
            TotalExcludingTax = r.TotalExcludingTaxAmount,
            TotalVat = r.TotalVATAmount,
            TotalTTC = r.TotalIncludingTaxAmount,
            Lines = r.Lines.Select(l => new PrintAvoirFournisseurLineModel
            {
                IdLi = l.IdLi,
                RefProduit = l.RefProduit,
                DesignationLi = l.DesignationLi,
                QteLi = l.QteLi,
                PrixHt = l.PrixHt,
                Remise = l.Remise,
                TotHt = l.TotHt,
                Tva = l.Tva,
                TotTtc = l.TotTtc
            }).ToList()
        };
    }

    private static void CalculateTotalAmounts(PrintAvoirFournisseurModel model, GetAppParametersResponse appParams)
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
        PrintAvoirFournisseurModel printModel,
        GetAppParametersResponse appParameters,
        bool includeHeader = true)
    {
        var printOptions = SilkPdfOptions.Default;
        if (!includeHeader)
            return printOptions;

        var providerInfo = "";
        if (printModel.Provider != null)
        {
            providerInfo = $@"
                <div style='font-weight: bold;'>Fournisseur :</div>
                <div style='font-weight: bold;'>{printModel.Provider.Nom}</div>";
            if (!string.IsNullOrEmpty(printModel.Provider.Adresse))
                providerInfo += $@"<div>Adresse: {printModel.Provider.Adresse}</div>";
            if (!string.IsNullOrEmpty(printModel.Provider.Tel))
                providerInfo += $@"<div>Tél: {printModel.Provider.Tel}</div>";
            if (!string.IsNullOrEmpty(printModel.Provider.Code))
                providerInfo += $@"<div>Code TVA: {printModel.Provider.Code}</div>";
            if (!string.IsNullOrEmpty(printModel.Provider.Matricule))
                providerInfo += $@"<div>Matricule: {printModel.Provider.Matricule}</div>";
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
                <div style='font-weight: bold; font-size: 14px;'>AVOIR FOURNISSEUR</div>
                <div style='font-weight: bold;'>N°: {printModel.NumAvoirChezFournisseur}</div>
                <div>Date: {printModel.Date:dd/MM/yyyy}</div>
                <div>Statut: {printModel.StatutLibelle}</div>
                <div>Page: <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
            <td style='width: 35%; text-align: left; vertical-align: top; padding-left: 50px;'>
                {providerInfo}
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
