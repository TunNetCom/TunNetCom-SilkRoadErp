using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.ProductImport;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.Import;

public class ProductImportPreviewEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/products/import/preview", HandlePreviewAsync)
            .WithTags(EndpointTags.Products)
            .DisableAntiforgery()
            .RequireAuthorization($"Permission:{Permissions.CreateProduct}")
            .Produces<ProductImportPreviewResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }

    public static async Task<Results<Ok<ProductImportPreviewResponse>, ValidationProblem>> HandlePreviewAsync(
        IProductPriceListExcelParser parser,
        IFormFile? file,
        [FromQuery] int sheetIndex = 0,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = new[] { "file_is_required" }
            });
        }

        var extension = Path.GetExtension(file.FileName ?? "").ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = new[] { "only_xlsx_or_xls_allowed" }
            });
        }

        await using var stream = file.OpenReadStream();
        var response = await parser.PreviewAsync(stream, sheetIndex, maxPreviewRows: 50, cancellationToken).ConfigureAwait(false);
        return TypedResults.Ok(response);
    }
}
