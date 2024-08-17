namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;

public class UpdateCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/customers/{id:int}",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator, int id,
                UpdateCustomerRequest request,
                CancellationToken cancellationToken) =>
        {
            var updateClientCommand = new UpdateCustomerCommand(
                Id: id,
                Nom: request.Nom,
                Tel: request.Tel,
                Adresse: request.Adresse,
                Matricule: request.Matricule,
                Code: request.Code,
                CodeCat: request.CodeCat,
                EtbSec: request.EtbSec,
                Mail: request.Mail);

            var updateCustomerResult = await mediator.Send(updateClientCommand, cancellationToken);

            if (updateCustomerResult.HasError<EntityNotFound>())
            {
                return TypedResults.NotFound();
            }

            if (updateCustomerResult.IsFailed)
            {
                return updateCustomerResult.ToValidationProblem();
            }

            return TypedResults.NoContent();
        });
    }
}
