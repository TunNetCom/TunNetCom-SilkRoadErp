using Microsoft.AspNetCore.Http.HttpResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/clients/{id:int}",
            async Task<Results<NoContent, BadRequest<List<IError>>>> (IMediator mediator, int id, UpdateClientRequest request) =>
        {
            var updateClientCommand = new UpdateClientCommand(
                Id: id,
                Nom: request.Nom,
                Tel: request.Tel,
                Adresse: request.Adresse,
                Matricule: request.Matricule,
                Code: request.Code,
                CodeCat: request.CodeCat,
                EtbSec: request.EtbSec,
                Mail: request.Mail);

            var result = await mediator.Send(updateClientCommand);

            //TODO map result.Errors to Microsoft.AspNetCore.Mvc.ProblemDetails #20
            if (result.IsFailed)
            {
                return TypedResults.BadRequest(result.Errors);
            }

            return TypedResults.NoContent();
        });
    }
}
