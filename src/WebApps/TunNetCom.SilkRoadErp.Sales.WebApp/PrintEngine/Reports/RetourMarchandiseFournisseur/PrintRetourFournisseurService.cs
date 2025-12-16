using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetourMarchandiseFournisseur;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.RetourMarchandiseFournisseur;

public class PrintRetourFournisseurService(
    ILogger<PrintRetourFournisseurService> _logger,
    IRetourMarchandiseFournisseurApiClient _retourApiClient,
    IProvidersApiClient _providersApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintRetourFournisseurModel, PrintRetourFournisseurView> _printService)
{
    public async Task<Result<byte[]>> GenerateRetourFournisseurPdfAsync(
        int retourNumber,
        CancellationToken cancellationToken)
    {
        var retourResponse = await _retourApiClient.GetRetourMarchandiseFournisseurAsync(
            retourNumber,
            cancellationToken);

        if (retourResponse.IsFailed || retourResponse.Value == null)
        {
            return Result.Fail("Retour fournisseur non trouvé");
        }

        var printModel = MapToPrintModel(retourResponse.Value);

        // Load provider if available
        if (printModel.ProviderId.HasValue)
        {
            var providerResult = await _providersApiClient.GetAsync(
                printModel.ProviderId.Value,
                cancellationToken);
            
            providerResult.Switch(
                provider => 
                {
                    printModel.Provider = new PrintProviderInfo
                    {
                        Id = provider.Id,
                        Nom = provider.Nom,
                        Adresse = provider.Adresse,
                        Tel = provider.Tel,
                        Matricule = provider.Matricule,
                        Code = provider.Code
                    };
                },
                notFound => { /* Provider not found, continue without it */ }
            );
        }

        var getAppParametersResponse = await FetchAppParametersAsync(cancellationToken);
        if (getAppParametersResponse.IsFailed)
        {
            return Result.Fail("Impossible de récupérer les paramètres de l'application");
        }

        printModel.VatRate0 = (double)getAppParametersResponse.Value.VatRate0;
        printModel.VatRate7 = (double)getAppParametersResponse.Value.VatRate7;
        printModel.VatRate13 = (double)getAppParametersResponse.Value.VatRate13;
        printModel.VatRate19 = (double)getAppParametersResponse.Value.VatRate19;
        CalculateTotalAmounts(printModel, getAppParametersResponse.Value);

        var printOptions = PreparePrintOptions(printModel, getAppParametersResponse.Value);

        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static void CalculateTotalAmounts(PrintRetourFournisseurModel printModel, GetAppParametersResponse appParameters)
    {
        printModel.Base19 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (double)appParameters.VatRate19).Sum(l => l.TotHt));
        printModel.Base13 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (double)appParameters.VatRate13).Sum(l => l.TotHt));
        printModel.Base7 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (double)appParameters.VatRate7).Sum(l => l.TotHt));
        printModel.Tva19 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (double)appParameters.VatRate19).Sum(l => l.TotTtc - l.TotHt));
        printModel.Tva13 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (double)appParameters.VatRate13).Sum(l => l.TotTtc - l.TotHt));
        printModel.Tva7 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (double)appParameters.VatRate7).Sum(l => l.TotTtc - l.TotHt));
        
        printModel.TotalExcludingTax = DecimalHelper.RoundAmount(printModel.Base19 + printModel.Base13 + printModel.Base7);
        printModel.TotalVat = DecimalHelper.RoundAmount(printModel.Tva19 + printModel.Tva13 + printModel.Tva7);
        printModel.TotalTTC = DecimalHelper.RoundAmount(printModel.TotalExcludingTax + printModel.TotalVat);
    }

    private static PrintRetourFournisseurModel MapToPrintModel(RetourMarchandiseFournisseurResponse retour)
    {
        return new PrintRetourFournisseurModel
        {
            Num = retour.Num,
            Date = retour.Date,
            ProviderId = retour.IdFournisseur,
            Statut = retour.Statut.ToString(),
            StatutLibelle = retour.StatutLibelle,
            TotalExcludingTax = retour.TotHTva,
            TotalVat = retour.TotTva,
            TotalTTC = retour.NetPayer,
            Lines = retour.Lines.Select(item => new PrintRetourLineModel
            {
                Id = item.IdLigne,
                RefProduit = item.RefProduit,
                DesignationLi = item.DesignationLi,
                QteLi = item.QteLi,
                PrixHt = item.PrixHt,
                Remise = item.Remise,
                TotHt = item.TotHt,
                Tva = item.Tva,
                TotTtc = item.TotTtc
            }).ToList()
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintRetourFournisseurModel printModel,
        GetAppParametersResponse appParameters)
    {
        // Build provider info section conditionally
        var providerInfo = "";
        if (printModel.Provider != null)
        {
            providerInfo = $@"
                <div style='font-weight: bold;'>Fournisseur :</div>
                <div style='font-weight: bold;'>{printModel.Provider.Nom ?? ""}</div>";
            
            if (!string.IsNullOrEmpty(printModel.Provider.Adresse))
            {
                providerInfo += $@"<div>Adresse: {printModel.Provider.Adresse}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Provider.Tel))
            {
                providerInfo += $@"<div>Tél: {printModel.Provider.Tel}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Provider.Code))
            {
                providerInfo += $@"<div>Code TVA: {printModel.Provider.Code}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Provider.Matricule))
            {
                providerInfo += $@"<div>Matricule: {printModel.Provider.Matricule}</div>";
            }
        }

        var headerContent = $@"
<div style='font-family: Arial; font-size: 12px; width: 90%; margin: 0 auto;'>
    <table style='width: 100%; border-collapse: collapse;'>
        <tr>
            <!-- Left Column - Company Info -->
            <td style='width: 35%; text-align: left; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>{appParameters.NomSociete}</div>
                <div>{appParameters.Adresse}</div>
                <div>Tel: {appParameters.Tel}</div>
                <div>TVA: {$"{appParameters.MatriculeFiscale}/{appParameters.CodeTva}/{appParameters.CodeCategorie}/{appParameters.EtbSecondaire}"}</div>
            </td>
            
            <!-- Middle Column - Document Info -->
            <td style='width: 35%; text-align: center; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>BON DE RETOUR FOURNISSEUR</div>
                <div style='font-weight: bold;'>N°: {printModel.Num}</div>
                <div>Date: {printModel.Date.ToString("dd/MM/yyyy")}</div>
                <div>Statut: {printModel.StatutLibelle}</div>
                <div>Page: <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
            
            <!-- Right Column - Provider Info -->
            <td style='width: 35%; text-align: left; vertical-align: top; padding-left: 50px;'>
                {providerInfo}
            </td>
        </tr>
    </table>
</div>";

        var printOptions = SilkPdfOptions.Default;
        printOptions.MarginTop = "150px";
        printOptions.HeaderTemplate = headerContent;
        return printOptions;
    }

    private async Task<Result<GetAppParametersResponse>> FetchAppParametersAsync(CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);

        if (appParametersResult.IsT1)
        {
            return Result.Fail("Impossible de récupérer les paramètres de l'application");
        }

        var getAppParametersResponse = appParametersResult.AsT0;
        _logger.LogInformation("Paramètres de l'application récupérés avec succès.");

        return Result.Ok(getAppParametersResponse);
    }
}
