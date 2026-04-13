using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetAvoir;

public class GetAvoirQueryHandler(
    SalesContext _context,
    ILogger<GetAvoirQueryHandler> _logger)
    : IRequestHandler<GetAvoirQuery, Result<AvoirResponse>>
{
    public async Task<Result<AvoirResponse>> Handle(GetAvoirQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Avoir with Num {Num}", query.Num);

        var avoir = await _context.Avoirs
            .AsNoTracking()
            .Where(a => a.Num == query.Num)
            .Select(a => new AvoirResponse
            {
                Num = a.Num,
                Date = a.Date,
                ClientId = a.ClientId,
                TotalExcludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotHt),
                TotalVATAmount = a.LigneAvoirs.Sum(l => l.TotTtc - l.TotHt),
                TotalIncludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotTtc)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (avoir == null)
        {
            _logger.LogWarning("Avoir with Num {Num} not found", query.Num);
            return Result.Fail("avoir_not_found");
        }

        _logger.LogInformation("Avoir with Num {Num} fetched successfully", query.Num);
        return Result.Ok(avoir);
    }
}

