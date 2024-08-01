using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Customers;

public interface ICustomersApiClient
{
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        UpdateCustomerRequest request,
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<CustomerResponse, bool>> GetAsync(
        int id,
        CancellationToken cancellationToken);

    Task<Stream> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<PagedList<CustomerResponse>> GetAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task<OneOf<CreateCustomerRequest, BadRequestResponse>> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken);

}