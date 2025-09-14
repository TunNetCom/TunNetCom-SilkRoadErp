using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class CreateDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/deliveryNote", HandleCreateDeliveryNoteAsync);
    }

    public async Task<Results<Created<CreateDeliveryNoteRequest>, ValidationProblem>> HandleCreateDeliveryNoteAsync(
        IMediator mediator,
        CreateDeliveryNoteRequest createDeliveryNoteRequest,
        CancellationToken cancellationToken)
    {
        var createDeliveryNoteCommand = new CreateDeliveryNoteCommand
        (
            Date: createDeliveryNoteRequest.Date,
            TotHTva: createDeliveryNoteRequest.TotalExcludingTax,
            TotTva: createDeliveryNoteRequest.TotalVat,
            NetPayer: createDeliveryNoteRequest.TotalAmount,
            TempBl: createDeliveryNoteRequest.DeliveryTime,
            NumFacture: createDeliveryNoteRequest.InvoiceNumber,
            ClientId: createDeliveryNoteRequest.CustomerId,
            DeliveryNoteDetails: createDeliveryNoteRequest.Items.Select(selector => new LigneBlSubCommand
            {
                RefProduit = selector.ProductReference,
                DesignationLi = selector.Description,
                QteLi = selector.Quantity,
                PrixHt = selector.UnitPriceExcludingTax,
                Remise = selector.DiscountPercentage,
                TotHt = selector.TotalExcludingTax,
                Tva = selector.VatPercentage,
                TotTtc = selector.TotalIncludingTax
            })
        );

        var result = await mediator.Send(createDeliveryNoteCommand, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/deliveryNote/{result.Value}", createDeliveryNoteRequest);
    }
}
