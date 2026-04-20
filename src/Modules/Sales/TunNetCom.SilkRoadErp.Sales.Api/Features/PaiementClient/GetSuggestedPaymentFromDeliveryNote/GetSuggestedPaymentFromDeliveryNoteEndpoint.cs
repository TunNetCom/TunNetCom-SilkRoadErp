using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetSuggestedPaymentFromDeliveryNote;

public class GetSuggestedPaymentFromDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiement-client/suggested-from-delivery-note", HandleAsync)
            .WithTags(EndpointTags.PaiementClient)
            .Produces<SuggestedPaymentFromDeliveryNoteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<Results<Ok<SuggestedPaymentFromDeliveryNoteResponse>, NotFound>> HandleAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<GetSuggestedPaymentFromDeliveryNoteEndpoint> logger,
        [FromQuery] int deliveryNoteId,
        CancellationToken cancellationToken)
    {
        var bl = await context.BonDeLivraison
            .FilterByActiveAccountingYear()
            .AsNoTracking()
            .Where(b => b.Id == deliveryNoteId)
            .Select(b => new { b.Id, b.NetPayer, b.NumFacture })
            .FirstOrDefaultAsync(cancellationToken);

        if (bl == null)
        {
            return TypedResults.NotFound();
        }

        // Default: if BL is not invoiced, suggest paying the BL amount (TTC) itself.
        decimal suggestedAmount = bl.NetPayer;
        int? invoiceNumber = bl.NumFacture;
        int? invoiceId = null;

        if (invoiceNumber.HasValue)
        {
            // timbre comes from accounting year financial parameters (server-side)
            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var timbre = await financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

            // Invoice amount (TTC): sum of BL.NetPayer for this invoice + timbre (once per invoice)
            var sumNetPayer = await context.BonDeLivraison
                .FilterByActiveAccountingYear()
                .AsNoTracking()
                .Where(b => b.NumFacture == invoiceNumber.Value)
                .SumAsync(b => (decimal?)b.NetPayer, cancellationToken) ?? 0m;

            suggestedAmount = sumNetPayer + timbre;

            invoiceId = await context.Facture
                .FilterByActiveAccountingYear()
                .AsNoTracking()
                .Where(f => f.Num == invoiceNumber.Value)
                .Select(f => (int?)f.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        logger.LogInformation(
            "Suggested payment computed for BL {DeliveryNoteId}: InvoiceNumber={InvoiceNumber}, InvoiceId={InvoiceId}, SuggestedAmount={SuggestedAmount}",
            deliveryNoteId, invoiceNumber, invoiceId, suggestedAmount);

        return TypedResults.Ok(new SuggestedPaymentFromDeliveryNoteResponse
        {
            DeliveryNoteId = deliveryNoteId,
            InvoiceId = invoiceId,
            InvoiceNumber = invoiceNumber,
            SuggestedAmount = suggestedAmount
        });
    }
}

