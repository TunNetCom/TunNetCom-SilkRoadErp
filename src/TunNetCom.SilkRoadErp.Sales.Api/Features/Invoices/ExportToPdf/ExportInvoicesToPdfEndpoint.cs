using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.ExportToPdf;

public class ExportInvoicesToPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/export/pdf", HandleExportToPdfAsync)
            .WithTags(EndpointTags.Invoices)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToPdfAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] PdfListExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportInvoicesToPdfEndpoint> logger,
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
                "ExportInvoicesToPdfEndpoint called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, tagIds: {TagIds}, status: {Status}",
                startDate, endDate, customerId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            // Get financial parameters from service
            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var timbre = await financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            // Build query similar to InvoiceBaseInfosController
            var baseQuery = from f in context.Facture
                           join c in context.Client on f.IdClient equals c.Id
                           join bdl in context.BonDeLivraison on f.Num equals bdl.NumFacture into deliveryNotes
                           from bdl in deliveryNotes.DefaultIfEmpty()
                           select new { f, c, bdl };

            // Apply filters
            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.f.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
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

            var invoicesData = await baseQuery.ToListAsync(cancellationToken);

            var invoices = invoicesData
                .GroupBy(x => new { x.f.Num, x.f.Date, x.f.IdClient, x.c.Nom, x.f.Statut })
                .Select(g => new InvoiceBaseInfo
                {
                    Number = g.Key.Num,
                    Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                    CustomerId = g.Key.IdClient,
                    CustomerName = g.Key.Nom,
                    NetAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.NetPayer) + timbre,
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

            logger.LogInformation("Exporting {Count} invoices to PDF with {ColumnCount} columns", invoiceList.Count, columnsToExport.Count);

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

            var totalNetAmount = lines.Sum(l => l.TotHt) + (timbre * invoiceList.Count);
            var totalVat7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
            var totalVat13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
            var totalVat19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
            var totalVatAmount = totalVat7 + totalVat13 + totalVat19;
            var totalTtcAmount = totalNetAmount + totalVatAmount;

            var fileBytes = await exportService.ExportToPdfAsync(invoiceList, columnsToExport, "Liste des Factures", decimalPlaces, totalNetAmount, totalVatAmount, totalTtcAmount, totalVat7, totalVat13, totalVat19, cancellationToken);

            var filename = $"Factures_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return TypedResults.File(
                fileBytes,
                contentType: "application/pdf",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting invoices to PDF format");
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

