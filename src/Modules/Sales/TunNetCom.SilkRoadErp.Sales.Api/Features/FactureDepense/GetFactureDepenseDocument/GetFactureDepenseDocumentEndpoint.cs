using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFactureDepenseDocument;

public class GetFactureDepenseDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/factures-depenses/{id:int}/document", HandleGetFactureDepenseDocumentAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.FactureDepense);
    }

    public async Task<Results<IResult, NotFound>> HandleGetFactureDepenseDocumentAsync(
        SalesContext context,
        IDocumentStorageService documentStorageService,
        int id,
        CancellationToken cancellationToken)
    {
        var facture = await context.FactureDepense
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        if (facture == null || string.IsNullOrWhiteSpace(facture.DocumentStoragePath))
        {
            return TypedResults.NotFound();
        }

        try
        {
            var documentBytes = await documentStorageService.GetAsync(facture.DocumentStoragePath, cancellationToken);

            var contentType = "image/jpeg";
            if (facture.DocumentStoragePath.StartsWith("data:application/pdf", StringComparison.OrdinalIgnoreCase) ||
                facture.DocumentStoragePath.Contains("pdf", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/pdf";
            }
            else if (facture.DocumentStoragePath.StartsWith("data:image/png", StringComparison.OrdinalIgnoreCase) ||
                     facture.DocumentStoragePath.Contains("png", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/png";
            }
            else if (facture.DocumentStoragePath.StartsWith("data:image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                     facture.DocumentStoragePath.StartsWith("data:image/jpg", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/jpeg";
            }

            var fileName = $"facture_depense_{facture.Num}.{(contentType == "application/pdf" ? "pdf" : "jpg")}";

            return TypedResults.File(documentBytes, contentType, fileName);
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }
}
