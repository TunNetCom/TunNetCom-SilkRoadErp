using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetFullAvoirFinancierFournisseurs;

public class GetFullAvoirFinancierFournisseursQueryHandler(
    SalesContext _context,
    ILogger<GetFullAvoirFinancierFournisseursQueryHandler> _logger)
    : IRequestHandler<GetFullAvoirFinancierFournisseursQuery, Result<FullAvoirFinancierFournisseursResponse>>
{
    public async Task<Result<FullAvoirFinancierFournisseursResponse>> Handle(GetFullAvoirFinancierFournisseursQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full AvoirFinancierFournisseurs with Num {Num}", query.Num);

        var avoirFinancier = await _context.AvoirFinancierFournisseurs
            .AsNoTracking()
            .Where(a => a.Num == query.Num)
            .Include(a => a.NumNavigation)
                .ThenInclude(f => f.IdFournisseurNavigation)
            .Select(a => new FullAvoirFinancierFournisseursResponse
            {
                Num = a.Num,
                NumSurPage = a.NumSurPage,
                Date = a.Date,
                Description = a.Description,
                TotTtc = a.TotTtc,
                FactureFournisseur = new AvoirFinancierFournisseursFactureFournisseurResponse
                {
                    Num = a.NumNavigation.Num,
                    ProviderId = a.NumNavigation.IdFournisseur,
                    ProviderName = a.NumNavigation.IdFournisseurNavigation.Nom,
                    Date = a.NumNavigation.Date,
                    DateFacturation = a.NumNavigation.DateFacturationFournisseur,
                    ProviderInvoiceNumber = a.NumNavigation.NumFactureFournisseur,
                    TotTTC = 0 // This would need to be calculated from lines if needed
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (avoirFinancier == null)
        {
            _logger.LogWarning("AvoirFinancierFournisseurs with Num {Num} not found", query.Num);
            return Result.Fail("avoir_financier_fournisseurs_not_found");
        }

        _logger.LogInformation("Full AvoirFinancierFournisseurs with Num {Num} fetched successfully", query.Num);
        return Result.Ok(avoirFinancier);
    }
}

