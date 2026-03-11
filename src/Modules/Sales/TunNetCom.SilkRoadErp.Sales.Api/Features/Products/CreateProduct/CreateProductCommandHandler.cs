using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.CreateProduct;

public class CreateProductCommandHandler(
    SalesContext _context,
    ILogger<CreateProductCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<CreateProductCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Produit), createProductCommand);

        var isProductRefeOrNameExist = await _context.Produit.AnyAsync(
            pro => pro.Refe == createProductCommand.Refe || pro.Nom == createProductCommand.Nom,
            cancellationToken);

        if (isProductRefeOrNameExist)
        {
            return Result.Fail("product_refe_or_name_exist");
        }

        string? image1StoragePath = null;
        string? image2StoragePath = null;
        string? image3StoragePath = null;

        // Process Image1
        if (!string.IsNullOrWhiteSpace(createProductCommand.Image1Base64))
        {
            try
            {
                var image1Bytes = Convert.FromBase64String(createProductCommand.Image1Base64);
                image1StoragePath = await _documentStorageService.SaveAsync(image1Bytes, $"product_{createProductCommand.Refe}_image1.jpg", cancellationToken);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 format for Image1");
                return Result.Fail("invalid_image1_format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing Image1");
                return Result.Fail("error_storing_image1");
            }
        }

        // Process Image2
        if (!string.IsNullOrWhiteSpace(createProductCommand.Image2Base64))
        {
            try
            {
                var image2Bytes = Convert.FromBase64String(createProductCommand.Image2Base64);
                image2StoragePath = await _documentStorageService.SaveAsync(image2Bytes, $"product_{createProductCommand.Refe}_image2.jpg", cancellationToken);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 format for Image2");
                return Result.Fail("invalid_image2_format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing Image2");
                return Result.Fail("error_storing_image2");
            }
        }

        // Process Image3
        if (!string.IsNullOrWhiteSpace(createProductCommand.Image3Base64))
        {
            try
            {
                var image3Bytes = Convert.FromBase64String(createProductCommand.Image3Base64);
                image3StoragePath = await _documentStorageService.SaveAsync(image3Bytes, $"product_{createProductCommand.Refe}_image3.jpg", cancellationToken);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 format for Image3");
                return Result.Fail("invalid_image3_format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing Image3");
                return Result.Fail("error_storing_image3");
            }
        }

        var product = Produit.CreateProduct(
            createProductCommand.Refe,
            createProductCommand.Nom,
            createProductCommand.QteLimite,
            createProductCommand.Remise,
            createProductCommand.RemiseAchat,
            createProductCommand.Tva,
            createProductCommand.Prix,
            createProductCommand.PrixAchat,
            createProductCommand.Visibilite,
            createProductCommand.SousFamilleProduitId,
            image1StoragePath,
            image2StoragePath,
            image3StoragePath
        );

        _ = _context.Produit.Add(product);
        _ = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogEntityCreatedSuccessfully(nameof(Produit), product.Refe);
        return product.Refe;
    }
}
