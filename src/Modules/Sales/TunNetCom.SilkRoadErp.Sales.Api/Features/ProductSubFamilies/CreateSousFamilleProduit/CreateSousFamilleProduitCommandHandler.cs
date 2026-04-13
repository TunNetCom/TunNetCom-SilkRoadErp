using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.CreateSousFamilleProduit;

public class CreateSousFamilleProduitCommandHandler(
    SalesContext _context,
    ILogger<CreateSousFamilleProduitCommandHandler> _logger)
    : IRequestHandler<CreateSousFamilleProduitCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSousFamilleProduitCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(SousFamilleProduit), command);

        var familleExists = await _context.FamilleProduit
            .AnyAsync(f => f.Id == command.FamilleProduitId, cancellationToken);

        if (!familleExists)
        {
            return Result.Fail(EntityNotFound.Error("Famille de produit non trouvée"));
        }

        var sousFamilleExists = await _context.SousFamilleProduit
            .AnyAsync(sf => sf.Nom == command.Nom && sf.FamilleProduitId == command.FamilleProduitId, cancellationToken);

        if (sousFamilleExists)
        {
            return Result.Fail("sous_famille_produit_name_exists");
        }

        var sousFamille = SousFamilleProduit.CreateSousFamilleProduit(command.Nom, command.FamilleProduitId);
        _context.SousFamilleProduit.Add(sousFamille);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogEntityCreatedSuccessfully(nameof(SousFamilleProduit), sousFamille.Id);
        return sousFamille.Id;
    }
}

