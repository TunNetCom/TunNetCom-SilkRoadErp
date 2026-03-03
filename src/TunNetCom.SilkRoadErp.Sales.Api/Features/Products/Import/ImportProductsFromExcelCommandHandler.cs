using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.Import;

public class ImportProductsFromExcelCommandHandler : IRequestHandler<ImportProductsFromExcelCommand, ProductImportResultResponse>
{
    private readonly SalesContext _context;
    private readonly IAccountingYearFinancialParametersService _financialParameters;
    private readonly ILogger<ImportProductsFromExcelCommandHandler> _logger;

    public ImportProductsFromExcelCommandHandler(
        SalesContext context,
        IAccountingYearFinancialParametersService financialParameters,
        ILogger<ImportProductsFromExcelCommandHandler> logger)
    {
        _context = context;
        _financialParameters = financialParameters;
        _logger = logger;
    }

    public async Task<ProductImportResultResponse> Handle(ImportProductsFromExcelCommand request, CancellationToken cancellationToken)
    {
        var createdCount = 0;
        var updatedCount = 0;
        var errors = new List<string>();
        var fodecRate = await _financialParameters.GetPourcentageFodecAsync(0, cancellationToken).ConfigureAwait(false);

        foreach (var row in request.Rows)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.Reference))
                {
                    errors.Add($"Ligne sans référence ignorée.");
                    continue;
                }

                var refe = row.Reference.Trim();
                var nom = string.IsNullOrWhiteSpace(row.Name) ? refe : row.Name.Trim();

                bool isSt = string.Equals(row.Societe, "ST", StringComparison.OrdinalIgnoreCase);
                decimal prixAchat = isSt
                    ? DecimalHelper.RoundAmount(row.PrixBrut * (1 + fodecRate / 100))
                    : DecimalHelper.RoundAmount(row.PrixBrut);
                decimal prix = isSt
                    ? DecimalHelper.RoundAmount(prixAchat * 1.30m * (1 + fodecRate / 100))
                    : DecimalHelper.RoundAmount(prixAchat * 1.30m);

                var existing = await _context.Produit
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Refe == refe, cancellationToken)
                    .ConfigureAwait(false);

                if (existing == null)
                {
                    var existsByName = await _context.Produit.AnyAsync(p => p.Nom == nom, cancellationToken).ConfigureAwait(false);
                    if (existsByName)
                    {
                        errors.Add($"Référence {refe}: un produit avec le nom '{nom}' existe déjà.");
                        continue;
                    }

                    var product = Produit.CreateProduct(
                        refe,
                        nom,
                        qteLimite: 0,
                        remise: 0,
                        remiseAchat: 0,
                        tva: 19,
                        prix,
                        prixAchat,
                        visibilite: true,
                        sousFamilleProduitId: null,
                        image1StoragePath: null,
                        image2StoragePath: null,
                        image3StoragePath: null);
                    _context.Produit.Add(product);
                    createdCount++;
                }
                else
                {
                    var productToUpdate = await _context.Produit.FirstOrDefaultAsync(p => p.Refe == refe, cancellationToken).ConfigureAwait(false);
                    if (productToUpdate == null)
                    {
                        errors.Add($"Référence {refe}: produit introuvable pour mise à jour.");
                        continue;
                    }

                    productToUpdate.UpdateProduct(
                        productToUpdate.Refe,
                        string.IsNullOrWhiteSpace(row.Name) ? productToUpdate.Nom : nom,
                        productToUpdate.QteLimite,
                        productToUpdate.Remise,
                        productToUpdate.RemiseAchat,
                        productToUpdate.Tva,
                        prix,
                        prixAchat,
                        productToUpdate.Visibilite,
                        productToUpdate.SousFamilleProduitId,
                        productToUpdate.Image1StoragePath,
                        productToUpdate.Image2StoragePath,
                        productToUpdate.Image3StoragePath);
                    updatedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erreur import ligne référence {Ref}", row.Reference);
                errors.Add($"Référence {row.Reference}: {ex.Message}");
            }
        }

        if (createdCount > 0 || updatedCount > 0)
        {
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return new ProductImportResultResponse
        {
            CreatedCount = createdCount,
            UpdatedCount = updatedCount,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }
}
