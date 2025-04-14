using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.ProviderInvoices;

public class PrintProviderFullInvoiceService(
    ILogger<PrintProviderFullInvoiceService> _logger,
    IAppParametersClient _appParametersClient,
    IProviderInvoiceApiClient _providerInvoiceService,
    IPrintPdfService<ProviderInvoiceModel, PrintProviderFullInvoice> _printService)
{
    public async Task<Result<byte[]>> GenerateInvoicePdfAsync(
        int invoiceId,
        CancellationToken cancellationToken)
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
        CalculateTotalAmounts(printInvoiceWithDetailsModel, getAppParametersResponse.Value.Timbre);

        SilkPdfOptions printOptions = PreparePrintOptions(printInvoiceWithDetailsModel, getAppParametersResponse.Value);

        var pdfBytes = await _printService.GeneratePdfAsync(printInvoiceWithDetailsModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    //  New method to map FullProviderInvoiceResponse to ProviderInvoiceModel
    private ProviderInvoiceModel MapToProviderInvoiceModel(FullProviderInvoiceResponse response)
    {
        return new ProviderInvoiceModel
        {
            InvoiceNumber = response.Num,
            IssueDate = response.Date,
            ProviderId = response.ProviderId,
            Provider = new ProviderDetailsModel
            {
                Id = response.Provider.Id,
                Name = response.Provider.Nom,
                Phone = response.Provider.Tel,
                Address = response.Provider.Adresse,
                TaxId = response.Provider.Matricule,
                ProviderCode = response.Provider.Code,
                CategoryCode = response.Provider.CodeCat,
                SecondaryEstablishment = response.Provider.EtbSec,
                Email = response.Provider.Mail
            },
            ReceiptNotes = response.ReceiptNotes.Select(rn => new ProviderReceiptNoteModel
            {
                ReceiptNoteId = rn.Num,
                ReceiptDate = rn.Date,
                ProviderId = rn.ProviderId,
                ReceivedValueInclTax = rn.TotHTva,
                ReceivedValueExclTax = rn.TotTva,
                ReceivedTaxAmount = rn.NetPayer,
                Lines = rn.Lines.Select(line => new ProviderReceiptLineModel
                {
                    LineId = line.IdLi,
                    ProductCode = line.RefProduit,
                    Description = line.DesignationLi,
                    ReceivedQuantity = line.QteLi,
                    Remise = line.Remise,
                    UnitPriceExclTax = line.PrixHt,
                    LineTotalExclTax = line.TotHt,
                    TaxRate = line.Tva,
                    LineTotalInclTax = line.TotTtc
                }).ToList()
            }).ToList()
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        ProviderInvoiceModel printInvoiceWithDetailsModel,
        GetAppParametersResponse getAppParametersResponse)
    {
        var headerContent = $@"
<div style='font-family: Arial; font-size: 12px; width: 90%; margin: 0 auto;'>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='width: 35%; text-align: left; vertical-align: top;'>
                    <div style='font-weight: bold; font-size: 14px;'>{getAppParametersResponse.NomSociete}</div>
                    <div>{getAppParametersResponse.Adresse}</div>
                    <div>Tel: {getAppParametersResponse.Tel}</div>
                    <div>TVA: {$"{getAppParametersResponse.MatriculeFiscale}/{getAppParametersResponse.CodeTva}/{getAppParametersResponse.CodeCategorie}/{getAppParametersResponse.EtbSecondaire}"}</div>
                    <div>E-mail: ste.nissaf@gmail.com</div>
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

        var printOptions = SilkPdfOptions.Default;
        printOptions.MarginTop = "150px";
        printOptions.HeaderTemplate = headerContent;
        return printOptions;
    }

    private static void CalculateTotalAmounts(ProviderInvoiceModel printInvoiceWithDetailsModel, decimal timbre)
    {
        printInvoiceWithDetailsModel.TotalHT = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Sum(l => l.LineTotalExclTax));
        printInvoiceWithDetailsModel.TotalTVA = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Sum(l => l.LineTotalInclTax - l.LineTotalExclTax));
        printInvoiceWithDetailsModel.TotalTTC = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Sum(l => l.LineTotalInclTax)) + timbre;
        printInvoiceWithDetailsModel.Base19 = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == 19).Sum(l => l.LineTotalExclTax));
        printInvoiceWithDetailsModel.Base13 = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == 13).Sum(l => l.LineTotalExclTax));
        printInvoiceWithDetailsModel.Base7 = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == 7).Sum(l => l.LineTotalExclTax));
        printInvoiceWithDetailsModel.Tva19 = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == 19).Sum(l => l.LineTotalInclTax - l.LineTotalExclTax));
        printInvoiceWithDetailsModel.Tva13 = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == 13).Sum(l => l.LineTotalInclTax - l.LineTotalExclTax));
        printInvoiceWithDetailsModel.Tva7 = printInvoiceWithDetailsModel.ReceiptNotes.Sum(dn => dn.Lines.Where(l => l.TaxRate == 7).Sum(l => l.LineTotalInclTax - l.LineTotalExclTax));
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