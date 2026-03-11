using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseur;

public class GetFactureAvoirFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetFactureAvoirFournisseurQueryHandler> _logger)
    : IRequestHandler<GetFactureAvoirFournisseurQuery, Result<FactureAvoirFournisseurResponse>>
{
    public async Task<Result<FactureAvoirFournisseurResponse>> Handle(GetFactureAvoirFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching FactureAvoirFournisseur with Id {Id}", query.Id);

        var factureAvoirFournisseur = await (from f in _context.FactureAvoirFournisseur
                                             join ay in _context.AccountingYear on f.AccountingYearId equals ay.Id
                                             where f.Id == query.Id
                                             select new FactureAvoirFournisseurResponse
                                             {
                                                 Id = f.Id,
                                                 NumFactureAvoirFourSurPage = f.NumFactureAvoirFourSurPage,
                                                 Date = f.Date,
                                                 IdFournisseur = f.IdFournisseur,
                                                 NumFactureFournisseur = f.FactureFournisseurId,
                                                 AccountingYearName = ay.Year.ToString()
                                             })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (factureAvoirFournisseur == null)
        {
            _logger.LogWarning("FactureAvoirFournisseur with Id {Id} not found", query.Id);
            return Result.Fail("facture_avoir_fournisseur_not_found");
        }

        _logger.LogInformation("FactureAvoirFournisseur with Id {Id} fetched successfully", query.Id);
        return Result.Ok(factureAvoirFournisseur);
    }
}

