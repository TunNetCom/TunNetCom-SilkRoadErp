using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.ExportToPdf;

public class ExportProviderInvoicesToPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/provider-invoices/export/pdf", HandleExportToPdfAsync)
            .WithTags(EndpointTags.ProviderInvoices)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToPdfAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] PdfListExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportProviderInvoicesToPdfEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? selectedColumns = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportProviderInvoicesToPdfEndpoint called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, tagIds: {TagIds}, status: {Status}",
                startDate, endDate, providerId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            // Get financial parameters from service
            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            // Build query similar to ProviderInvoiceBaseInfosController
            var baseQuery = context.FactureFournisseur.AsQueryable();

            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(ff => ff.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(ff => ff.Date <= endDateInclusive);
            }

            if (providerId.HasValue)
            {
                baseQuery = baseQuery.Where(ff => ff.IdFournisseur == providerId.Value);
            }

            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(ff => (int)ff.Statut == status.Value);
            }

            if (tagIds != null && tagIds.Length > 0)
            {
                var tagIdsList = tagIds.ToList();
                baseQuery = baseQuery.Where(ff => context.DocumentTag
                    .Any(dt => dt.DocumentType == "FactureFournisseur"
                        && dt.DocumentId == ff.Num
                        && tagIdsList.Contains(dt.TagId)));
            }

            var queryWithJoins = from ff in baseQuery
                                join f in context.Fournisseur on ff.IdFournisseur equals f.Id
                                join br in context.BonDeReception on ff.Num equals br.NumFactureFournisseur into receiptNotesGroup
                                from br in receiptNotesGroup.DefaultIfEmpty()
                                join lbr in context.LigneBonReception on br.Id equals lbr.BonDeReceptionId into linesGroup
                                from lbr in linesGroup.DefaultIfEmpty()
                                select new { ff, f, lbr };

            var invoicesData = await queryWithJoins.ToListAsync(cancellationToken);

            var invoices = invoicesData
                .GroupBy(x => new { x.ff.Num, x.ff.Date, x.ff.IdFournisseur, x.ff.Statut, x.f.Nom })
                .Select(g => new ProviderInvoiceBaseInfo
                {
                    Number = g.Key.Num,
                    Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                    ProviderId = g.Key.IdFournisseur,
                    ProviderName = g.Key.Nom,
                    NetAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt),
                    VatAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotTtc) - g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt),
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
                new() { PropertyName = "ProviderName", DisplayName = "Fournisseur" },
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

            logger.LogInformation("Exporting {Count} provider invoices to PDF with {ColumnCount} columns", invoiceList.Count, columnsToExport.Count);

            // Calculate totals with VAT details
            var vatRate7 = (int)await financialParametersService.GetVatRate7Async(appParams.Value.VatRate7, cancellationToken);
            var vatRate13 = (int)await financialParametersService.GetVatRate13Async(appParams.Value.VatRate13, cancellationToken);
            var vatRate19 = (int)await financialParametersService.GetVatRate19Async(appParams.Value.VatRate19, cancellationToken);

            var invoiceNumbers = invoiceList.Select(i => i.Number).ToList();
            var linesQuery = from br in context.BonDeReception
                            where br.NumFactureFournisseur.HasValue && invoiceNumbers.Contains(br.NumFactureFournisseur.Value)
                            join line in context.LigneBonReception on br.Id equals line.BonDeReceptionId
                            select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };

            var lines = await linesQuery.ToListAsync(cancellationToken);

            var totalNetAmount = lines.Sum(l => l.TotHt);
            var totalVat7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
            var totalVat13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
            var totalVat19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
            var totalVatAmount = totalVat7 + totalVat13 + totalVat19;
            var totalTtcAmount = totalNetAmount + totalVatAmount;

            var fileBytes = await exportService.ExportToPdfAsync(invoiceList, columnsToExport, "Liste des Factures Fournisseurs", decimalPlaces, totalNetAmount, totalVatAmount, totalTtcAmount, totalVat7, totalVat13, totalVat19, null, null, null, cancellationToken);

            var filename = $"Factures_Fournisseurs_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return TypedResults.File(
                fileBytes,
                contentType: "application/pdf",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting provider invoices to PDF format");
            return TypedResults.StatusCode(500);
        }
    }

    private static System.Linq.Expressions.Expression<System.Func<ProviderInvoiceBaseInfo, object>> GetOrderByExpression(string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "number" => i => i.Number,
            "date" => i => i.Date,
            "providername" => i => i.ProviderName ?? string.Empty,
            "netamount" => i => i.NetAmount,
            "vatamount" => i => i.VatAmount,
            "statut" => i => i.Statut,
            _ => i => i.Number
        };
    }
}

