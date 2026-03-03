using TunNetCom.SilkRoadErp.Sales.Api.Features.Products.Import;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.ProductImport;

public interface IProductPriceListExcelParser
{
    /// <summary>
    /// Reads the Excel file and returns sheet names, headers of the selected sheet, and first N data rows for preview.
    /// </summary>
    Task<ProductImportPreviewResponse> PreviewAsync(Stream stream, int sheetIndex = 0, int maxPreviewRows = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses the Excel file with the given column mapping and returns all data rows for import.
    /// </summary>
    Task<IReadOnlyList<ProductImportRowDto>> ParseWithMappingAsync(Stream stream, ProductImportMappingRequest mapping, CancellationToken cancellationToken = default);
}
