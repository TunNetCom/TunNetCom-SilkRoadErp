using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;
using TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.UpdateRetourMarchandiseFournisseur;

public class UpdateRetourMarchandiseFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/retour-marchandise-fournisseur",
            async ([FromBody] UpdateRetourMarchandiseFournisseurRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var retourLines = request.Lines.Select(x =>
                new RetourMarchandiseFournisseurLigne(
                    ProductRef: x.ProductRef,
                    Description: x.Description,
                    Quantity: x.Quantity,
                    UnitPrice: x.UnitPrice,
                    Discount: x.Discount,
                    Tax: x.Tax
                )).ToList();

            var command = new UpdateRetourMarchandiseFournisseurCommand(
                Num: request.Num,
                Date: request.Date,
                IdFournisseur: request.IdFournisseur,
                Lines: retourLines);

            var result = await mediator.Send(command, cancellationToken);
            if (result.IsFailed)
            {
                return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
            }

            return Results.Ok();
        })
        .WithTags(EndpointTags.RetourMarchandiseFournisseur)
        .WithName("UpdateRetourMarchandiseFournisseur")
        .WithSummary("Update a supplier return")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

