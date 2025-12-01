using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.UpdateSousFamilleProduit;

public class UpdateSousFamilleProduitCommandHandler(
    SalesContext _context,
    ILogger<UpdateSousFamilleProduitCommandHandler> _logger)
    : IRequestHandler<UpdateSousFamilleProduitCommand, Result>
{
    public async Task<Result> Handle(UpdateSousFamilleProduitCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(SousFamilleProduit), command.Id);

        var sousFamille = await _context.SousFamilleProduit
            .FirstOrDefaultAsync(sf => sf.Id == command.Id, cancellationToken);

        if (sousFamille == null)
        {
            _logger.LogEntityNotFound(nameof(SousFamilleProduit), command.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        var familleExists = await _context.FamilleProduit
            .AnyAsync(f => f.Id == command.FamilleProduitId, cancellationToken);

        if (!familleExists)
        {
            return Result.Fail(EntityNotFound.Error("Famille de produit non trouvée"));
        }

        var nomExists = await _context.SousFamilleProduit
            .AnyAsync(sf => sf.Nom == command.Nom && sf.FamilleProduitId == command.FamilleProduitId && sf.Id != command.Id, cancellationToken);

        if (nomExists)
        {
            return Result.Fail("sous_famille_produit_name_exists");
        }

        sousFamille.UpdateSousFamilleProduit(command.Nom, command.FamilleProduitId);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(SousFamilleProduit), command.Id);
        return Result.Ok();
    }
}

