using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementsFournisseur;

public record GetPaiementsFournisseurQuery(
    int? FournisseurId = null,
    int? AccountingYearId = null,
    DateTime? DateEcheanceFrom = null,
    DateTime? DateEcheanceTo = null,
    decimal? MontantMin = null,
    decimal? MontantMax = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<PaiementFournisseurResponse>>>;

