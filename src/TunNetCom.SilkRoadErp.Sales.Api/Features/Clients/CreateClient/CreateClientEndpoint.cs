using Microsoft.AspNetCore.Http.HttpResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/clients",
            async Task<Results<Created<CreateClientRequest>, BadRequest<List<IError>>>> 
            (IMediator mediator,
            CreateClientRequest request) =>
        {
            var createClientCommand = new CreateClientCommand
            (
                Nom: request.Nom,
                Tel: request.Tel,
                Adresse: request.Adresse,
                Matricule: request.Matricule,
                Code: request.Code,
                CodeCat: request.CodeCat,
                EtbSec: request.EtbSec,
                Mail: request.Mail);

            var result = await mediator.Send(createClientCommand);

            //TODO map result.Errors to Microsoft.AspNetCore.Mvc.ProblemDetails #20
            if (result.IsFailed)
            {
                return TypedResults.BadRequest(result.Errors);
            }

            return TypedResults.Created($"/clients/{result.Value}", request);
        });
    }
}
