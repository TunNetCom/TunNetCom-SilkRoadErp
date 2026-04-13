using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.ProviderInvoices;

public class PrintProviderFullInvoiceService(
    ILogger<PrintProviderFullInvoiceService> _logger,
    IAppParametersClient _appParametersClient,
    IProviderInvoiceApiClient _providerInvoiceService,
    IPrintPdfService<ProviderInvoiceModel, PrintProviderFullInvoice> _printService)
{
    public async Task<Result<byte[]>> GenerateInvoicePdfAsync(
        int invoiceId,
        CancellationToken cancellationToken,
        bool includeHeader = true)
    {
        var providerInvoiceResponse = await _providerInvoiceService.GetFullProviderInvoiceByIdAsync(invoiceId, cancellationToken);

        if (providerInvoiceResponse.IsFailed)
        {
            return Result.Fail(providerInvoiceResponse.Errors);
        }
        ProviderInvoiceModel printInvoiceWithDetailsModel = MapToProviderInvoiceModel(providerInvoiceResponse.Value);

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
        CalculateTotalAmounts(printInvoiceWithDetailsModel, getAppParametersResponse.Value.Timbre, getAppParametersResponse.Value);

        SilkPdfOptions printOptions = PreparePrintOptions(printInvoiceWithDetailsModel, getAppParametersResponse.Value, includeHeader);

        var pdfBytes = await _printService.GeneratePdfAsync(printInvoiceWithDetailsModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    //  New method to map FullProviderInvoiceResponse to ProviderInvoiceModel
    private ProviderInvoiceModel MapToProviderInvoiceModel(FullProviderInvoiceResponse response)
    {
        return new ProviderInvoiceModel
        {
            InvoiceNumber = response.ProviderInvoiceNumber,
            IssueDate = response.Date,
            ProviderId = response.ProviderId,
            Provider = new ProviderDetailsModel
            {
                Id = response.Provider.Id,
                Name = response.Provider.Name,
                Phone = response.Provider.Phone,
                Address = response.Provider.Adress,
                TaxId = response.Provider.RegistrationNumber,
                ProviderCode = response.Provider.Code,
                CategoryCode = response.Provider.CategoryCode,
                SecondaryEstablishment = response.Provider.SecondaryEstablishment,
                Email = response.Provider.Mail
            },
            ReceiptNotes = response.ReceiptNotes.Select(rn => new ProviderReceiptNoteModel
            {
                ReceiptNoteId = rn.ReceiptNoteNumber,
                ReceiptDate = rn.Date,
                ProviderId = rn.ProviderId,
                ReceivedValueInclTax = rn.TotalExcludingVat,
                ReceivedValueExclTax = rn.TotalVat,
                ReceivedTaxAmount = rn.NetToPay,
                Lines = rn.Lines.Select(line => new ProviderReceiptLineModel
                {
                    LineId = line.LineId,
                    ProductCode = line.ProductReference,
                    Description = line.ItemDescription,
                    ReceivedQuantity = line.ItemQuantity,
                    Remise = line.Discount,
                    UnitPriceExclTax = line.UnitPriceExcludingTax,
                    LineTotalExclTax = line.TotalExcludingTax,
                    TaxRate = line.VatRate,
                    LineTotalInclTax = line.TotalIncludingTax
                }).ToList()
            }).ToList()
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        ProviderInvoiceModel printInvoiceWithDetailsModel,
        GetAppParametersResponse getAppParametersResponse,
        bool includeHeader = true)
    {
        var printOptions = SilkPdfOptions.Default;
        
        if (!includeHeader)
        {
            // No header, use default margin
            return printOptions;
        }

        var headerContent = $@"
<div style='font-family: Arial; font-size: 12px; width: 90%; margin: 0 auto;'>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='width: 35%; text-align: left; vertical-align: top;'>
                    <div style='font-weight: bold; font-size: 14px;'>{getAppParametersResponse.NomSociete}</div>
                    <div>{getAppParametersResponse.Adresse}</div>
                    <div>Tel: {getAppParametersResponse.Tel}</div>
                    <div>TVA: {$"{getAppParametersResponse.MatriculeFiscale}/{getAppParametersResponse.CodeTva}/{getAppParametersResponse.CodeCategorie}/{getAppParametersResponse.EtbSecondaire}"}</div>
                    <div>E-mail: {getAppParametersResponse.Email ?? ""}</div>
                </td>
                
                <td style='width: 35%; text-align: center; vertical-align: top;'>
                    <div style='font-weight: bold; font-size: 14px;'>FACTURE</div>
                    <div style='font-weight: bold;'>N°: {printInvoiceWithDetailsModel.InvoiceNumber}</div>
                    <div>Date: {printInvoiceWithDetailsModel.IssueDate.ToString("dd/MM/yyyy")}</div>
                    <div>Page: <span class='pageNumber'></span>/<span class='totalPages'></span></div>
                </td>
                
                <td style='width: 35%; text-align: left; vertical-align: top; padding-left: 50px;'>
                    <div style='font-weight: bold;'>Fournisseur :</div>
                    <div style='font-weight: bold;'>{printInvoiceWithDetailsModel.Provider.Name}</div>
                    <div>Adresse: {printInvoiceWithDetailsModel.Provider.Address}</div>
                    <div>Tél: {printInvoiceWithDetailsModel.Provider.Phone}</div>
                    <div>Code TVA: {printInvoiceWithDetailsModel.Provider.ProviderCode}</div>
                    <div>Matricule: {printInvoiceWithDetailsModel.Provider.TaxId}</div>
                </td>
            </tr>
        </table>
</div>";

        printOptions.MarginTop = "150px";
        printOptions.HeaderTemplate = headerContent;
        return printOptions;
    }

    private static void CalculateTotalAmounts(ProviderInvoiceModel printInvoiceWithDetailsModel, decimal timbre, GetAppParametersResponse appParameters)
    {
        printInvoiceWithDetailsModel.Base19 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == (int)appParameters.VatRate19).Sum(l => l.LineTotalExclTax)));
        printInvoiceWithDetailsModel.Base13 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == (int)appParameters.VatRate13).Sum(l => l.LineTotalExclTax)));
        printInvoiceWithDetailsModel.Base7 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == (int)appParameters.VatRate7).Sum(l => l.LineTotalExclTax)));
        printInvoiceWithDetailsModel.Tva19 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == (int)appParameters.VatRate19).Sum(l => l.LineTotalInclTax - l.LineTotalExclTax)));
        printInvoiceWithDetailsModel.Tva13 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == (int)appParameters.VatRate13).Sum(l => l.LineTotalInclTax - l.LineTotalExclTax)));
        printInvoiceWithDetailsModel.Tva7 = DecimalHelper.RoundAmount(printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == (int)appParameters.VatRate7).Sum(l => l.LineTotalInclTax - l.LineTotalExclTax)));
        
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