using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Banque.GetBanques;

public class GetBanquesQueryHandler(
    SalesContext _context,
    ILogger<GetBanquesQueryHandler> _logger)
    : IRequestHandler<GetBanquesQuery, Result<List<BanqueResponse>>>
{
    public async Task<Result<List<BanqueResponse>>> Handle(GetBanquesQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetBanquesQuery called");

        var banques = await _context.Banque
            .AsNoTracking()
            .OrderBy(b => b.Nom)
            .Select(b => new BanqueResponse
            {
                Id = b.Id,
                Nom = b.Nom
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(banques);
    }
}

