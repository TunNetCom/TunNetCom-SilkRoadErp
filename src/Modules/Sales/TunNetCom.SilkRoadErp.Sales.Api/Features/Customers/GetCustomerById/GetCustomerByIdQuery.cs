namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

public record GetCustomerByIdQuery(int Id) : IRequest<Result<CustomerResponse>>;