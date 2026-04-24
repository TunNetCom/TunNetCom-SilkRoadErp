using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.ExportToExcel;

public class ExportInvoicesToExcelEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/export/excel", HandleExportToExcelAsync)
            .WithTags(EndpointTags.Invoices)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToExcelAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] ExcelExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportInvoicesToExcelEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? selectedColumns = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportInvoicesToExcelEndpoint called with startDate: {StartDate} (HasValue: {HasStartDate}, Kind: {StartDateKind}), endDate: {EndDate} (HasValue: {HasEndDate}, Kind: {EndDateKind}), customerId: {CustomerId}, tagIds: {TagIds}, status: {Status}",
                startDate, startDate.HasValue, startDate.HasValue ? startDate.Value.Kind.ToString() : "N/A",
                endDate, endDate.HasValue, endDate.HasValue ? endDate.Value.Kind.ToString() : "N/A",
                customerId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            // Get financial parameters from service
            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var timbre = await financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            // Build query similar to InvoiceBaseInfosController
            var baseQuery = from f in context.Facture
                                .FilterByActiveAccountingYear()
                                .AsNoTracking()
                           join c in context.Client on f.IdClient equals c.Id
                           join bdl in context.BonDeLivraison on f.Num equals bdl.NumFacture into deliveryNotes
                           from bdl in deliveryNotes.DefaultIfEmpty()
                           select new { f, c, bdl };

            // Apply filters - EXACTLY like GetInvoiceTotalsEndpoint
            if (startDate.HasValue)
            {
                logger.LogInformation("Filtering by startDate: {StartDate}", startDate.Value);
                baseQuery = baseQuery.Where(x => x.f.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // If endDate already has time component (not midnight), use it as-is
                // Otherwise, set to end of day
                var endDateInclusive = endDate.Value.TimeOfDay == TimeSpan.Zero 
                    ? endDate.Value.Date.AddDays(1).AddTicks(-1) 
                    : endDate.Value;
                logger.LogInformation("Filtering by endDate: {EndDate} (inclusive: {EndDateInclusive})", endDate.Value, endDateInclusive);
                baseQuery = baseQuery.Where(x => x.f.Date <= endDateInclusive);
            }

            if (customerId.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.f.IdClient == customerId.Value);
            }

            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(x => (int)x.f.Statut == status.Value);
            }

            if (tagIds != null && tagIds.Length > 0)
            {
                var tagIdsList = tagIds.ToList();
                var factureNumsWithTags = await context.DocumentTag
                    .Where(dt => dt.DocumentType == DocumentTypes.Facture && tagIdsList.Contains(dt.TagId))
                    .Select(dt => dt.DocumentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
                
                baseQuery = baseQuery.Where(x => factureNumsWithTags.Contains(x.f.Num));
            }

            // Log query before execution to debug
            var sqlQuery = baseQuery.ToQueryString();
            logger.LogInformation("SQL Query before execution: {SqlQuery}", sqlQuery);
            
            var invoicesData = await baseQuery.ToListAsync(cancellationToken);
            logger.LogInformation("Found {Count} invoices after applying filters", invoicesData.Count);

            var invoices = invoicesData
                .GroupBy(x => new { x.f.Num, x.f.Date, x.f.IdClient, x.c.Nom, x.f.Statut })
                .Select(g => new InvoiceBaseInfo
                {
                    Number = g.Key.Num,
                    Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                    CustomerId = g.Key.IdClient,
                    CustomerName = g.Key.Nom,
                    // NetAmount is HT (excl. VAT). Using NetPayer (TTC) here caused VAT to be counted twice in UI/exports.
                    NetAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.TotHTva) + timbre,
                    VatAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.TotTva),
                    Statut = (int)g.Key.Statut,
                    StatutLibelle = g.Key.Statut.ToString()
                })
                .AsQueryable();

            // Apply ordering
            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderParts = orderBy.Split(' ');
                var propertyName = orderParts[0];
                var isDescending = orderParts.Length > 1 && orderParts[1].ToLower() == "desc";
                
                invoices = isDescending
                    ? invoices.OrderByDescending(GetOrderByExpression(propertyName))
                    : invoices.OrderBy(GetOrderByExpression(propertyName));
            }
            else
            {
                invoices = invoices.OrderBy(i => i.Date).ThenBy(i => i.Number);
            }

            var invoiceList = invoices.ToList();

            // Define all available columns
            var allColumns = new List<ColumnMapping>
            {
                new() { PropertyName = "Number", DisplayName = "Numéro" },
                new() { PropertyName = "Date", DisplayName = "Date" },
                new() { PropertyName = "CustomerName", DisplayName = "Client" },
                new() { PropertyName = "NetAmount", DisplayName = "Montant HT" },
                new() { PropertyName = "VatAmount", DisplayName = "Montant TVA" },
                new() { PropertyName = "StatutLibelle", DisplayName = "Statut" }
            };

            // Filter columns based on selection
            var columnsToExport = selectedColumns != null && selectedColumns.Length > 0
                ? allColumns.Where(c => selectedColumns.Contains(c.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList()
                : allColumns;

            if (!columnsToExport.Any())
            {
                columnsToExport = allColumns; // Default to all if none selected
            }

            logger.LogInformation("Exporting {Count} invoices to Excel with {ColumnCount} columns", invoiceList.Count, columnsToExport.Count);

            // Calculate totals with VAT details
            var vatRate7 = (int)await financialParametersService.GetVatRate7Async(appParams.Value.VatRate7, cancellationToken);
            var vatRate13 = (int)await financialParametersService.GetVatRate13Async(appParams.Value.VatRate13, cancellationToken);
            var vatRate19 = (int)await financialParametersService.GetVatRate19Async(appParams.Value.VatRate19, cancellationToken);

            var invoiceNumbers = invoiceList.Select(i => i.Number).ToList();
            var linesQuery = from bdl in context.BonDeLivraison
                            where bdl.NumFacture.HasValue && invoiceNumbers.Contains(bdl.NumFacture.Value)
                            join line in context.LigneBl on bdl.Id equals line.BonDeLivraisonId
                            select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };

            var lines = await linesQuery.ToListAsync(cancellationToken);
            var vatLines = lines.Select(l => new VatLineItem(l.TotHt, l.TotTtc, l.Tva));
            var totals = VatTotalsAggregator.Aggregate(vatLines, vatRate7, vatRate13, vatRate19, timbre, invoiceList.Count);
            
            logger.LogInformation("VAT totals calculated - Base7: {Base7}, Base19: {Base19}, Vat7: {Vat7}, Vat19: {Vat19}", 
                totals.TotalBase7, totals.TotalBase19, totals.TotalVat7, totals.TotalVat19);

            // Ensure values are always passed (even if 0) for display
            var fileBytes = exportService.ExportToExcel(
                invoiceList, 
                columnsToExport, 
                "Factures", 
                decimalPlaces, 
                totals.TotalHT, 
                totals.TotalVat, 
                totals.TotalTTC, 
                totals.TotalVat7, 
                totals.TotalVat13, 
                totals.TotalVat19, 
                totals.TotalBase7, 
                totals.TotalBase13, 
                totals.TotalBase19);

            var filename = $"Factures_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return TypedResults.File(
                fileBytes,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting invoices to Excel format");
            return TypedResults.StatusCode(500);
        }
    }

    private static System.Linq.Expressions.Expression<System.Func<InvoiceBaseInfo, object>> GetOrderByExpression(string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "number" => i => i.Number,
            "date" => i => i.Date,
            "customername" => i => i.CustomerName ?? string.Empty,
            "netamount" => i => i.NetAmount,
            "vatamount" => i => i.VatAmount,
            "statut" => i => i.Statut,
            _ => i => i.Number
        };
    }
}

