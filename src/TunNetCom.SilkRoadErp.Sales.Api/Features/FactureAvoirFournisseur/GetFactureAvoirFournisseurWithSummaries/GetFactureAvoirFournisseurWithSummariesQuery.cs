using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseurWithSummaries;

public record GetFactureAvoirFournisseurWithSummariesQuery(
    int PageNumber,
    int PageSize,
    int? IdFournisseur,
    int? NumFactureFournisseur,
    string? SortOrder,
    string? SortProperty,
    string? SearchKeyword,
    DateTime? StartDate,
    DateTime? EndDate
) : IRequest<GetFactureAvoirFournisseurWithSummariesResponse>;

