using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;

public class CreateCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/customers",
            async Task<Results<Created<CreateCustomerRequest>, ValidationProblem>> (
                IMediator mediator,
                CreateCustomerRequest request,
                CancellationToken cancellationToken) =>
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
                    Mail: request.Mail);

                var result = await mediator.Send(createCustomerCommand, cancellationToken);

                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }

                return TypedResults.Created($"/customers/{result.Value}", request);
            });
    }
}
