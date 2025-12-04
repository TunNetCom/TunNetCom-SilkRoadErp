namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.UpdateProduct;
public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut(
            "/products/{refe}",
            async Task<Results<NoContent, NotFound, BadRequest<List<IError>>>> (
            IMediator mediator,
            string refe,
            UpdateProductRequest updateProductRequest,
            CancellationToken cancellationToken) =>
        {
            var updateProductCommand = new UpdateProductCommand(
                Refe: updateProductRequest.Refe,
                Nom: updateProductRequest.Nom,
                QteLimite: updateProductRequest.QteLimite,
                Remise: updateProductRequest.Remise,
                RemiseAchat: updateProductRequest.RemiseAchat,
                Tva: updateProductRequest.Tva,
                Prix: updateProductRequest.Prix,
                PrixAchat: updateProductRequest.PrixAchat,
                Visibilite: updateProductRequest.Visibilite,
                SousFamilleProduitId: updateProductRequest.SousFamilleProduitId,
                Image1Base64: updateProductRequest.Image1Base64,
                Image2Base64: updateProductRequest.Image2Base64,
                Image3Base64: updateProductRequest.Image3Base64
            );

            var result = await mediator.Send(updateProductCommand, cancellationToken);

            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }

            if (result.IsFailed)
            {
                return TypedResults.BadRequest(result.Errors);
            }

            return TypedResults.NoContent();
        })
        .RequireAuthorization($"Permission:{Permissions.UpdateProduct}")
        .WithTags(EndpointTags.Products);
    }
}