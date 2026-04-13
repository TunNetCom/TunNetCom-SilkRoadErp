using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur;

public class CreateRetourMarchandiseFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/retour-marchandise-fournisseur",
            async ([FromBody] CreateRetourMarchandiseFournisseurRequest request,
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
                    Tax: x.Tax,
                    QteRecue: x.QteRecue
                )).ToList();

            var command = new CreateRetourMarchandiseFournisseurCommand(
                Date: request.Date,
                IdFournisseur: request.IdFournisseur,
                Lines: retourLines);

            var result = await mediator.Send(command, cancellationToken);
            if (result.IsFailed)
            {
                return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
            }

            return Results.Created($"/retour-marchandise-fournisseur/{result.Value}", new { num = result.Value });
        })
        .WithTags(EndpointTags.RetourMarchandiseFournisseur)
        .WithName("CreateRetourMarchandiseFournisseur")
        .WithSummary("Create a new supplier return")
        .Produces<int>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

