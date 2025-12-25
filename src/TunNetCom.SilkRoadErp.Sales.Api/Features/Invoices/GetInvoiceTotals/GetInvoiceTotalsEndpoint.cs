using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceTotals;

public class GetInvoiceTotalsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/totals", HandleGetTotalsAsync)
            .WithTags(EndpointTags.Invoices)
            .Produces<InvoiceTotalsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<InvoiceTotalsResponse>, StatusCodeHttpResult>> HandleGetTotalsAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<GetInvoiceTotalsEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "GetInvoiceTotalsEndpoint called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, tagIds: {TagIds}, status: {Status}",
                startDate, endDate, customerId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var timbre = appParams.Value.Timbre;
            var vatRate7 = (int)appParams.Value.VatRate7;
            var vatRate13 = (int)appParams.Value.VatRate13;
            var vatRate19 = (int)appParams.Value.VatRate19;

            // Build base query for invoices
            var invoiceQuery = context.Facture.AsQueryable();

            if (startDate.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(f => f.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                invoiceQuery = invoiceQuery.Where(f => f.Date <= endDateInclusive);
            }

            if (customerId.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(f => f.IdClient == customerId.Value);
            }

            if (status.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(f => (int)f.Statut == status.Value);
            }

            if (tagIds != null && tagIds.Any())
            {
                var factureNumsWithTags = await context.DocumentTag
                    .Where(dt => dt.DocumentType == DocumentTypes.Facture && tagIds.Contains(dt.TagId))
                    .Select(dt => dt.DocumentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
                
                invoiceQuery = invoiceQuery.Where(f => factureNumsWithTags.Contains(f.Num));
            }

            // Get all invoice numbers matching the criteria
            var invoiceNumbers = await invoiceQuery.Select(f => f.Num).ToListAsync(cancellationToken);

            // Calculate totals from delivery note lines
            var linesQuery = from bdl in context.BonDeLivraison
                            where bdl.NumFacture.HasValue && invoiceNumbers.Contains(bdl.NumFacture.Value)
                            join line in context.LigneBl on bdl.Id equals line.BonDeLivraisonId
                            select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };

            var lines = await linesQuery.ToListAsync(cancellationToken);

            var totalHT = lines.Sum(l => l.TotHt);
            var totalVat7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
            var totalVat13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
            var totalVat19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
            var totalVat = totalVat7 + totalVat13 + totalVat19;

            // Add timbre (stamp tax) to total HT (one per invoice)
            var invoiceCount = invoiceNumbers.Count;
            totalHT += timbre * invoiceCount;

            var totalTTC = totalHT + totalVat;

            var response = new InvoiceTotalsResponse
            {
                TotalHT = totalHT,
                TotalVat7 = totalVat7,
                TotalVat13 = totalVat13,
                TotalVat19 = totalVat19,
                TotalVat = totalVat,
                TotalTTC = totalTTC
            };

            logger.LogInformation("Invoice totals calculated: HT={TotalHT}, TVA7={TotalVat7}, TVA13={TotalVat13}, TVA19={TotalVat19}, TTC={TotalTTC}",
                response.TotalHT, response.TotalVat7, response.TotalVat13, response.TotalVat19, response.TotalTTC);

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating invoice totals");
            return TypedResults.StatusCode(500);
        }
    }
}

