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
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            _logger.LogWarning("FactureDepense not found: Id {Id}", query.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        var response = new FactureDepenseResponse
        {
            Id = entity.Id,
            Num = entity.Num,
            IdTiersDepenseFonctionnement = entity.IdTiersDepenseFonctionnement,
            TiersDepenseFonctionnementNom = entity.IdTiersDepenseFonctionnementNavigation.Nom,
            Date = entity.Date,
            Description = entity.Description,
            MontantTotal = entity.MontantTotal,
            LignesTVA = new List<FactureDepenseLigneTvaDto>
            {
                new() { TauxTVA = 0, BaseHT = entity.BaseHT0, MontantTVA = entity.MontantTVA0 },
                new() { TauxTVA = 7, BaseHT = entity.BaseHT7, MontantTVA = entity.MontantTVA7 },
                new() { TauxTVA = 13, BaseHT = entity.BaseHT13, MontantTVA = entity.MontantTVA13 },
                new() { TauxTVA = 19, BaseHT = entity.BaseHT19, MontantTVA = entity.MontantTVA19 }
            },
            AccountingYearId = entity.AccountingYearId,
            Statut = entity.Statut.ToString(),
            DocumentStoragePath = entity.DocumentStoragePath,
            HasDocument = !string.IsNullOrEmpty(entity.DocumentStoragePath)
        };
        return Result.Ok(response);
    }
}
