using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.ProductImport;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.Import;

public class ProductImportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/products/import", HandleImportAsync)
            .WithTags(EndpointTags.Products)
            .DisableAntiforgery()
            .RequireAuthorization($"Permission:{Permissions.CreateProduct}")
            .Produces<ProductImportResultResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }

    public static async Task<Results<Ok<ProductImportResultResponse>, ValidationProblem>> HandleImportAsync(
        IProductPriceListExcelParser parser,
        IMediator mediator,
        HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!request.HasFormContentType)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["form"] = new[] { "form_data_required" }
            });
        }

        var form = await request.ReadFormAsync(cancellationToken).ConfigureAwait(false);
        var file = form.Files.GetFile("file");
        if (file == null || file.Length == 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = new[] { "file_is_required" }
            });
        }

        var mappingJson = form["mapping"].ToString();
        if (string.IsNullOrWhiteSpace(mappingJson))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["mapping"] = new[] { "mapping_is_required" }
            });
        }

        ProductImportMappingRequest? mapping;
        try
        {
            mapping = System.Text.Json.JsonSerializer.Deserialize<ProductImportMappingRequest>(mappingJson);
        }
        catch (System.Text.Json.JsonException)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["mapping"] = new[] { "mapping_invalid_json" }
            });
        }

        if (mapping == null || string.IsNullOrWhiteSpace(mapping.ReferenceColumn) || string.IsNullOrWhiteSpace(mapping.PrixBrutColumn))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["mapping"] = new[] { "reference_and_prix_brut_columns_required" }
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
        var rows = await parser.ParseWithMappingAsync(stream, mapping, cancellationToken).ConfigureAwait(false);

        var command = new ImportProductsFromExcelCommand(rows);
        var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(result);
    }
}
