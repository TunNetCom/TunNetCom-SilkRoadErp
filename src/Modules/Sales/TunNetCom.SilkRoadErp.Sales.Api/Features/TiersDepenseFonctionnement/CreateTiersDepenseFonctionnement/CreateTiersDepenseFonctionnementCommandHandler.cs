using Domain = TunNetCom.SilkRoadErp.Sales.Domain;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.CreateTiersDepenseFonctionnement;

public class CreateTiersDepenseFonctionnementCommandHandler(SalesContext _context, ILogger<CreateTiersDepenseFonctionnementCommandHandler> _logger)
    : IRequestHandler<CreateTiersDepenseFonctionnementCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTiersDepenseFonctionnementCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateTiersDepenseFonctionnementCommand called for Nom {Nom}", command.Nom);

        var entity = Domain.Entites.TiersDepenseFonctionnement.Create(
            command.Nom,
            command.Tel,
            command.Adresse,
            command.Matricule,
            command.Code,
            command.CodeCat,
            command.EtbSec,
            command.Mail,
            command.ExonereRetenueSource);

        _context.TiersDepenseFonctionnement.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TiersDepenseFonctionnement created with Id {Id}", entity.Id);
        return Result.Ok(entity.Id);
    }
}
