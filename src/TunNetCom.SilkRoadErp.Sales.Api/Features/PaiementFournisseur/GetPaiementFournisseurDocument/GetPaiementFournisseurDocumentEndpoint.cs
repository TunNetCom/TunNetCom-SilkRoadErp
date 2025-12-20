using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementFournisseurDocument;

public class GetPaiementFournisseurDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiement-fournisseur/{id:int}/document", HandleGetPaiementFournisseurDocumentAsync)
            .WithTags(EndpointTags.PaiementFournisseur);
    }

    public async Task<Results<IResult, NotFound>> HandleGetPaiementFournisseurDocumentAsync(
        SalesContext context,
        IDocumentStorageService documentStorageService,
        int id,
        CancellationToken cancellationToken)
    {
        var paiement = await context.PaiementFournisseur
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (paiement == null || string.IsNullOrWhiteSpace(paiement.DocumentStoragePath))
        {
            return TypedResults.NotFound();
        }

        try
        {
            var documentBytes = await documentStorageService.GetAsync(paiement.DocumentStoragePath, cancellationToken);
            
            // Determine content type based on file extension or default to image/jpeg
            var contentType = "image/jpeg"; // Default
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

            var fileName = $"paiement_fournisseur_{paiement.Numero}.{(contentType == "application/pdf" ? "pdf" : "jpg")}";
            
            return TypedResults.File(documentBytes, contentType, fileName);
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }
}
