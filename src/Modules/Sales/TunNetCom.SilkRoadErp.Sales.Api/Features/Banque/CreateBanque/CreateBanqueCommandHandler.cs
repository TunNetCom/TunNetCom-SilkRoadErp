using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Banque.CreateBanque;

public class CreateBanqueCommandHandler(
    SalesContext _context,
    ILogger<CreateBanqueCommandHandler> _logger)
    : IRequestHandler<CreateBanqueCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateBanqueCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateBanqueCommand called with Nom {Nom}", command.Nom);

        // Check if banque with same name already exists
        var banqueExists = await _context.Banque
            .AnyAsync(b => b.Nom == command.Nom, cancellationToken);
        if (banqueExists)
        {
            return Result.Fail("banque_name_already_exists");
        }

        var banque = Domain.Entites.Banque.CreateBanque(command.Nom);
        _context.Banque.Add(banque);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Banque created successfully with Id {Id}", banque.Id);
        return Result.Ok(banque.Id);
    }
}

