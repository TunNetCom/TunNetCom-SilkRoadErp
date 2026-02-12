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
            .Include(a => a.NumFactureFournisseurNavigation)
                .ThenInclude(f => f!.IdFournisseurNavigation)
            .Select(a => new FullAvoirFinancierFournisseursResponse
            {
                Num = a.Num,
                NumSurPage = a.NumSurPage,
                Date = a.Date,
                Description = a.Description,
                TotTtc = a.TotTtc,
                FactureFournisseur = a.NumFactureFournisseurNavigation == null ? null : new AvoirFinancierFournisseursFactureFournisseurResponse
                {
                    Num = a.NumFactureFournisseurNavigation.Num,
                    ProviderId = a.NumFactureFournisseurNavigation.IdFournisseur,
                    ProviderName = a.NumFactureFournisseurNavigation.IdFournisseurNavigation.Nom,
                    Date = a.NumFactureFournisseurNavigation.Date,
                    DateFacturation = a.NumFactureFournisseurNavigation.DateFacturationFournisseur,
                    ProviderInvoiceNumber = a.NumFactureFournisseurNavigation.NumFactureFournisseur,
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

