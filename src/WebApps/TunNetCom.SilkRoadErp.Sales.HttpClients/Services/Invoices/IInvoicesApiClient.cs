namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;

public interface IInvoicesApiClient
{
    Task<OneOf<GetInvoiceListWithSummary, BadRequestResponse>> GetInvoicesByCustomerIdWithSummary(
        int customerId,
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);
    Task<PagedList<InvoiceResponse>> GetInvoices(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);
    Task<OneOf<int, BadRequestResponse>> CreateInvoice(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<IList<InvoiceResponse>, BadRequestResponse>> GetInvoicesByIdsAsync(
      List<int> invoiceIds,
      CancellationToken cancellationToken);

    Task<Result<FullInvoiceResponse>> GetFullInvoiceByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<GetInvoicesWithSummariesResponse> GetInvoicesWithSummariesAsync(
        int? customerId,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken);

    Task<Result> ValidateInvoicesAsync(
        List<int> ids,
        CancellationToken cancellationToken);

    Task<Result<int>> GetInvoiceIdByNumberAsync(
        int invoiceNumber,
        CancellationToken cancellationToken);
}
