using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseur;

public class GetFactureAvoirFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetFactureAvoirFournisseurQueryHandler> _logger)
    : IRequestHandler<GetFactureAvoirFournisseurQuery, Result<FactureAvoirFournisseurResponse>>
{
    public async Task<Result<FactureAvoirFournisseurResponse>> Handle(GetFactureAvoirFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching FactureAvoirFournisseur with Num {Num}", query.Num);

        var factureAvoirFournisseur = await _context.FactureAvoirFournisseur
            .AsNoTracking()
            .Where(f => f.Num == query.Num)
            .Select(f => new FactureAvoirFournisseurResponse
            {
                Num = f.Num,
                NumFactureAvoirFourSurPage = f.NumFactureAvoirFourSurPage,
                Date = f.Date,
                IdFournisseur = f.IdFournisseur,
                NumFactureFournisseur = f.NumFactureFournisseur
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (factureAvoirFournisseur == null)
        {
            _logger.LogWarning("FactureAvoirFournisseur with Num {Num} not found", query.Num);
            return Result.Fail("facture_avoir_fournisseur_not_found");
        }

        _logger.LogInformation("FactureAvoirFournisseur with Num {Num} fetched successfully", query.Num);
        return Result.Ok(factureAvoirFournisseur);
    }
}

