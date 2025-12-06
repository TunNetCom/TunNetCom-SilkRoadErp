using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;

public class UpdateDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/deliveryNote/{num:int}", HandleUpdateDeliveryNoteAsync)
            .WithTags(EndpointTags.DeliveryNotes);
    }

    public async Task<Results<NoContent, NotFound, ValidationProblem>> HandleUpdateDeliveryNoteAsync(
        IMediator mediator,
        int num,
        CreateDeliveryNoteRequest updateDeliveryNoteRequest,
        CancellationToken cancellationToken)
    {
        var updateDeliveryNoteCommand = new UpdateDeliveryNoteCommand(
            Num: num,
            Date: updateDeliveryNoteRequest.Date,
            TotHTva: updateDeliveryNoteRequest.TotalExcludingTax,
            TotTva: updateDeliveryNoteRequest.TotalVat,
            NetPayer: updateDeliveryNoteRequest.TotalAmount,
            TempBl: updateDeliveryNoteRequest.DeliveryTime,
            NumFacture: updateDeliveryNoteRequest.InvoiceNumber,
            ClientId: updateDeliveryNoteRequest.CustomerId,
            InstallationTechnicianId: updateDeliveryNoteRequest.InstallationTechnicianId,
            DeliveryCarId: updateDeliveryNoteRequest.DeliveryCarId,
            DeliveryNoteDetails: updateDeliveryNoteRequest.Items.Select(selector => new LigneBlSubCommand
            {
                Id = selector.Id,
                RefProduit = selector.ProductReference,
                DesignationLi = selector.Description,
                QteLi = selector.Quantity,
                QteLivree = selector.DeliveredQuantity,
                PrixHt = selector.UnitPriceExcludingTax,
                Remise = selector.DiscountPercentage,
                TotHt = selector.TotalExcludingTax,
                Tva = selector.VatPercentage,
                TotTtc = selector.TotalIncludingTax
            })
        );

        var result = await mediator.Send(updateDeliveryNoteCommand, cancellationToken);

        if (result.IsEntityNotFound())
        {
            return TypedResults.NotFound();
        }

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

