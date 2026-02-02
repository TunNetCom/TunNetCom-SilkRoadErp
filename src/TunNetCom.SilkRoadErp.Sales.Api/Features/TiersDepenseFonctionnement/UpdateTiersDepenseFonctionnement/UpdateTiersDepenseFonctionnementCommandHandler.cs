using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.UpdateTiersDepenseFonctionnement;

public class UpdateTiersDepenseFonctionnementCommandHandler(SalesContext _context, ILogger<UpdateTiersDepenseFonctionnementCommandHandler> _logger)
    : IRequestHandler<UpdateTiersDepenseFonctionnementCommand, Result>
{
    public async Task<Result> Handle(UpdateTiersDepenseFonctionnementCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.TiersDepenseFonctionnement.FindAsync(new object[] { command.Id }, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("TiersDepenseFonctionnement not found: Id {Id}", command.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        entity.Update(
            command.Nom,
            command.Tel,
            command.Adresse,
            command.Matricule,
            command.Code,
            command.CodeCat,
            command.EtbSec,
            command.Mail);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("TiersDepenseFonctionnement updated: Id {Id}", command.Id);
        return Result.Ok();
    }
}
