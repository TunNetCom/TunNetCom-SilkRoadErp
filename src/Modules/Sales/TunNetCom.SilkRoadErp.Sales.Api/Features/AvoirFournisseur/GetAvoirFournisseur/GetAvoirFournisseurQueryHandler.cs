using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseur;

public class GetAvoirFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetAvoirFournisseurQueryHandler> _logger)
    : IRequestHandler<GetAvoirFournisseurQuery, Result<AvoirFournisseurResponse>>
{
    public async Task<Result<AvoirFournisseurResponse>> Handle(GetAvoirFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching AvoirFournisseur with Id {Id}", query.Id);

        var avoirFournisseur = await _context.AvoirFournisseur
            .AsNoTracking()
            .Where(a => a.Id == query.Id)
            .Select(a => new AvoirFournisseurResponse
            {
                Id = a.Id,
                Date = a.Date,
                FournisseurId = a.FournisseurId,
                NumFactureAvoirFournisseur = a.FactureAvoirFournisseurId,
                NumAvoirChezFournisseur = a.NumAvoirChezFournisseur,
                TotalExcludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotHt),
                TotalVATAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt),
                TotalIncludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (avoirFournisseur == null)
        {
            _logger.LogWarning("AvoirFournisseur with Id {Id} not found", query.Id);
            return Result.Fail("avoir_fournisseur_not_found");
        }

        _logger.LogInformation("AvoirFournisseur with Id {Id} fetched successfully", query.Id);
        return Result.Ok(avoirFournisseur);
    }
}

