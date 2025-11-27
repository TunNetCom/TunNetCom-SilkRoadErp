namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;

public class CreateCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
            "/customers",
            HandleCreateCustomerAsync)
            .WithTags(EndpointTags.Customers)
            .RequireAuthorization();
    }

    public async Task<Results<Created<CreateCustomerRequest>, ValidationProblem>> HandleCreateCustomerAsync(
        IMediator mediator,
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var createCustomerCommand = new CreateCustomerCommand
        (
            Nom: request.Nom,
            Tel: request.Tel,
            Adresse: request.Adresse,
            Matricule: request.Matricule,
            Code: request.Code,
            CodeCat: request.CodeCat,
            EtbSec: request.EtbSec,
            Mail: request.Mail
        );

        Result<int> result = await mediator.Send(createCustomerCommand, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/customers/{result.Value}", request);
    }
}
