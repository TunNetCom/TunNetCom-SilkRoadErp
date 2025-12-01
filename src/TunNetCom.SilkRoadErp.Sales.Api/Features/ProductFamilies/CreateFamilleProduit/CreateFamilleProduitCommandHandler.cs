using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.CreateFamilleProduit;

public class CreateFamilleProduitCommandHandler(
    SalesContext _context,
    ILogger<CreateFamilleProduitCommandHandler> _logger)
    : IRequestHandler<CreateFamilleProduitCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateFamilleProduitCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(FamilleProduit), command);

        var familleExists = await _context.FamilleProduit
            .AnyAsync(f => f.Nom == command.Nom, cancellationToken);

        if (familleExists)
        {
            return Result.Fail("famille_produit_name_exists");
        }

        var famille = FamilleProduit.CreateFamilleProduit(command.Nom);
        _context.FamilleProduit.Add(famille);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogEntityCreatedSuccessfully(nameof(FamilleProduit), famille.Id.ToString());
        return famille.Id;
    }
}

