using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.UpdateProduct
{
    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/Products/{refe}", async Task<Results<NoContent, BadRequest<List<IError>>>> (
                IMediator mediator, string refe, UpdateProductRequest updateProductRequest) =>
            {
                var updateProductCommand = new UpdateProductCommand(
                    Refe: updateProductRequest.Refe,
                    Nom: updateProductRequest.Nom,
                    Qte: updateProductRequest.Qte,
                    QteLimite: updateProductRequest.QteLimite,
                    Remise: updateProductRequest.Remise,
                    RemiseAchat: updateProductRequest.RemiseAchat,
                    Tva: updateProductRequest.Tva,
                    Prix: updateProductRequest.Prix,
                    PrixAchat: updateProductRequest.PrixAchat,
                    Visibilite: updateProductRequest.Visibilite
                );

                var result = await mediator.Send(updateProductCommand);

                if (result.IsFailed)
                {
                    return TypedResults.BadRequest(result.Errors);
                }

                return TypedResults.NoContent();
            });
        }
    }

}
