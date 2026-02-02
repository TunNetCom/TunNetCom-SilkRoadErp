using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.UpdateFactureDepense;

public class UpdateFactureDepenseCommandHandler(SalesContext _context, ILogger<UpdateFactureDepenseCommandHandler> _logger)
    : IRequestHandler<UpdateFactureDepenseCommand, Result>
{
    public async Task<Result> Handle(UpdateFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.FactureDepense.FindAsync(new object[] { command.Id }, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("FactureDepense not found: Id {Id}", command.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        if (entity.Statut != DocumentStatus.Draft)
        {
            _logger.LogWarning("FactureDepense cannot be updated when not Draft: Id {Id}", command.Id);
            return Result.Fail("facture_depense_not_draft");
        }
        entity.Update(command.Date, command.Description ?? string.Empty, command.MontantTotal);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("FactureDepense updated: Id {Id}", command.Id);
        return Result.Ok();
    }
}
