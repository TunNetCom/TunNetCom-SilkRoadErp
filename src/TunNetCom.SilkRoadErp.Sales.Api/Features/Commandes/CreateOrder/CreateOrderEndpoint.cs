using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.CreateOrder;

public class CreateOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
           "/orders",
           async Task<Results<Created<CreateOrderRequest>, ValidationProblem>> (
               IMediator mediator,
               CreateOrderRequest request,
               CancellationToken cancellationToken) =>
           {
               var orderLines = request.Items.Select(item => new LigneCommandeSubCommand
               {
                   RefProduit = item.ProductReference,
                   DesignationLi = item.Description,
                   QteLi = item.Quantity,
                   PrixHt = item.UnitPriceExcludingTax,
                   Remise = item.DiscountPercentage,
                   TotHt = item.TotalExcludingTax,
                   Tva = item.VatPercentage,
                   TotTtc = item.TotalIncludingTax
               });

               var createOrderCommand = new CreateOrderCommand(
                   FournisseurId: request.FournisseurId,
                   Date: request.Date,
                   TotHTva: request.TotHTva,
                   TotTva: request.TotTva,
                   TotTtc: request.TotTtc,
                   OrderLines: orderLines
               );

               var result = await mediator.Send(createOrderCommand, cancellationToken);

               if (result.IsFailed)
               {
                   return result.ToValidationProblem();
               }

               return TypedResults.Created($"/orders/{result.Value}", request);
           })
           .WithTags(EndpointTags.Orders);
    }
}

