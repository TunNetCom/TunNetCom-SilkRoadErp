namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.Import;

/// <summary>
/// One parsed row from the product price list Excel file.
/// </summary>
public sealed class ProductImportRowDto
{
    public string Reference { get; set; } = string.Empty;
    public string? Name { get; set; }
    public decimal PrixBrut { get; set; }
    public string? Societe { get; set; }
}
