using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNotesList;

public class GetReceiptNotesListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/receipt-notes/list", HandleGetReceiptNotesListAsync)
            .WithTags(EndpointTags.ReceiptNotes)
            .Produces<ReceiptNotesListResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<ReceiptNotesListResponse>, StatusCodeHttpResult>> HandleGetReceiptNotesListAsync(
        [FromServices] SalesContext context,
        [FromServices] ILogger<GetReceiptNotesListEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
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
                "GetReceiptNotesListEndpoint called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, page: {Page}, pageSize: {PageSize}",
                startDate, endDate, providerId, page, pageSize);

            var baseQuery = context.BonDeReception
                .AsNoTracking()
                .FilterByActiveAccountingYear();

            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(br => br.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.TimeOfDay == TimeSpan.Zero
                    ? endDate.Value.Date.AddDays(1).AddTicks(-1)
                    : endDate.Value;
                baseQuery = baseQuery.Where(br => br.Date <= endDateInclusive);
            }

            if (providerId.HasValue)
            {
                baseQuery = baseQuery.Where(br => br.IdFournisseur == providerId.Value);
            }

            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(br => (int)br.Statut == status.Value);
            }

            if (tagIds != null && tagIds.Length > 0)
            {
                var tagIdsList = tagIds.ToList();
                var receiptNoteNumsWithTags = await context.DocumentTag
                    .Where(dt => dt.DocumentType == DocumentTypes.BonDeReception && tagIdsList.Contains(dt.TagId))
                    .Select(dt => dt.DocumentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                baseQuery = baseQuery.Where(br => receiptNoteNumsWithTags.Contains(br.Num));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var allReceiptNotesData = await (from br in baseQuery
                                             join f in context.Fournisseur on br.IdFournisseur equals f.Id into providerGroup
                                             from f in providerGroup.DefaultIfEmpty()
                                             select new { br, f })
                .ToListAsync(cancellationToken);

            var allReceiptNotes = allReceiptNotesData
                .Select(x => new ReceiptNoteBaseInfo
                {
                    Number = x.br.Num,
                    Date = new DateTimeOffset(x.br.Date, TimeSpan.Zero),
                    ProviderId = x.br.IdFournisseur,
                    ProviderName = x.f?.Nom,
                    NetAmount = x.br.NetPayer,
                    GrossAmount = x.br.TotHTva,
                    VatAmount = x.br.TotTva,
                    Statut = (int)x.br.Statut,
                    StatutLibelle = x.br.Statut.ToString(),
                    SupplierReceiptNumber = x.br.NumBonFournisseur
                })
                .ToList();

            IEnumerable<ReceiptNoteBaseInfo> sortedReceiptNotes = allReceiptNotes;
            if (!string.IsNullOrEmpty(sortBy))
            {
                sortedReceiptNotes = sortBy.ToLower() switch
                {
                    "number" => sortDescending
                        ? allReceiptNotes.OrderByDescending(r => r.Number)
                        : allReceiptNotes.OrderBy(r => r.Number),
                    "date" => sortDescending
                        ? allReceiptNotes.OrderByDescending(r => r.Date)
                        : allReceiptNotes.OrderBy(r => r.Date),
                    "providername" => sortDescending
                        ? allReceiptNotes.OrderByDescending(r => r.ProviderName)
                        : allReceiptNotes.OrderBy(r => r.ProviderName),
                    "grossamount" => sortDescending
                        ? allReceiptNotes.OrderByDescending(r => r.GrossAmount)
                        : allReceiptNotes.OrderBy(r => r.GrossAmount),
                    "vatamount" => sortDescending
                        ? allReceiptNotes.OrderByDescending(r => r.VatAmount)
                        : allReceiptNotes.OrderBy(r => r.VatAmount),
                    "netamount" => sortDescending
                        ? allReceiptNotes.OrderByDescending(r => r.NetAmount)
                        : allReceiptNotes.OrderBy(r => r.NetAmount),
                    "statut" => sortDescending
                        ? allReceiptNotes.OrderByDescending(r => r.Statut)
                        : allReceiptNotes.OrderBy(r => r.Statut),
                    _ => allReceiptNotes.OrderBy(r => r.Number)
                };
            }
            else
            {
                sortedReceiptNotes = allReceiptNotes.OrderBy(r => r.Number);
            }

            var paginatedReceiptNotes = sortedReceiptNotes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalGrossAmount = allReceiptNotes.Sum(r => r.GrossAmount);
            var totalVatAmount = allReceiptNotes.Sum(r => r.VatAmount);
            var totalNetAmount = allReceiptNotes.Sum(r => r.NetAmount);

            var response = new ReceiptNotesListResponse
            {
                ReceiptNotes = paginatedReceiptNotes,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Totals = new ReceiptNoteTotalsResponse
                {
                    TotalGrossAmount = totalGrossAmount,
                    TotalVatAmount = totalVatAmount,
                    TotalNetAmount = totalNetAmount
                }
            };

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting receipt notes list");
            return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
