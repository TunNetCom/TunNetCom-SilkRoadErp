using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.ValidateFactureDepense;

public class ValidateFactureDepenseCommandHandler(SalesContext _context, ILogger<ValidateFactureDepenseCommandHandler> _logger)
    : IRequestHandler<ValidateFactureDepenseCommand, Result>
{
    public async Task<Result> Handle(ValidateFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.FactureDepense.FindAsync(new object[] { command.Id }, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("FactureDepense not found: Id {Id}", command.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        try
        {
            entity.Valider();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "FactureDepense validation failed: Id {Id}", command.Id);
            return Result.Fail("facture_depense_validation_failed");
        }
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("FactureDepense validated: Id {Id}", command.Id);
        return Result.Ok();
    }
}
