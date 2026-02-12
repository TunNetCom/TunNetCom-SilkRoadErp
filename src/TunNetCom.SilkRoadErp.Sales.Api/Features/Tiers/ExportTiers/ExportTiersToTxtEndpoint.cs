using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tiers.ExportTiers;

public class ExportTiersToTxtEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tiers/export/txt", HandleExportToTxtAsync)
            .WithTags(EndpointTags.Tiers)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithDescription("Exports clients or suppliers to a text file in Tiers format")
            .RequireAuthorization()
            .WithOpenApi();
    }
 
    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToTxtAsync(
        [FromQuery] string? type,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<ExportTiersToTxtEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("ExportTiersToTxtEndpoint called with type: {Type}", type);
 
            TierType? tierType = type?.ToLower() switch
            {
                "client" => TierType.Client,
                "fournisseur" => TierType.Supplier,
                _ => null
            };

            var query = new ExportTiersToTxtQuery(tierType);
            var fileBytes = await mediator.Send(query, cancellationToken);
 
            var prefix = tierType switch
            {
                TierType.Client => "Clients",
                TierType.Supplier => "Fournisseurs",
                _ => "Tiers"
            };

            var filename = $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
 
            logger.LogInformation("{Prefix} export successful. File size: {Size} bytes", prefix, fileBytes.Length);
 
            return TypedResults.File(
                fileBytes,
                contentType: "text/plain",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting tiers to text file");
            return TypedResults.StatusCode(500);
        }
    }
}
