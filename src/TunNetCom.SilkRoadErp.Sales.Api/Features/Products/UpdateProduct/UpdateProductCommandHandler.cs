using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.UpdateProduct;

public class UpdateProductCommandHandler(
    SalesContext _context,
    ILogger<UpdateProductCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(
        UpdateProductCommand updateProductCommand,
        CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(Produit), updateProductCommand.Refe);
        var productToUpdate = await _context.Produit.FindAsync(updateProductCommand.Refe);

        if (productToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Produit), updateProductCommand.Refe);
            return Result.Fail(EntityNotFound.Error());
        }

        //var isProductNameExist = await _context.Produit.AnyAsync(
        //            pro => pro.Nom == updateProductCommand.Nom
        //            && pro.Refe != updateProductCommand.Refe,
        //            cancellationToken);

        //if (isProductNameExist)
        //{
        //    return Result.Fail("product_name_exist");
        //}

        string? image1StoragePath = productToUpdate.Image1StoragePath;
        string? image2StoragePath = productToUpdate.Image2StoragePath;
        string? image3StoragePath = productToUpdate.Image3StoragePath;

        // Process Image1
        if (!string.IsNullOrWhiteSpace(updateProductCommand.Image1Base64))
        {
            try
            {
                // Delete old image if exists
                if (!string.IsNullOrWhiteSpace(productToUpdate.Image1StoragePath))
                {
                    try
                    {
                        await _documentStorageService.DeleteAsync(productToUpdate.Image1StoragePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old Image1, continuing with new upload");
                    }
                }

                var image1Bytes = Convert.FromBase64String(updateProductCommand.Image1Base64);
                image1StoragePath = await _documentStorageService.SaveAsync(image1Bytes, $"product_{updateProductCommand.Refe}_image1.jpg", cancellationToken);
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
        if (!string.IsNullOrWhiteSpace(updateProductCommand.Image2Base64))
        {
            try
            {
                // Delete old image if exists
                if (!string.IsNullOrWhiteSpace(productToUpdate.Image2StoragePath))
                {
                    try
                    {
                        await _documentStorageService.DeleteAsync(productToUpdate.Image2StoragePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old Image2, continuing with new upload");
                    }
                }

                var image2Bytes = Convert.FromBase64String(updateProductCommand.Image2Base64);
                image2StoragePath = await _documentStorageService.SaveAsync(image2Bytes, $"product_{updateProductCommand.Refe}_image2.jpg", cancellationToken);
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
        if (!string.IsNullOrWhiteSpace(updateProductCommand.Image3Base64))
        {
            try
            {
                // Delete old image if exists
                if (!string.IsNullOrWhiteSpace(productToUpdate.Image3StoragePath))
                {
                    try
                    {
                        await _documentStorageService.DeleteAsync(productToUpdate.Image3StoragePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old Image3, continuing with new upload");
                    }
                }

                var image3Bytes = Convert.FromBase64String(updateProductCommand.Image3Base64);
                image3StoragePath = await _documentStorageService.SaveAsync(image3Bytes, $"product_{updateProductCommand.Refe}_image3.jpg", cancellationToken);
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

        productToUpdate.UpdateProduct(
            refe: updateProductCommand.Refe,
            nom: updateProductCommand.Nom,
            qteLimite: updateProductCommand.QteLimite,
            remise: updateProductCommand.Remise,
            remiseAchat: updateProductCommand.RemiseAchat,
            tva: updateProductCommand.Tva,
            prix: updateProductCommand.Prix,
            prixAchat: updateProductCommand.PrixAchat,
            visibilite: updateProductCommand.Visibilite,
            sousFamilleProduitId: updateProductCommand.SousFamilleProduitId,
            image1StoragePath: image1StoragePath,
            image2StoragePath: image2StoragePath,
            image3StoragePath: image3StoragePath
            );

        _ = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogEntityUpdated(nameof(Produit), updateProductCommand.Refe);

        return Result.Ok();
    }
}