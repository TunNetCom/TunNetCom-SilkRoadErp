namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

public class DeleteCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/customers/{id:int}", async Task<Results<NoContent, NotFound>> (
            IMediator mediator,
            int id,
            CancellationToken cancellationToken) =>
        {
            var deleteCustomerCommand = new DeleteCustomerCommand(id);
            var deleteResult = await mediator.Send(deleteCustomerCommand, cancellationToken);

            //TODO conditions based on the result : business validations and not found case.
            if (deleteResult.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });
    }
}
