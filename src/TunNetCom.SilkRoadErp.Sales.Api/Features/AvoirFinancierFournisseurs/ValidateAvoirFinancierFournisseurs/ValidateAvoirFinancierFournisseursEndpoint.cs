using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.ValidateAvoirFinancierFournisseurs;

public class ValidateAvoirFinancierFournisseursEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/avoir-financier-fournisseurs/validate", HandleValidateAvoirFinancierFournisseursAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs);
    }

    public async Task<Results<Ok, ValidationProblem>> HandleValidateAvoirFinancierFournisseursAsync(
        IMediator mediator,
        ValidateAvoirFinancierFournisseursRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ValidateAvoirFinancierFournisseursCommand(request.Ids);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok();
    }
}

