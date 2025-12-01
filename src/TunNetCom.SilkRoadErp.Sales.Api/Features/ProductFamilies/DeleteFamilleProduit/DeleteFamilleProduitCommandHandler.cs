using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.DeleteFamilleProduit;

public class DeleteFamilleProduitCommandHandler(
    SalesContext _context,
    ILogger<DeleteFamilleProduitCommandHandler> _logger)
    : IRequestHandler<DeleteFamilleProduitCommand, Result>
{
    public async Task<Result> Handle(DeleteFamilleProduitCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(nameof(FamilleProduit), command.Id);

        var famille = await _context.FamilleProduit
            .Include(f => f.SousFamilles)
            .FirstOrDefaultAsync(f => f.Id == command.Id, cancellationToken);

        if (famille == null)
        {
            _logger.LogEntityNotFound(nameof(FamilleProduit), command.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        if (famille.SousFamilles.Any())
        {
            return Result.Fail("famille_produit_has_subfamilies");
        }

        _context.FamilleProduit.Remove(famille);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(nameof(FamilleProduit), command.Id);
        return Result.Ok();
    }
}

