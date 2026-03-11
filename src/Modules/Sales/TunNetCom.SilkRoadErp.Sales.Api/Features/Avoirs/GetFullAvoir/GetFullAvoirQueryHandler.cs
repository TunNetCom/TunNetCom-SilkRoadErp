using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetFullAvoir;

public class GetFullAvoirQueryHandler(
    SalesContext _context,
    ILogger<GetFullAvoirQueryHandler> _logger)
    : IRequestHandler<GetFullAvoirQuery, Result<FullAvoirResponse>>
{
    public async Task<Result<FullAvoirResponse>> Handle(GetFullAvoirQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full Avoir with Num {Num}", query.Num);

        var avoir = await _context.Avoirs
            .AsNoTracking()
            .Where(a => a.Num == query.Num)
            .Include(a => a.Client)
            .Include(a => a.LigneAvoirs)
            .Select(a => new FullAvoirResponse
            {
                Num = a.Num,
                Date = a.Date,
                ClientId = a.ClientId,
                Client = a.Client != null ? new AvoirClientResponse
                {
                    Id = a.Client.Id,
                    Nom = a.Client.Nom,
                    Tel = a.Client.Tel,
                    Adresse = a.Client.Adresse,
                    Matricule = a.Client.Matricule,
                    Code = a.Client.Code,
                    CodeCat = a.Client.CodeCat,
                    EtbSec = a.Client.EtbSec,
                    Mail = a.Client.Mail
                } : null,
                Lines = a.LigneAvoirs.Select(l => new AvoirLineResponse
                {
                    IdLi = l.IdLi,
                    RefProduit = l.RefProduit,
                    DesignationLi = l.DesignationLi,
                    QteLi = l.QteLi,
                    PrixHt = l.PrixHt,
                    Remise = l.Remise,
                    TotHt = l.TotHt,
                    Tva = l.Tva,
                    TotTtc = l.TotTtc
                }).ToList(),
                TotalExcludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotHt),
                TotalVATAmount = a.LigneAvoirs.Sum(l => l.TotTtc - l.TotHt),
                TotalIncludingTaxAmount = a.LigneAvoirs.Sum(l => l.TotTtc)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (avoir == null)
        {
            _logger.LogWarning("Avoir with Num {Num} not found", query.Num);
            return Result.Fail("avoir_not_found");
        }

        _logger.LogInformation("Full Avoir with Num {Num} fetched successfully", query.Num);
        return Result.Ok(avoir);
    }
}

