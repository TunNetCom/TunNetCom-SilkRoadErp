using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.UpdateOrder;

public class UpdateOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/orders/{num:int}",
        async Task<Results<NoContent, NotFound, ValidationProblem>> (
            IMediator mediator, int num,
            UpdateOrderRequest request,
            CancellationToken cancellationToken) =>
        {
            var orderLines = request.Items.Select(item => new CreateOrder.LigneCommandeSubCommand
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

            var updateOrderCommand = new UpdateOrderCommand(
                Num: num,
                FournisseurId: request.FournisseurId,
                Date: request.Date,
                TotHTva: request.TotHTva,
                TotTva: request.TotTva,
                TotTtc: request.TotTtc,
                OrderLines: orderLines);

            var updateOrderResult = await mediator.Send(updateOrderCommand, cancellationToken);

            if (updateOrderResult.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }

            if (updateOrderResult.IsFailed)
            {
                return updateOrderResult.ToValidationProblem();
            }

            return TypedResults.NoContent();
        })
        .WithTags(EndpointTags.Orders);
    }
}

