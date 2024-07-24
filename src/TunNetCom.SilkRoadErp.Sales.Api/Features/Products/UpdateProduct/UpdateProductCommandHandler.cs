namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.UpdateProduct;
public class UpdateProductCommandHandler(SalesContext _context ,ILogger<UpdateProductCommandHandler> _logger) : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand updateProductCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt("Product", updateProductCommand.Refe);
        var productToUpdate = await _context.Produit.FindAsync(updateProductCommand.Refe);

        if (productToUpdate is null)
        {
            _logger.LogEntityNotFound("Product", updateProductCommand.Refe);
            return Result.Fail("Product_not_found");
        }
        var isProductNameExist = await _context.Produit.AnyAsync(
                    pro => pro.Nom == updateProductCommand.Nom
                    && pro.Refe != updateProductCommand.Refe,
                    cancellationToken);

        if (isProductNameExist)
        {
            return Result.Fail("Product_name_exist");
        }
        productToUpdate.UpdateProduct(
            refe: updateProductCommand.Refe,
            nom: updateProductCommand.Nom,
            qte: updateProductCommand.Qte,
            qteLimite: updateProductCommand.QteLimite,
            remise: updateProductCommand.Remise,
            remiseAchat: updateProductCommand.RemiseAchat,
            tva: updateProductCommand.Tva,
            prix: updateProductCommand.Prix,
            prixAchat: updateProductCommand.PrixAchat,
            visibilite: updateProductCommand.Visibilite
            );
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogEntityUpdated("Product", updateProductCommand.Refe);
        return Result.Ok();
    }
}