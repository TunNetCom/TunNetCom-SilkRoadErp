using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.DeliveryNotes.PrintDeliveryNote;

public class PrintDeliveryNoteService(
    ILogger<PrintDeliveryNoteService> _logger,
    IDeliveryNoteApiClient _deliveryNoteApiClient,
    ICustomersApiClient _customersApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintDeliveryNoteModel, PrintDeliveryNoteView> _printService)
{
    public async Task<Result<byte[]>> GenerateDeliveryNotePdfAsync(
        int deliveryNoteNumber,
        CancellationToken cancellationToken,
        bool includeHeader = true)
    {
        var deliveryNoteResponse = await _deliveryNoteApiClient.GetDeliveryNoteByNumAsync(
            deliveryNoteNumber,
            cancellationToken);

        if (deliveryNoteResponse == null)
        {
            return Result.Fail("Delivery note not found");
        }

        var printModel = MapToPrintModel(deliveryNoteResponse);

        // Load customer if available
        if (printModel.CustomerId.HasValue)
        {
            var customer = await _customersApiClient.GetCustomerByIdAsync(
                printModel.CustomerId.Value,
                cancellationToken);
            if (customer != null)
            {
                printModel.Customer = new DeliveryNoteCustomerModel
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

        // Le timbre n'est PAS utilisé dans les BL, seulement dans les factures
        printModel.Timbre = 0;
        printModel.VatRate0 = getAppParametersResponse.Value.VatRate0;
        printModel.VatRate7 = getAppParametersResponse.Value.VatRate7;
        printModel.VatRate13 = getAppParametersResponse.Value.VatRate13;
        printModel.VatRate19 = getAppParametersResponse.Value.VatRate19;
        CalculateTotalAmounts(printModel, getAppParametersResponse.Value);

        var printOptions = PreparePrintOptions(printModel, getAppParametersResponse.Value, includeHeader);

        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    public async Task<Result<byte[]>> GenerateDeliveryNotePdfFromDataAsync(
        PrintDeliveryNoteModel printModel,
        CancellationToken cancellationToken,
        bool includeHeader = true)
    {
        // Load customer if available
        if (printModel.CustomerId.HasValue && printModel.Customer == null)
        {
            var customer = await _customersApiClient.GetCustomerByIdAsync(
                printModel.CustomerId.Value,
                cancellationToken);
            if (customer != null)
            {
                printModel.Customer = new DeliveryNoteCustomerModel
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

        // Le timbre n'est PAS utilisé dans les BL, seulement dans les factures
        printModel.Timbre = 0;
        printModel.VatRate0 = getAppParametersResponse.Value.VatRate0;
        printModel.VatRate7 = getAppParametersResponse.Value.VatRate7;
        printModel.VatRate13 = getAppParametersResponse.Value.VatRate13;
        printModel.VatRate19 = getAppParametersResponse.Value.VatRate19;
        CalculateTotalAmounts(printModel, getAppParametersResponse.Value);

        var printOptions = PreparePrintOptions(printModel, getAppParametersResponse.Value, includeHeader);

        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static void CalculateTotalAmounts(PrintDeliveryNoteModel printModel, GetAppParametersResponse appParameters)
    {
        printModel.Base19 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (int)appParameters.VatRate19).Sum(l => l.TotHt));
        printModel.Base13 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (int)appParameters.VatRate13).Sum(l => l.TotHt));
        printModel.Base7 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (int)appParameters.VatRate7).Sum(l => l.TotHt));
        printModel.Tva19 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (int)appParameters.VatRate19).Sum(l => l.TotTtc - l.TotHt));
        printModel.Tva13 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (int)appParameters.VatRate13).Sum(l => l.TotTtc - l.TotHt));
        printModel.Tva7 = DecimalHelper.RoundAmount(printModel.Lines.Where(l => l.Tva == (int)appParameters.VatRate7).Sum(l => l.TotTtc - l.TotHt));
        
        // Calculate TotalExcludingTax and TotalVat from the rounded bases
        printModel.TotalExcludingTax = DecimalHelper.RoundAmount(printModel.Base19 + printModel.Base13 + printModel.Base7);
        printModel.TotalVat = DecimalHelper.RoundAmount(printModel.Tva19 + printModel.Tva13 + printModel.Tva7);
        
        // Le timbre n'est PAS ajouté dans les BL, seulement dans les factures
        printModel.TotalTTC = DecimalHelper.RoundAmount(printModel.TotalExcludingTax + printModel.TotalVat);
    }

    private static PrintDeliveryNoteModel MapToPrintModel(DeliveryNoteResponse deliveryNote)
    {
        return new PrintDeliveryNoteModel
        {
            Num = deliveryNote.DeliveryNoteNumber,
            Date = deliveryNote.Date,
            DeliveryTime = deliveryNote.CreationTime,
            CustomerId = deliveryNote.CustomerId,
            InvoiceNumber = deliveryNote.InvoiceNumber,
            TotalExcludingTax = deliveryNote.TotalExcludingTax,
            TotalVat = deliveryNote.TotalVat,
            TotalAmount = deliveryNote.TotalAmount,
            Lines = deliveryNote.Items.Select(item => new DeliveryNoteLineModel
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
        PrintDeliveryNoteModel printModel,
        GetAppParametersResponse appParameters,
        bool includeHeader = true)
    {
        var printOptions = SilkPdfOptions.Default;
        
        if (!includeHeader)
        {
            // No header, use default margin
            return printOptions;
        }

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
            
            <!-- Middle Column - Delivery Note Info -->
            <td style='width: 35%; text-align: center; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>BON DE LIVRAISON</div>
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

