using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.GetRetenueSourceFactureDepense;

public class GetRetenueSourceFactureDepenseQueryHandler(
    SalesContext _context,
    ILogger<GetRetenueSourceFactureDepenseQueryHandler> _logger)
    : IRequestHandler<GetRetenueSourceFactureDepenseQuery, Result<RetenueSourceFactureDepenseResponse>>
{
    public async Task<Result<RetenueSourceFactureDepenseResponse>> Handle(GetRetenueSourceFactureDepenseQuery query, CancellationToken cancellationToken)
    {
        var retenue = await _context.RetenueSourceFactureDepense
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.FactureDepenseId == query.FactureDepenseId, cancellationToken);

        if (retenue == null)
        {
            return Result.Fail(EntityNotFound.Error());
        }

        var response = new RetenueSourceFactureDepenseResponse
        {
            Id = retenue.Id,
            FactureDepenseId = retenue.FactureDepenseId,
            NumTej = retenue.NumTej,
            MontantAvantRetenu = retenue.MontantAvantRetenu,
            TauxRetenu = retenue.TauxRetenu,
            MontantApresRetenu = retenue.MontantApresRetenu,
            DateCreation = retenue.DateCreation,
            HasPdf = !string.IsNullOrWhiteSpace(retenue.PdfStoragePath)
        };

        return Result.Ok(response);
    }
}
