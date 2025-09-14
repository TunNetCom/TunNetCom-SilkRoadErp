namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.UpdateCustomer;

public class UpdateCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/customers/{id:int}", HandleUpdateCustomerAsync);
    }

    public static async Task<Results<NoContent, NotFound, ValidationProblem>> HandleUpdateCustomerAsync(
        IMediator mediator, int id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var updateCustomerCommand = new UpdateCustomerCommand(
            Id: id,
            Nom: request.Nom,
            Tel: request.Tel,
            Adresse: request.Adresse,
            Matricule: request.Matricule,
            Code: request.Code,
            CodeCat: request.CodeCat,
            EtbSec: request.EtbSec,
            Mail: request.Mail);

        var updateCustomerResult = await mediator.Send(updateCustomerCommand, cancellationToken);

        if (updateCustomerResult.IsEntityNotFound())
        {
            return TypedResults.NotFound();
        }

        if (updateCustomerResult.IsFailed)
        {
            return updateCustomerResult.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
