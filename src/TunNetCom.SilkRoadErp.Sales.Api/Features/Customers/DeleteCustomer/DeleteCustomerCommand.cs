namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

public record DeleteCustomerCommand(int Id) : IRequest<Result>;