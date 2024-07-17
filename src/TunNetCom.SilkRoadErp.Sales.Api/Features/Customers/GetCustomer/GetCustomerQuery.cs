namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomer;

public record GetCustomerQuery(
    int PageNumber,
    int PageSize,
    string? SearchKeyword) : IRequest<PagedList<CustomerResponse>>;
