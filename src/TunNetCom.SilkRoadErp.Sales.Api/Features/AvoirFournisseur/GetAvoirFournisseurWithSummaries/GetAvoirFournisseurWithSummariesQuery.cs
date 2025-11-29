using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseurWithSummaries;

public record GetAvoirFournisseurWithSummariesQuery(
    int PageNumber,
    int PageSize,
    int? FournisseurId,
    int? NumFactureAvoirFournisseur,
    string? SortOrder,
    string? SortProperty,
    string? SearchKeyword,
    DateTime? StartDate,
    DateTime? EndDate,
    int? Status
) : IRequest<GetAvoirFournisseurWithSummariesResponse>;

