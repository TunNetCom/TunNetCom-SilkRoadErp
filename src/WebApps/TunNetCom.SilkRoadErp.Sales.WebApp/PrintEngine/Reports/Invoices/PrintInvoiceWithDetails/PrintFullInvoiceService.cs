using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;
namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;

public class PrintFullInvoiceService(
    ILogger<PrintRetenuSourceService> _logger,
    IInvoicesApiClient _invoicesService,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintFullInvoiceModel, PrintFullInvoiceView> _printService)
{
    public async Task<Result<byte[]>> GenerateInvoicePdfAsync(
    int invoiceId,
    CancellationToken cancellationToken,
    bool isDuplicata = false,
    bool includeHeader = true)
    {
        var invoiceResponse = await _invoicesService.GetFullInvoiceByIdAsync(invoiceId, cancellationToken);

        if (invoiceResponse.IsFailed)
        {
            return Result.Fail(invoiceResponse.Errors);
        }

        PrintFullInvoiceModel printInvoiceWithDetailsModel = invoiceResponse.Value.Adapt<PrintFullInvoiceModel>();

        var getAppParametersResponse = await FetchAppParametersAsync(cancellationToken);

        if (getAppParametersResponse.IsFailed)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }
        printInvoiceWithDetailsModel.Timbre = getAppParametersResponse.Value.Timbre;
        printInvoiceWithDetailsModel.VatRate0 = getAppParametersResponse.Value.VatRate0;
        printInvoiceWithDetailsModel.VatRate7 = getAppParametersResponse.Value.VatRate7;
        printInvoiceWithDetailsModel.VatRate13 = getAppParametersResponse.Value.VatRate13;
        printInvoiceWithDetailsModel.VatRate19 = getAppParametersResponse.Value.VatRate19;
        printInvoiceWithDetailsModel.Rib = getAppParametersResponse.Value.Rib;
        CalculateTotalAmounts(printInvoiceWithDetailsModel, getAppParametersResponse.Value.Timbre, getAppParametersResponse.Value);

        SilkPdfOptions printOptions = PreparePrintOptions(printInvoiceWithDetailsModel, getAppParametersResponse.Value, isDuplicata, includeHeader);

        var pdfBytes = await _printService.GeneratePdfAsync(printInvoiceWithDetailsModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintFullInvoiceModel printInvoiceWithDetailsModel,
        GetAppParametersResponse getAppParametersResponse,
        bool isDuplicata = false,
        bool includeHeader = true)
    {
        var printOptions = SilkPdfOptions.Default;
        
        if (!includeHeader)
        {
            // No header, use default margin
            return printOptions;
        }

        var duplicataText = isDuplicata ? "<div style='font-weight: bold; font-size: 16px; color: red; margin-top: 10px;'>DUPLICATA</div>" : "";
        
        var headerContent = $@"
<div style='font-family: Arial; font-size: 12px; width: 90%; margin: 0 auto;'>
    <table style='width: 100%; border-collapse: collapse;'>
        <tr>
            <!-- Left Column - Company Info -->
            <td style='width: 35%; text-align: left; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>{getAppParametersResponse.NomSociete}</div>
                <div>{getAppParametersResponse.Adresse}</div>
                <div>Tel: {getAppParametersResponse.Tel}</div>
                <div>TVA: {$"{getAppParametersResponse.MatriculeFiscale}/{getAppParametersResponse.CodeTva}/{getAppParametersResponse.CodeCategorie}/{getAppParametersResponse.EtbSecondaire}"}</div>
                <div>E-mail: ste.nissaf@gmail.com</div>
            </td>
            
            <!-- Middle Column - Invoice Info -->
            <td style='width: 35%; text-align: center; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>FACTURE</div>
                {duplicataText}
                <div style='font-weight: bold;'>N°: {printInvoiceWithDetailsModel.Num}</div>
                <div>Date: {printInvoiceWithDetailsModel.Date.ToString("dd/MM/yyyy")}</div>
                <div>Page: <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
            
            <!-- Right Column - Client Info -->
            <td style='width: 35%; text-align: left; vertical-align: top; padding-left: 50px;'>
                <div style='font-weight: bold;'>Client :</div>
                <div style='font-weight: bold;'>{printInvoiceWithDetailsModel.Client.Nom}</div>
                <div>Adresse: {printInvoiceWithDetailsModel.Client.Adresse}</div>
                <div>Tél: {printInvoiceWithDetailsModel.Client.Tel}</div>
                <div>Code TVA: {printInvoiceWithDetailsModel.Client.Code}</div>
                <div>Matricule: {printInvoiceWithDetailsModel.Client.Matricule}</div>
            </td>
        </tr>
    </table>
</div>";

        printOptions.MarginTop = "150px";
        printOptions.HeaderTemplate = headerContent;
        return printOptions;
    }

    private static void CalculateTotalAmounts(PrintFullInvoiceModel printInvoiceWithDetailsModel, decimal timbre, GetAppParametersResponse appParameters)
    {
        printInvoiceWithDetailsModel.Base19 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == (int)appParameters.VatRate19).Sum(l => l.TotHt)));
        printInvoiceWithDetailsModel.Base13 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == (int)appParameters.VatRate13).Sum(l => l.TotHt)));
        printInvoiceWithDetailsModel.Base7 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == (int)appParameters.VatRate7).Sum(l => l.TotHt)));
        printInvoiceWithDetailsModel.Tva19 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == (int)appParameters.VatRate19).Sum(l => l.TotTtc - l.TotHt)));
        printInvoiceWithDetailsModel.Tva13 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == (int)appParameters.VatRate13).Sum(l => l.TotTtc - l.TotHt)));
        printInvoiceWithDetailsModel.Tva7 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == (int)appParameters.VatRate7).Sum(l => l.TotTtc - l.TotHt)));
        
        // Calculate TotalHT and TotalTVA from the rounded bases
        printInvoiceWithDetailsModel.TotalHT = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.Base19 + printInvoiceWithDetailsModel.Base13 + printInvoiceWithDetailsModel.Base7);
        printInvoiceWithDetailsModel.TotalTVA = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.Tva19 + printInvoiceWithDetailsModel.Tva13 + printInvoiceWithDetailsModel.Tva7);
        
        // Calculate TotalTTC including timbre
        printInvoiceWithDetailsModel.TotalTTC = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.TotalHT + printInvoiceWithDetailsModel.TotalTVA + timbre);
    }

    private async Task<Result<GetAppParametersResponse>> FetchAppParametersAsync(CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);

        if (appParametersResult.IsT1)
        {
            _ = Result.Fail("failed_to_retrieve_app_parameters");
        }

        var getAppParametersResponse = appParametersResult.AsT0;
        _logger.LogInformation("Successfully retrieved app parameters.");

        return Result.Ok(getAppParametersResponse);
    }
}