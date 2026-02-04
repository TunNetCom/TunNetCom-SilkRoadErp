using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFactureDepense;

public class GetFactureDepenseQueryHandler(SalesContext _context, ILogger<GetFactureDepenseQueryHandler> _logger)
    : IRequestHandler<GetFactureDepenseQuery, Result<FactureDepenseResponse>>
{
    public async Task<Result<FactureDepenseResponse>> Handle(GetFactureDepenseQuery query, CancellationToken cancellationToken)
    {
        var entity = await _context.FactureDepense
            .AsNoTracking()
            .Include(f => f.IdTiersDepenseFonctionnementNavigation)
            .Where(f => f.Id == query.Id)
            .Select(f => new FactureDepenseResponse
            {
                Id = f.Id,
                Num = f.Num,
                IdTiersDepenseFonctionnement = f.IdTiersDepenseFonctionnement,
                TiersDepenseFonctionnementNom = f.IdTiersDepenseFonctionnementNavigation.Nom,
                Date = f.Date,
                Description = f.Description,
                MontantTotal = f.MontantTotal,
                AccountingYearId = f.AccountingYearId,
                Statut = f.Statut.ToString(),
                DocumentStoragePath = f.DocumentStoragePath,
                HasDocument = !string.IsNullOrEmpty(f.DocumentStoragePath)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            _logger.LogWarning("FactureDepense not found: Id {Id}", query.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        return Result.Ok(entity);
    }
}
