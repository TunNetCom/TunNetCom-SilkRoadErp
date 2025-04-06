using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
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
    CancellationToken cancellationToken)
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
        CalculateTotalAmounts(printInvoiceWithDetailsModel, getAppParametersResponse.Value.Timbre);

        SilkPdfOptions printOptions = PreparePrintOptions(printInvoiceWithDetailsModel, getAppParametersResponse.Value);

        var pdfBytes = await _printService.GeneratePdfAsync(printInvoiceWithDetailsModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintFullInvoiceModel printInvoiceWithDetailsModel,
        GetAppParametersResponse getAppParametersResponse)
    {
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

        var printOptions = SilkPdfOptions.Default;
        printOptions.MarginTop = "150px";
        printOptions.HeaderTemplate = headerContent;
        return printOptions;
    }

    private static void CalculateTotalAmounts(PrintFullInvoiceModel printInvoiceWithDetailsModel, decimal timbre)
    {
        printInvoiceWithDetailsModel.TotalHT = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Sum(l => l.TotHt));
        printInvoiceWithDetailsModel.TotalTVA = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Sum(l => l.TotTtc - l.TotHt));
        printInvoiceWithDetailsModel.TotalTTC = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Sum(l => l.TotTtc)) + timbre;
        printInvoiceWithDetailsModel.Base19 = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == 19).Sum(l => l.TotHt));
        printInvoiceWithDetailsModel.Base13 = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == 13).Sum(l => l.TotHt));
        printInvoiceWithDetailsModel.Base7 = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == 7).Sum(l => l.TotHt));
        printInvoiceWithDetailsModel.Tva19 = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == 19).Sum(l => l.TotTtc - l.TotHt));
        printInvoiceWithDetailsModel.Tva13 = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == 13).Sum(l => l.TotTtc - l.TotHt));
        printInvoiceWithDetailsModel.Tva7 = printInvoiceWithDetailsModel.DeliveryNotes.Sum(dn => dn.Lines.Where(l => l.Tva == 7).Sum(l => l.TotTtc - l.TotHt));
    }

    private async Task<Result<GetAppParametersResponse>> FetchAppParametersAsync(CancellationToken cancellationToken)
    {
        var appParametersResult = await _appParametersClient.GetAppParametersAsync(cancellationToken);

        if (appParametersResult.IsT1)
        {
            Result.Fail("failed_to_retrieve_app_parameters");
        }

        var getAppParametersResponse = appParametersResult.AsT0;
        _logger.LogInformation("Successfully retrieved app parameters.");

        return Result.Ok(getAppParametersResponse);
    }
}