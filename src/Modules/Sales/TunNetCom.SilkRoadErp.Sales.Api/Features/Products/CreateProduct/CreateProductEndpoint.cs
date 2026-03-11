namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.CreateProduct;
public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
         "/products",
         async Task<Results<Created<CreateProductRequest>, ValidationProblem>>
         (IMediator mediator,
         CreateProductRequest request, CancellationToken cancellationToken) =>
         {
             var createProductCommand = new CreateProductCommand
            (
                Refe: request.Refe,
                Nom: request.Nom,
                QteLimite: request.QteLimite,
                Remise: request.Remise,
                RemiseAchat: request.RemiseAchat,
                Tva: request.Tva,
                Prix: request.Prix,
                PrixAchat: request.PrixAchat,
                Visibilite: request.Visibilite,
                SousFamilleProduitId: request.SousFamilleProduitId,
                Image1Base64: request.Image1Base64,
                Image2Base64: request.Image2Base64,
                Image3Base64: request.Image3Base64
             );

             var result = await mediator.Send(createProductCommand, cancellationToken);
             if (result.IsFailed)
             {
                 return result.ToValidationProblem();
             }
             return TypedResults.Created($"/products/{result.Value}", request);
         })
         .RequireAuthorization($"Permission:{Permissions.CreateProduct}")
         .WithTags(EndpointTags.Products);
    }
}
