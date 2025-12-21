using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.GetProviderInvoiceTotals;

public class GetProviderInvoiceTotalsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/provider-invoices/totals", HandleGetTotalsAsync)
            .WithTags(EndpointTags.ProviderInvoices)
            .Produces<ProviderInvoiceTotalsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<ProviderInvoiceTotalsResponse>, StatusCodeHttpResult>> HandleGetTotalsAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<GetProviderInvoiceTotalsEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "GetProviderInvoiceTotalsEndpoint called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, tagIds: {TagIds}, status: {Status}",
                startDate, endDate, providerId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var vatRate7 = (int)appParams.Value.VatRate7;
            var vatRate13 = (int)appParams.Value.VatRate13;
            var vatRate19 = (int)appParams.Value.VatRate19;

            // Build base query for provider invoices
            var invoiceQuery = context.FactureFournisseur.AsQueryable();

            if (startDate.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(ff => ff.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                invoiceQuery = invoiceQuery.Where(ff => ff.Date <= endDateInclusive);
            }

            if (providerId.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(ff => ff.IdFournisseur == providerId.Value);
            }

            if (status.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(ff => (int)ff.Statut == status.Value);
            }

            if (tagIds != null && tagIds.Any())
            {
                invoiceQuery = invoiceQuery.Where(ff => context.DocumentTag
                    .Any(dt => dt.DocumentType == "FactureFournisseur"
                        && dt.DocumentId == ff.Num
                        && tagIds.Contains(dt.TagId)));
            }

            // Get all invoice numbers matching the criteria
            var invoiceNumbers = await invoiceQuery.Select(ff => ff.Num).ToListAsync(cancellationToken);

            // Calculate totals from receipt note lines
            var linesQuery = from br in context.BonDeReception
                            where br.NumFactureFournisseur.HasValue && invoiceNumbers.Contains(br.NumFactureFournisseur.Value)
                            join line in context.LigneBonReception on br.Id equals line.BonDeReceptionId
                            select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };

            var lines = await linesQuery.ToListAsync(cancellationToken);

            var totalHT = lines.Sum(l => l.TotHt);
            var totalVat7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
            var totalVat13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
            var totalVat19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
            var totalVat = totalVat7 + totalVat13 + totalVat19;
            var totalTTC = totalHT + totalVat;

            var response = new ProviderInvoiceTotalsResponse
            {
                TotalHT = totalHT,
                TotalVat7 = totalVat7,
                TotalVat13 = totalVat13,
                TotalVat19 = totalVat19,
                TotalVat = totalVat,
                TotalTTC = totalTTC
            };

            logger.LogInformation("Provider invoice totals calculated: HT={TotalHT}, TVA7={TotalVat7}, TVA13={TotalVat13}, TVA19={TotalVat19}, TTC={TotalTTC}",
                response.TotalHT, response.TotalVat7, response.TotalVat13, response.TotalVat19, response.TotalTTC);

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating provider invoice totals");
            return TypedResults.StatusCode(500);
        }
    }
}

