using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.ExportToSageErp;

public class ExportProviderInvoicesToSageErpEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/provider-invoices/export/sage", HandleExportToSageErpAsync)
            .WithTags(EndpointTags.ProviderInvoices)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToSageErpAsync(
        [FromServices] SalesContext context,
        [FromServices] SageErpExportService exportService,
        [FromServices] ILogger<ExportProviderInvoicesToSageErpEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
        [FromQuery] int[]? tagIds = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportProviderInvoicesToSageErpEndpoint called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, tagIds: {TagIds}",
                startDate, endDate, providerId, tagIds != null ? string.Join(",", tagIds) : "null");

            // Step 1: Get filtered facture fournisseur numbers first
            var factureQuery = context.FactureFournisseur.AsQueryable();

            if (startDate.HasValue)
            {
                factureQuery = factureQuery.Where(ff => ff.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                factureQuery = factureQuery.Where(ff => ff.Date <= endDateInclusive);
            }

            if (providerId.HasValue)
            {
                factureQuery = factureQuery.Where(ff => ff.IdFournisseur == providerId.Value);
            }

            // Apply tag filter if provided
            if (tagIds != null && tagIds.Length > 0)
            {
                var tagIdsList = tagIds.ToList();
                factureQuery = factureQuery.Where(ff => context.DocumentTag
                    .Any(dt => dt.DocumentType == "FactureFournisseur"
                        && dt.DocumentId == ff.Num
                        && tagIdsList.Contains(dt.TagId)));
            }

            var factureNums = await factureQuery
                .Select(ff => ff.Num)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!factureNums.Any())
            {
                logger.LogInformation("No provider invoices found matching the criteria");
                var emptyFileBytes = exportService.ExportProviderInvoicesToSageFormat(Enumerable.Empty<ProviderInvoiceBaseInfo>(), "ACH");
                return TypedResults.File(
                    emptyFileBytes,
                    contentType: "text/plain; charset=windows-1252",
                    fileDownloadName: $"Factures_Fournisseurs_Sage_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            }

            // Step 2: Get factures with providers
            var facturesWithProviders = await context.FactureFournisseur
                .Where(ff => factureNums.Contains(ff.Num))
                .Join(context.Fournisseur, ff => ff.IdFournisseur, f => f.Id, (ff, f) => new { Facture = ff, Provider = f })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Step 3: Get receipt notes for these factures
            var receiptNotes = await context.BonDeReception
                .Where(br => br.NumFactureFournisseur.HasValue && factureNums.Contains(br.NumFactureFournisseur.Value))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Step 4: Get receipt note lines
            var receiptNoteIds = receiptNotes.Select(br => br.Id).ToList();
            var receiptNoteLines = await context.LigneBonReception
                .Where(lbr => receiptNoteIds.Contains(lbr.BonDeReceptionId))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Step 5: Group and calculate in memory
            var invoices = facturesWithProviders
                .GroupBy(fp => new { fp.Facture.Num, fp.Facture.Date, fp.Facture.IdFournisseur, fp.Provider.Nom })
                .Select(g =>
                {
                    var factureNum = g.Key.Num;
                    var relatedReceiptNotes = receiptNotes
                        .Where(br => br.NumFactureFournisseur.HasValue && br.NumFactureFournisseur.Value == factureNum)
                        .ToList();
                    var relatedLines = receiptNoteLines
                        .Where(lbr => relatedReceiptNotes.Any(br => br.Id == lbr.BonDeReceptionId))
                        .ToList();

                    return new ProviderInvoiceBaseInfo
                    {
                        Number = g.Key.Num,
                        Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                        ProviderId = g.Key.IdFournisseur,
                        ProviderName = g.Key.Nom,
                        NetAmount = relatedLines.Sum(lbr => lbr.TotHt),
                        VatAmount = relatedLines.Sum(lbr => lbr.TotTtc) - relatedLines.Sum(lbr => lbr.TotHt)
                    };
                })
                .OrderBy(inv => inv.Date)
                .ThenBy(inv => inv.Number)
                .ToList();

            logger.LogInformation("Exporting {Count} provider invoices to Sage ERP format", invoices.Count);

            // Generate the Sage ERP file with journal code "ACH" for purchases
            var fileBytes = exportService.ExportProviderInvoicesToSageFormat(invoices, "ACH");

            // Generate filename with date range
            var filename = $"Factures_Fournisseurs_Sage_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            return TypedResults.File(
                fileBytes,
                contentType: "text/plain; charset=windows-1252",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting provider invoices to Sage ERP format");
            return TypedResults.StatusCode(500);
        }
    }
}

