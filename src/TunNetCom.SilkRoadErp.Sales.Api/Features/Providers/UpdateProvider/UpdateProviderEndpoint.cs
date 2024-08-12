namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.UpdateProvider;

public class UpdateProviderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/providers/{id:int}",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator, int id,
                UpdateProviderRequest request,
                CancellationToken cancellationToken) =>
            {
                var updateProviderCommand = new UpdateProviderCommand(
                   Id: id,
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

                var updateProviderResult = await mediator.Send(updateProviderCommand, cancellationToken);

                if (updateProviderResult.HasError<EntityNotFound>())
                {
                    return TypedResults.NotFound();
                }

                if (updateProviderResult.IsFailed)
                {
                    return updateProviderResult.ToValidationProblem();
                }

                return TypedResults.NoContent();
            });
    }
}
