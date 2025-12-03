using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.GetRetenueSourceClient;

public class GetRetenueSourceClientQueryHandler(
    SalesContext _context,
    ILogger<GetRetenueSourceClientQueryHandler> _logger)
    : IRequestHandler<GetRetenueSourceClientQuery, Result<RetenueSourceClientResponse>>
{
    public async Task<Result<RetenueSourceClientResponse>> Handle(GetRetenueSourceClientQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetRetenueSourceClientQuery called for Facture {NumFacture}", query.NumFacture);

        var retenue = await _context.RetenueSourceClient
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.NumFacture == query.NumFacture, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceClient), query.NumFacture);
            return Result.Fail(EntityNotFound.Error());
        }

        var response = new RetenueSourceClientResponse
        {
            Id = retenue.Id,
            NumFacture = retenue.NumFacture,
            NumTej = retenue.NumTej,
            MontantAvantRetenu = retenue.MontantAvantRetenu,
            TauxRetenu = retenue.TauxRetenu,
            MontantApresRetenu = retenue.MontantApresRetenu,
            DateCreation = retenue.DateCreation,
            AccountingYearId = retenue.AccountingYearId,
            HasPdf = !string.IsNullOrWhiteSpace(retenue.PdfStoragePath)
        };

        return Result.Ok(response);
    }
}

