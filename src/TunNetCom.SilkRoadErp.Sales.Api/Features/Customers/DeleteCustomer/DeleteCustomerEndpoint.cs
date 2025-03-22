namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

public class DeleteCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/customers/{id:int}", async Task<Results<NoContent, ValidationProblem, NotFound>> (
            IMediator mediator,
            int id,
            CancellationToken cancellationToken) =>
        {
            var deleteCustomerCommand = new DeleteCustomerCommand(id);
            var deleteResult = await mediator.Send(deleteCustomerCommand, cancellationToken);

            if(deleteResult.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }

            if (deleteResult.IsFailed)
            {
                return deleteResult.ToValidationProblem();
            }

            return TypedResults.NoContent();
        });
    }
}
