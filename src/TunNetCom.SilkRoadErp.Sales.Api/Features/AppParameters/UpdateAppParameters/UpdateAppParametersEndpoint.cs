using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;

public class UpdateAppParametersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Put endpoint for updating app parameters
        _ = app.MapPut(
                "/appParameters",
                async Task<Results<NoContent, NotFound, ValidationProblem>> (
                    IMediator mediator,

                    UpdateAppParametersRequest request,
                    CancellationToken cancellationToken) =>
                {
                    var updateAppParametersCommand = new UpdateAppParametersCommand(
                        NomSociete: request.NomSociete,
                        Timbre: request.Timbre,
                        Adresse: request.Adresse,
                        Tel: request.Tel,
                        Fax: request.Fax,
                        Email: request.Email,
                        MatriculeFiscale: request.MatriculeFiscale,
                        CodeTva: request.CodeTva,
                        CodeCategorie: request.CodeCategorie,
                        EtbSecondaire: request.EtbSecondaire,
                        PourcentageFodec: request.PourcentageFodec,
                        AdresseRetenu: request.AdresseRetenu,
                        PourcentageRetenu: request.PourcentageRetenu,
                        VatAmount: request.VatAmount,
                        DiscountPercentage: request.DiscountPercentage,
                        VatRate0: request.VatRate0,
                        VatRate7: request.VatRate7,
                        VatRate13: request.VatRate13,
                        VatRate19: request.VatRate19,
                        BloquerVenteStockInsuffisant: request.BloquerVenteStockInsuffisant,
                        DecimalPlaces: request.DecimalPlaces,
                        SeuilRetenueSource: request.SeuilRetenueSource);

                    var updateAppParametersResult = await mediator.Send(updateAppParametersCommand, cancellationToken);

                    if (updateAppParametersResult.IsFailed)
                    {
                        return updateAppParametersResult.ToValidationProblem();
                    }
                    return TypedResults.NoContent();
                })
            .WithName("UpdateAppParameters")
            .WithTags(EndpointTags.AppParameters)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest);
    }
}
