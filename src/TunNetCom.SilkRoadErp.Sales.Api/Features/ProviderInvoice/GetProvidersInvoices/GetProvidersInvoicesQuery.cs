namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProvidersInvoices;

public record GetProvidersInvoicesQuery
    (
        int IdFournisseur,
        int PageNumber,
        int PageSize,
        string? SearchKeyword,
        string? SortOrder,
        string? SortProperty) : IRequest<GetProviderInvoicesWithSummary>;