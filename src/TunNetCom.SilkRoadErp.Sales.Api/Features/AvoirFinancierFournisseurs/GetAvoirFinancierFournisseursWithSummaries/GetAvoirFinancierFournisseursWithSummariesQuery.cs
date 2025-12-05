namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetAvoirFinancierFournisseursWithSummaries;

public record GetAvoirFinancierFournisseursWithSummariesQuery(
    int? ProviderId,
    int? NumFactureFournisseur,
    string? SortOrder,
    string? SortProperty,
    int PageNumber,
    int PageSize,
    string? SearchKeyword,
    DateTime? StartDate,
    DateTime? EndDate
) : IRequest<Contracts.AvoirFinancierFournisseurs.GetAvoirFinancierFournisseursWithSummariesResponse>;

