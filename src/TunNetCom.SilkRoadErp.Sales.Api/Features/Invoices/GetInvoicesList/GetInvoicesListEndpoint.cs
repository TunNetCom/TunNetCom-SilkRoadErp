using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesList;

public class GetInvoicesListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/list", HandleGetInvoicesListAsync)
            .WithTags(EndpointTags.Invoices)
            .Produces<InvoicesListResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<InvoicesListResponse>, StatusCodeHttpResult>> HandleGetInvoicesListAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<GetInvoicesListEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "GetInvoicesListEndpoint called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, page: {Page}, pageSize: {PageSize}",
                startDate, endDate, customerId, page, pageSize);

            // Get financial parameters
            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var timbre = await financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);
            var vatRate7 = (int)await financialParametersService.GetVatRate7Async(appParams.Value.VatRate7, cancellationToken);
            var vatRate13 = (int)await financialParametersService.GetVatRate13Async(appParams.Value.VatRate13, cancellationToken);
            var vatRate19 = (int)await financialParametersService.GetVatRate19Async(appParams.Value.VatRate19, cancellationToken);

            // Build base query
            var baseQuery = context.Facture
                .AsNoTracking()
                .FilterByActiveAccountingYear();

            // Log count before date filters
            var countBeforeDateFilters = await baseQuery.CountAsync(cancellationToken);
            logger.LogInformation("Count before date filters: {Count}", countBeforeDateFilters);

            // Apply filters
            if (startDate.HasValue)
            {
                logger.LogInformation("Applying startDate filter: {StartDate}", startDate.Value);
                baseQuery = baseQuery.Where(f => f.Date >= startDate.Value);
                var countAfterStart = await baseQuery.CountAsync(cancellationToken);
                logger.LogInformation("Count after startDate filter: {Count}", countAfterStart);
            }
            else
            {
                logger.LogWarning("StartDate is NULL");
            }

            if (endDate.HasValue)
            {
                // If endDate already has time component (not midnight), use it as-is
                // Otherwise, set to end of day
                var endDateInclusive = endDate.Value.TimeOfDay == TimeSpan.Zero 
                    ? endDate.Value.Date.AddDays(1).AddTicks(-1) 
                    : endDate.Value;
                logger.LogInformation("Applying endDate filter: {EndDate} (inclusive: {EndDateInclusive})", endDate.Value, endDateInclusive);
                baseQuery = baseQuery.Where(f => f.Date <= endDateInclusive);
                var countAfterEnd = await baseQuery.CountAsync(cancellationToken);
                logger.LogInformation("Count after endDate filter: {Count}", countAfterEnd);
            }
            else
            {
                logger.LogWarning("EndDate is NULL");
            }

            if (customerId.HasValue)
            {
                baseQuery = baseQuery.Where(f => f.IdClient == customerId.Value);
            }

            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(f => (int)f.Statut == status.Value);
            }

            if (tagIds != null && tagIds.Length > 0)
            {
                var tagIdsList = tagIds.ToList();
                var factureNumsWithTags = await context.DocumentTag
                    .Where(dt => dt.DocumentType == DocumentTypes.Facture && tagIdsList.Contains(dt.TagId))
                    .Select(dt => dt.DocumentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
                
                baseQuery = baseQuery.Where(f => factureNumsWithTags.Contains(f.Num));
            }

            // Get total count before pagination
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // Build query with joins
            var queryWithJoins = from f in baseQuery
                                join c in context.Client on f.IdClient equals c.Id
                                join bdl in context.BonDeLivraison on f.Num equals bdl.NumFacture into deliveryNotes
                                from bdl in deliveryNotes.DefaultIfEmpty()
                                select new { f, c, bdl };

            // Get all data for totals calculation
            var allInvoicesData = await queryWithJoins.ToListAsync(cancellationToken);

            // Group and calculate
            var allInvoices = allInvoicesData
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
                .ToList();

            // Log actual invoice dates
            if (allInvoices.Any())
            {
                var invoiceDates = allInvoices.Take(5).Select(i => new { i.Number, Date = i.Date.ToString("yyyy-MM-dd") }).ToList();
                logger.LogInformation("Sample invoice dates: {Dates}", string.Join(", ", invoiceDates.Select(d => $"#{d.Number}: {d.Date}")));
            }
            else
            {
                logger.LogWarning("No invoices found after filtering!");
            }

            // Apply sorting
            IEnumerable<InvoiceBaseInfo> sortedInvoices = allInvoices;
            if (!string.IsNullOrEmpty(sortBy))
            {
                sortedInvoices = sortBy.ToLower() switch
                {
                    "number" => sortDescending ? allInvoices.OrderByDescending(i => i.Number) : allInvoices.OrderBy(i => i.Number),
                    "date" => sortDescending ? allInvoices.OrderByDescending(i => i.Date) : allInvoices.OrderBy(i => i.Date),
                    "customername" => sortDescending ? allInvoices.OrderByDescending(i => i.CustomerName) : allInvoices.OrderBy(i => i.CustomerName),
                    "netamount" => sortDescending ? allInvoices.OrderByDescending(i => i.NetAmount) : allInvoices.OrderBy(i => i.NetAmount),
                    "vatamount" => sortDescending ? allInvoices.OrderByDescending(i => i.VatAmount) : allInvoices.OrderBy(i => i.VatAmount),
                    "statut" => sortDescending ? allInvoices.OrderByDescending(i => i.Statut) : allInvoices.OrderBy(i => i.Statut),
                    _ => allInvoices.OrderByDescending(i => i.Date)
                };
            }
            else
            {
                sortedInvoices = allInvoices.OrderByDescending(i => i.Date);
            }

            // Apply pagination
            var paginatedInvoices = sortedInvoices
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Calculate totals for ALL invoices (not just current page)
            var totalNetAmount = allInvoices.Sum(i => i.NetAmount);
            var totalVatAmount = allInvoices.Sum(i => i.VatAmount);
            var totalTtc = totalNetAmount + totalVatAmount;

            // Calculate VAT details
            var invoiceNumbers = allInvoices.Select(i => i.Number).ToList();
            var linesQuery = from bdl in context.BonDeLivraison
                            where bdl.NumFacture.HasValue && invoiceNumbers.Contains(bdl.NumFacture.Value)
                            join line in context.LigneBl on bdl.Id equals line.BonDeLivraisonId
                            select new { line.TotHt, line.TotTtc, Tva = (int)line.Tva };

            var lines = await linesQuery.ToListAsync(cancellationToken);

            var totalVat7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
            var totalVat13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
            var totalVat19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
            
            var totalBase7 = lines.Where(l => l.Tva == vatRate7).Sum(l => l.TotHt);
            var totalBase13 = lines.Where(l => l.Tva == vatRate13).Sum(l => l.TotHt);
            var totalBase19 = lines.Where(l => l.Tva == vatRate19).Sum(l => l.TotHt);

            var response = new InvoicesListResponse
            {
                Invoices = paginatedInvoices,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Totals = new InvoiceTotalsResponse
                {
                    TotalHT = totalNetAmount,
                    TotalVat = totalVatAmount,
                    TotalTTC = totalTtc,
                    TotalVat7 = totalVat7,
                    TotalVat13 = totalVat13,
                    TotalVat19 = totalVat19,
                    TotalBase7 = totalBase7,
                    TotalBase13 = totalBase13,
                    TotalBase19 = totalBase19
                }
            };

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting invoices list");
            return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

public class InvoicesListResponse
{
    public List<InvoiceBaseInfo> Invoices { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public InvoiceTotalsResponse Totals { get; set; } = new();
}

