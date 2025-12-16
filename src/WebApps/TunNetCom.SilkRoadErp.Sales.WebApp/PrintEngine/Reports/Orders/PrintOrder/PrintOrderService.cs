using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Orders;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Orders.PrintOrder;

public class PrintOrderService(
    ILogger<PrintOrderService> _logger,
    IOrderApiClient _orderApiClient,
    IProvidersApiClient _providersApiClient,
    IAppParametersClient _appParametersClient,
    IPrintPdfService<PrintOrderModel, PrintOrderView> _printService)
{
    public async Task<Result<byte[]>> GenerateOrderPdfAsync(
        int orderNumber,
        CancellationToken cancellationToken,
        bool includeHeader = true)
    {
        var orderResult = await _orderApiClient.GetFullOrderAsync(
            orderNumber,
            cancellationToken);

        if (orderResult == null)
        {
            return Result.Fail("Order not found");
        }

        var printModel = MapToPrintModel(orderResult);

        // Load supplier if available
        if (printModel.SupplierId.HasValue)
        {
            var supplierResult = await _providersApiClient.GetAsync(
                printModel.SupplierId.Value,
                cancellationToken);
            if (supplierResult.IsT0)
            {
                var supplier = supplierResult.AsT0;
                printModel.Supplier = new OrderSupplierModel
                {
                    Id = supplier.Id,
                    Nom = supplier.Nom,
                    Tel = supplier.Tel,
                    Adresse = supplier.Adresse,
                    Matricule = supplier.Matricule,
                    Code = supplier.Code
                };
            }
        }

        var getAppParametersResponse = await FetchAppParametersAsync(cancellationToken);
        if (getAppParametersResponse.IsFailed)
        {
            return Result.Fail("Failed to retrieve app parameters");
        }

        var printOptions = PreparePrintOptions(printModel, getAppParametersResponse.Value, includeHeader);

        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    public async Task<Result<byte[]>> GenerateOrderPdfFromDataAsync(
        PrintOrderModel printModel,
        CancellationToken cancellationToken,
        bool includeHeader = true)
    {
        // Load supplier if available
        if (printModel.SupplierId.HasValue && printModel.Supplier == null)
        {
            var supplierResult = await _providersApiClient.GetAsync(
                printModel.SupplierId.Value,
                cancellationToken);
            if (supplierResult.IsT0)
            {
                var supplier = supplierResult.AsT0;
                printModel.Supplier = new OrderSupplierModel
                {
                    Id = supplier.Id,
                    Nom = supplier.Nom,
                    Tel = supplier.Tel,
                    Adresse = supplier.Adresse,
                    Matricule = supplier.Matricule,
                    Code = supplier.Code
                };
            }
        }

        var getAppParametersResponse = await FetchAppParametersAsync(cancellationToken);
        if (getAppParametersResponse.IsFailed)
        {
            return Result.Fail("Failed to retrieve app parameters");
        }

        var printOptions = PreparePrintOptions(printModel, getAppParametersResponse.Value, includeHeader);

        var pdfBytes = await _printService.GeneratePdfAsync(printModel, printOptions, cancellationToken);

        return Result.Ok(pdfBytes);
    }

    private static PrintOrderModel MapToPrintModel(FullOrderResponse order)
    {
        return new PrintOrderModel
        {
            Num = order.OrderNumber,
            Date = order.Date,
            SupplierId = order.SupplierId,
            TotalExcludingTax = order.TotalExcludingVat,
            TotalVat = order.TotalVat,
            TotalAmount = order.NetToPay,
            Lines = order.OrderLines.Select(item => new OrderLineModel
            {
                Id = item.LineId,
                RefProduit = item.ProductReference ?? string.Empty,
                DesignationLi = item.ItemDescription ?? string.Empty,
                QteLi = (int)item.ItemQuantity,
                PrixHt = item.UnitPriceExcludingTax,
                Remise = (double)item.Discount,
                TotHt = item.TotalExcludingTax,
                Tva = (double)item.VatRate,
                TotTtc = item.TotalIncludingTax
            }).ToList()
        };
    }

    private static SilkPdfOptions PreparePrintOptions(
        PrintOrderModel printModel,
        GetAppParametersResponse appParameters,
        bool includeHeader = true)
    {
        var printOptions = SilkPdfOptions.Default;
        
        if (!includeHeader)
        {
            // No header, use default margin
            return printOptions;
        }

        // Build supplier info section conditionally
        var supplierInfo = "";
        if (printModel.Supplier != null)
        {
            supplierInfo = $@"
                <div style='font-weight: bold;'>Fournisseur :</div>
                <div style='font-weight: bold;'>{printModel.Supplier.Nom ?? ""}</div>";
            
            if (!string.IsNullOrEmpty(printModel.Supplier.Adresse))
            {
                supplierInfo += $@"<div>Adresse: {printModel.Supplier.Adresse}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Supplier.Tel))
            {
                supplierInfo += $@"<div>Tél: {printModel.Supplier.Tel}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Supplier.Code))
            {
                supplierInfo += $@"<div>Code TVA: {printModel.Supplier.Code}</div>";
            }
            
            if (!string.IsNullOrEmpty(printModel.Supplier.Matricule))
            {
                supplierInfo += $@"<div>Matricule: {printModel.Supplier.Matricule}</div>";
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
            
            <!-- Middle Column - Order Info -->
            <td style='width: 35%; text-align: center; vertical-align: top;'>
                <div style='font-weight: bold; font-size: 14px;'>COMMANDE FOURNISSEUR</div>
                <div style='font-weight: bold;'>N°: {printModel.Num}</div>
                <div>Date: {printModel.Date.ToString("dd/MM/yyyy")}</div>
                <div>Page: <span class='pageNumber'></span>/<span class='totalPages'></span></div>
            </td>
            
            <!-- Right Column - Supplier Info -->
            <td style='width: 35%; text-align: left; vertical-align: top; padding-left: 50px;'>
                {supplierInfo}
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

