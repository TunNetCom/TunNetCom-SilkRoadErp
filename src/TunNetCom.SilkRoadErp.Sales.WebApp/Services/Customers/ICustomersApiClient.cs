using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Customers;

public interface ICustomersApiClient
{
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateCustomer(
        UpdateCustomerRequest request,
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<CustomerResponse, bool>> GetCustomer(
        int id,
        CancellationToken cancellationToken);

    Task<Stream> DeleteCustomer(string id, CancellationToken cancellationToken);

    Task<PagedList<CustomerResponse>> GetCustomers(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task<OneOf<CreateCustomerRequest, BadRequestResponse>> CreateCustomer(
        CreateCustomerRequest request,
        CancellationToken cancellationToken);
}