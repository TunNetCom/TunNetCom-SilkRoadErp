namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients", async (IMediator mediator, CreateClientRequest request) =>
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
                Mail: request.Mail
                );
            var result = await mediator.Send(createClientCommand);
            return Results.Created($"/clients/{result.Id}", result);
        });
    }
}
