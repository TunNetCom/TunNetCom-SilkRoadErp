using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementTiersDepenseDocument;

public class GetPaiementTiersDepenseDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiements-tiers-depenses/{id:int}/document", HandleGetPaiementTiersDepenseDocumentAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewPaiementTiersDepense}")
            .WithTags(EndpointTags.PaiementTiersDepense);
    }

    public async Task<Results<IResult, NotFound>> HandleGetPaiementTiersDepenseDocumentAsync(
        SalesContext context,
        IDocumentStorageService documentStorageService,
        int id,
        CancellationToken cancellationToken)
    {
        var paiement = await context.PaiementTiersDepense
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (paiement == null || string.IsNullOrWhiteSpace(paiement.DocumentStoragePath))
        {
            return TypedResults.NotFound();
        }

        try
        {
            var documentBytes = await documentStorageService.GetAsync(paiement.DocumentStoragePath, cancellationToken);

            var contentType = "image/jpeg";
            if (paiement.DocumentStoragePath.StartsWith("data:application/pdf", StringComparison.OrdinalIgnoreCase) ||
                paiement.DocumentStoragePath.Contains("pdf", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/pdf";
            }
            else if (paiement.DocumentStoragePath.StartsWith("data:image/png", StringComparison.OrdinalIgnoreCase) ||
                     paiement.DocumentStoragePath.Contains("png", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/png";
            }
            else if (paiement.DocumentStoragePath.StartsWith("data:image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                     paiement.DocumentStoragePath.StartsWith("data:image/jpg", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/jpeg";
            }

            var fileName = $"paiement_tiers_depense_{paiement.NumeroTransactionBancaire}.{(contentType == "application/pdf" ? "pdf" : "jpg")}";

            return TypedResults.File(documentBytes, contentType, fileName);
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }
}
