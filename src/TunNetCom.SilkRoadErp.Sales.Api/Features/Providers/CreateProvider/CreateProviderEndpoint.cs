namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.CreateProvider;
public class CreateProviderEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/providers", async Task<Results<Created<CreateProviderRequest>, BadRequest<List<IError>>>> (IMediator mediator,
            CreateProviderRequest request) =>
        {
            var createProviderCommand = new CreateProviderCommand(
                 Nom: request.Nom,
                 Tel: request.Tel,
                 Fax: request.Fax,
                 Matricule: request.Matricule,
                 Code: request.Code,
                 CodeCat: request.CodeCat,
                 EtbSec: request.EtbSec,
                 Mail: request.Mail,
                 MailDeux: request.MailDeux,
                 Constructeur: request.Constructeur,
                 Adresse: request.Adresse);

            var result = await mediator.Send(createProviderCommand);
            if (result.IsFailed)
            {
                return TypedResults.BadRequest(result.Errors);
            }
            return TypedResults.Created($"/providers/{result.Value}", request);

        });
    }

}
