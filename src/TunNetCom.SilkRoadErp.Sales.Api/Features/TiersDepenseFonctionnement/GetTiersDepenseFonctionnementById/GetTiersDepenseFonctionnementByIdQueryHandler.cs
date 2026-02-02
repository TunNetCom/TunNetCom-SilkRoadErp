using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.GetTiersDepenseFonctionnementById;

public class GetTiersDepenseFonctionnementByIdQueryHandler(SalesContext _context, ILogger<GetTiersDepenseFonctionnementByIdQueryHandler> _logger)
    : IRequestHandler<GetTiersDepenseFonctionnementByIdQuery, Result<TiersDepenseFonctionnementResponse>>
{
    public async Task<Result<TiersDepenseFonctionnementResponse>> Handle(GetTiersDepenseFonctionnementByIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await _context.TiersDepenseFonctionnement
            .AsNoTracking()
            .Where(t => t.Id == query.Id)
            .Select(t => new TiersDepenseFonctionnementResponse
            {
                Id = t.Id,
                Nom = t.Nom,
                Tel = t.Tel,
                Adresse = t.Adresse,
                Matricule = t.Matricule,
                Code = t.Code,
                CodeCat = t.CodeCat,
                EtbSec = t.EtbSec,
                Mail = t.Mail
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            _logger.LogWarning("TiersDepenseFonctionnement not found: Id {Id}", query.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        return Result.Ok(entity);
    }
}
