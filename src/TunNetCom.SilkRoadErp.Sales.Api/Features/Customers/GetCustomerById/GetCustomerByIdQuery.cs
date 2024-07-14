using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

public class GetCustomerByIdQuery : IRequest<Result<CustomerResponse>>
{
    public int Id { get; set; }

    public GetCustomerByIdQuery(int id)
    {
        Id = id;
    }
}
