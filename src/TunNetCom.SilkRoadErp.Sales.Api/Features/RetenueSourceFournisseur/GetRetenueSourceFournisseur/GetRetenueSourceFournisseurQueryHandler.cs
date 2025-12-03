using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.GetRetenueSourceFournisseur;

public class GetRetenueSourceFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetRetenueSourceFournisseurQueryHandler> _logger)
    : IRequestHandler<GetRetenueSourceFournisseurQuery, Result<RetenueSourceFournisseurResponse>>
{
    public async Task<Result<RetenueSourceFournisseurResponse>> Handle(GetRetenueSourceFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetRetenueSourceFournisseurQuery called for FactureFournisseur {NumFactureFournisseur}", query.NumFactureFournisseur);

        var retenue = await _context.RetenueSourceFournisseur
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.NumFactureFournisseur == query.NumFactureFournisseur, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceFournisseur), query.NumFactureFournisseur);
            return Result.Fail(EntityNotFound.Error());
        }

        var response = new RetenueSourceFournisseurResponse
        {
            Id = retenue.Id,
            NumFactureFournisseur = retenue.NumFactureFournisseur,
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

