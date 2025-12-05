using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetAvoirFinancierFournisseurs;

public class GetAvoirFinancierFournisseursQueryHandler(
    SalesContext _context,
    ILogger<GetAvoirFinancierFournisseursQueryHandler> _logger)
    : IRequestHandler<GetAvoirFinancierFournisseursQuery, Result<AvoirFinancierFournisseursResponse>>
{
    public async Task<Result<AvoirFinancierFournisseursResponse>> Handle(GetAvoirFinancierFournisseursQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching AvoirFinancierFournisseurs with Num {Num}", query.Num);

        var avoirFinancier = await _context.AvoirFinancierFournisseurs
            .AsNoTracking()
            .Where(a => a.Num == query.Num)
            .Select(a => new AvoirFinancierFournisseursResponse
            {
                Num = a.Num,
                NumSurPage = a.NumSurPage,
                Date = a.Date,
                Description = a.Description,
                TotTtc = a.TotTtc,
                NumFactureFournisseur = a.Num
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (avoirFinancier == null)
        {
            _logger.LogWarning("AvoirFinancierFournisseurs with Num {Num} not found", query.Num);
            return Result.Fail("avoir_financier_fournisseurs_not_found");
        }

        _logger.LogInformation("AvoirFinancierFournisseurs with Num {Num} fetched successfully", query.Num);
        return Result.Ok(avoirFinancier);
    }
}

