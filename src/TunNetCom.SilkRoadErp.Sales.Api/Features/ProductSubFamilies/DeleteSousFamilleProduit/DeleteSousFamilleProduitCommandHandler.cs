using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.DeleteSousFamilleProduit;

public class DeleteSousFamilleProduitCommandHandler(
    SalesContext _context,
    ILogger<DeleteSousFamilleProduitCommandHandler> _logger)
    : IRequestHandler<DeleteSousFamilleProduitCommand, Result>
{
    public async Task<Result> Handle(DeleteSousFamilleProduitCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(nameof(SousFamilleProduit), command.Id);

        var sousFamille = await _context.SousFamilleProduit
            .Include(sf => sf.Produits)
            .FirstOrDefaultAsync(sf => sf.Id == command.Id, cancellationToken);

        if (sousFamille == null)
        {
            _logger.LogEntityNotFound(nameof(SousFamilleProduit), command.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        if (sousFamille.Produits.Any())
        {
            return Result.Fail("sous_famille_produit_has_products");
        }

        _context.SousFamilleProduit.Remove(sousFamille);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(nameof(SousFamilleProduit), command.Id);
        return Result.Ok();
    }
}

