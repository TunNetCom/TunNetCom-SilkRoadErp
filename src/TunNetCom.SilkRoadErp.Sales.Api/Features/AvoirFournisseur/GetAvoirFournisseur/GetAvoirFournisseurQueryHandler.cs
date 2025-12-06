using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseur;

public class GetAvoirFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetAvoirFournisseurQueryHandler> _logger)
    : IRequestHandler<GetAvoirFournisseurQuery, Result<AvoirFournisseurResponse>>
{
    public async Task<Result<AvoirFournisseurResponse>> Handle(GetAvoirFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching AvoirFournisseur with Num {Num}", query.Num);

        var avoirFournisseur = await _context.AvoirFournisseur
            .AsNoTracking()
            .Where(a => a.Num == query.Num)
            .Select(a => new AvoirFournisseurResponse
            {
                Num = a.Num,
                Date = a.Date,
                FournisseurId = a.FournisseurId,
                NumFactureAvoirFournisseur = a.FactureAvoirFournisseurId,
                NumAvoirFournisseur = a.NumAvoirFournisseur,
                TotalExcludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotHt),
                TotalVATAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc - l.TotHt),
                TotalIncludingTaxAmount = a.LigneAvoirFournisseur.Sum(l => l.TotTtc)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (avoirFournisseur == null)
        {
            _logger.LogWarning("AvoirFournisseur with Num {Num} not found", query.Num);
            return Result.Fail("avoir_fournisseur_not_found");
        }

        _logger.LogInformation("AvoirFournisseur with Num {Num} fetched successfully", query.Num);
        return Result.Ok(avoirFournisseur);
    }
}

