using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementsClient;

public record GetPaiementsClientQuery(
    int? ClientId = null,
    IEnumerable<int>? AccountingYearIds = null,
    DateTime? DateEcheanceFrom = null,
    DateTime? DateEcheanceTo = null,
    decimal? MontantMin = null,
    decimal? MontantMax = null,
    bool? HasNumeroTransactionBancaire = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<PaiementClientResponse>>>;

