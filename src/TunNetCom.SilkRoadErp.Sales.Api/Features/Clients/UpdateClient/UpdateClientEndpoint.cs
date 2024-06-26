namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/clients/{id:int}", async (IMediator mediator, int id, UpdateClientRequest request) =>
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

            await mediator.Send(updateClientCommand);

            return Results.NoContent();
        });
    }
}
