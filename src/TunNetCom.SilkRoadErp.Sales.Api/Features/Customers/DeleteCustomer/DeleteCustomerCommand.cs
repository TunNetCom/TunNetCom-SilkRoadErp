namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

public class DeleteCustomerCommand : IRequest<Result>
{
    public int Id { get; }

    public DeleteCustomerCommand(int id)
    {
        Id = id;
    }
}
