using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.ExportToSageErp;

public class ExportToSageErpEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/export/sage", HandleExportToSageErpAsync)
            .WithTags(EndpointTags.Invoices)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToSageErpAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] SageErpExportService exportService,
        [FromServices] ILogger<ExportToSageErpEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int[]? tagIds = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportToSageErpEndpoint called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, tagIds: {TagIds}",
                startDate, endDate, customerId, tagIds != null ? string.Join(",", tagIds) : "null");

            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var timbre = appParams.Value.Timbre;

            // Step 1: Get filtered facture numbers first
            var factureQuery = context.Facture.AsQueryable();

            if (startDate.HasValue)
            {
                factureQuery = factureQuery.Where(f => f.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                factureQuery = factureQuery.Where(f => f.Date <= endDateInclusive);
            }

            if (customerId.HasValue)
            {
                factureQuery = factureQuery.Where(f => f.IdClient == customerId.Value);
            }

            // Apply tag filter if provided
            if (tagIds != null && tagIds.Length > 0)
            {
                var tagIdsList = tagIds.ToList();
                factureQuery = factureQuery.Where(f => context.DocumentTag
                    .Any(dt => dt.DocumentType == DocumentTypes.Facture
                        && dt.DocumentId == f.Num
                        && tagIdsList.Contains(dt.TagId)));
            }

            var factureNums = await factureQuery
                .Select(f => f.Num)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!factureNums.Any())
            {
                logger.LogInformation("No invoices found matching the criteria");
                var emptyFileBytes = exportService.ExportInvoicesToSageFormat(Enumerable.Empty<InvoiceBaseInfo>(), timbre);
                return TypedResults.File(
                    emptyFileBytes,
                    contentType: "text/plain; charset=windows-1252",
                    fileDownloadName: $"Factures_Sage_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            }

            // Step 2: Get factures with clients
            var facturesWithClients = await context.Facture
                .Where(f => factureNums.Contains(f.Num))
                .Join(context.Client, f => f.IdClient, c => c.Id, (f, c) => new { Facture = f, Client = c })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Step 3: Get delivery notes for these factures
            var deliveryNotes = await context.BonDeLivraison
                .Where(bdl => bdl.NumFacture.HasValue && factureNums.Contains(bdl.NumFacture.Value))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Step 4: Group and calculate in memory
            var invoices = facturesWithClients
                .GroupBy(fc => new { fc.Facture.Num, fc.Facture.Date, fc.Facture.IdClient, fc.Client.Nom })
                .Select(g =>
                {
                    var factureNum = g.Key.Num;
                    var relatedDeliveryNotes = deliveryNotes.Where(bdl => bdl.NumFacture.HasValue && bdl.NumFacture.Value == factureNum).ToList();

                    return new InvoiceBaseInfo
                    {
                        Number = g.Key.Num,
                        Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                        CustomerId = g.Key.IdClient,
                        CustomerName = g.Key.Nom,
                        NetAmount = relatedDeliveryNotes.Sum(bdl => bdl.NetPayer) + timbre,
                        VatAmount = relatedDeliveryNotes.Sum(bdl => bdl.TotTva)
                    };
                })
                .OrderBy(inv => inv.Date)
                .ThenBy(inv => inv.Number)
                .ToList();

            logger.LogInformation("Exporting {Count} invoices to Sage ERP format", invoices.Count);

            // Generate the Sage ERP file with journal code "VTE" for sales
            var fileBytes = exportService.ExportInvoicesToSageFormat(invoices, timbre, "VTE");

            // Generate filename with date range
            var filename = $"Factures_Sage_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            return TypedResults.File(
                fileBytes,
                contentType: "text/plain; charset=windows-1252",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting invoices to Sage ERP format");
            return TypedResults.StatusCode(500);
        }
    }
}

