using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Quotations;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Quotations.PrintQuotation;

public class PrintQuotationService(
    ILogger<PrintQuotationService> _logger,
    IQuotationApiClient _quotationApiClient,
    ICustomersApiClient _customersApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintQuotationModel, PrintQuotationView> _printService)
{
    public async Task<Result<byte[]>> GenerateQuotationPdfAsync(
        int quotationNumber,
        CancellationToken cancellationToken)
    {
        var quotationResult = await _quotationApiClient.GetQuotationByIdAsync(
            quotationNumber,
            cancellationToken);

        if (quotationResult.IsFailed || quotationResult.Value == null)
        {
            return Result.Fail("Quotation not found");
        }

        var quotation = quotationResult.Value;
        var printModel = MapToPrintModel(quotation);

        // Load customer if available
        if (printModel.CustomerId.HasValue)
        {
            var customer = await _customersApiClient.GetCustomerByIdAsync(
                printModel.CustomerId.Value,
                cancellationToken);
            if (customer != null)
            {
                printModel.Customer = new QuotationCustomerModel
                {
                    Id = customer.Id,
                    Nom = customer.Name,
                    Tel = customer.Tel,
                    Adresse = customer.Adresse,
                    Matricule = customer.Matricule,
                    Code = customer.Code
                };
            }
        }

        var getAppParametersResponse = await FetchAppParametersAsync(cancellationToken);
        if (getAppParametersResponse.IsFailed)
        {
            return Result.Fail("Failed to retrieve app parameters");
        }

        printModel.Timbre = getAppParametersResponse.Value.Timbre;
        CalculateTotalAmounts(printModel);

        var printOptions = PreparePrintOptions(printModel, getAppParametersResponse.Value);

        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    public async Task<Result<byte[]>> GenerateQuotationPdfFromDataAsync(
        PrintQuotationModel printModel,
        CancellationToken cancellationToken)
    {
        // Load customer if available
        if (printModel.CustomerId.HasValue && printModel.Customer == null)
        {
            var customer = await _customersApiClient.GetCustomerByIdAsync(
                printModel.CustomerId.Value,
                cancellationToken);
            if (customer != null)
            {
                printModel.Customer = new QuotationCustomerModel
                {
                    Id = customer.Id,
                    Nom = customer.Name,
                    Tel = customer.Tel,
                    Adresse = customer.Adresse,
                    Matricule = customer.Matricule,
                    Code = customer.Code
                };
            }
        }

        var getAppParametersResponse = await FetchAppParametersAsync(cancellationToken);
        if (getAppParametersResponse.IsFailed)
        {
            return Result.Fail("Failed to retrieve app parameters");
        }

        printModel.Timbre = getAppParametersResponse.Value.Timbre;
        CalculateTotalAmounts(printModel);

        var printOptions = PreparePrintOptions(printModel, getAppParametersResponse.Value);

        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static void CalculateTotalAmounts(PrintQuotationModel printModel)
    {
        printModel.Base19 = printModel.Lines.Where(l => l.Tva == 19).Sum(l => l.TotHt);
        printModel.Base13 = printModel.Lines.Where(l => l.Tva == 13).Sum(l => l.TotHt);
        printModel.Base7 = printModel.Lines.Where(l => l.Tva == 7).Sum(l => l.TotHt);
        printModel.Tva19 = printModel.Lines.Where(l => l.Tva == 19).Sum(l => l.TotTtc - l.TotHt);
        printModel.Tva13 = printModel.Lines.Where(l => l.Tva == 13).Sum(l => l.TotTtc - l.TotHt);
        printModel.Tva7 = printModel.Lines.Where(l => l.Tva == 7).Sum(l => l.TotTtc - l.TotHt);
        printModel.TotalTTC = printModel.TotalAmount + printModel.Timbre;
    }

    private static PrintQuotationModel MapToPrintModel(FullQuotationResponse quotation)
    {
        return new PrintQuotationModel
        {
            Num = quotation.Num,
            Date = quotation.Date,
            CustomerId = quotation.CustomerId,
            TotalExcludingTax = quotation.TotalExcludingTax,
            TotalVat = quotation.TotalVat,
            TotalAmount = quotation.TotalAmount,
            Lines = quotation.Items.Select(item => new QuotationLineModel
            {
                Id = item.Id,
                RefProduit = item.ProductReference,
                DesignationLi = item.Description,
                QteLi = item.Quantity,
                PrixHt = item.UnitPriceExcludingTax,
                Remise = item.DiscountPercentage,
                TotHt = item.TotalExcludingTax,
                Tva = item.VatPercentage,
                TotTtc = item.TotalIncludingTax
            }).ToList()
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintQuotationModel printModel,
        GetAppParametersResponse appParameters)
    {
        // Build customer info section conditionally
        var customerInfo = "";
        if (printModel.Customer != null)
        {
            customerInfo = $@"
                <div style='font-weight: bold;'>Client :</div>
                <div style='font-weight: bold;'>{printModel.Customer.Nom ?? ""}</div>";
            
            if (!string.IsNullOrEmpty(printModel.Customer.Adresse))
            {
                customerInfo += $@"<div>Adresse: {printModel.Customer.Adresse}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Customer.Tel))
            {
                customerInfo += $@"<div>Tél: {printModel.Customer.Tel}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Customer.Code))
            {
                customerInfo += $@"<div>Code TVA: {printModel.Customer.Code}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Customer.Matricule))
            {
                customerInfo += $@"<div>Matricule: {printModel.Customer.Matricule}</div>";
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
                <div>E-mail: ste.nissaf@gmail.com</div>
            </td>
            
            <!-- Middle Column - Quotation Info -->
            <td style='width: 35%; text-align: center; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>DEVIS</div>
                <div style='font-weight: bold;'>N°: {printModel.Num}</div>
                <div>Date: {printModel.Date.ToString("dd/MM/yyyy")}</div>
                <div>Page: <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
            
            <!-- Right Column - Client Info -->
            <td style='width: 35%; text-align: left; vertical-align: top; padding-left: 50px;'>
                {customerInfo}
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
            return Result.Fail("Failed to retrieve app parameters");
        }

        var getAppParametersResponse = appParametersResult.AsT0;
        _logger.LogInformation("Successfully retrieved app parameters.");

        return Result.Ok(getAppParametersResponse);
    }
}

