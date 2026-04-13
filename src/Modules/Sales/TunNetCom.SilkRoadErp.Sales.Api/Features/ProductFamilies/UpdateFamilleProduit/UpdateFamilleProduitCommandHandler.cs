using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.UpdateFamilleProduit;

public class UpdateFamilleProduitCommandHandler(
    SalesContext _context,
    ILogger<UpdateFamilleProduitCommandHandler> _logger)
    : IRequestHandler<UpdateFamilleProduitCommand, Result>
{
    public async Task<Result> Handle(UpdateFamilleProduitCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(FamilleProduit), command.Id);

        var famille = await _context.FamilleProduit
            .FirstOrDefaultAsync(f => f.Id == command.Id, cancellationToken);

        if (famille == null)
        {
            _logger.LogEntityNotFound(nameof(FamilleProduit), command.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        var nomExists = await _context.FamilleProduit
            .AnyAsync(f => f.Nom == command.Nom && f.Id != command.Id, cancellationToken);

        if (nomExists)
        {
            return Result.Fail("famille_produit_name_exists");
        }

        famille.UpdateFamilleProduit(command.Nom);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(FamilleProduit), command.Id);
        return Result.Ok();
    }
}

