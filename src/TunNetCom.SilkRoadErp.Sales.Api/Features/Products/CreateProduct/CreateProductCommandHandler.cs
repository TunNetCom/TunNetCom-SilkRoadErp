namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.CreateProduct;

public class CreateProductCommandHandler(SalesContext _context, ILogger<CreateProductCommandHandler> _logger) : IRequestHandler<CreateProductCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated("Product", createProductCommand);

        var isProductNameExist = await _context.Produit.AnyAsync(
            pro => pro.Nom == createProductCommand.Nom,
            cancellationToken);

        if (isProductNameExist)
        {
            return Result.Fail("product_name_exist");
        }
        var product = Produit.CreateProduct(
            createProductCommand.Refe,
            createProductCommand.Nom,
            createProductCommand.Qte,
            createProductCommand.QteLimite,
            createProductCommand.Remise,
            createProductCommand.RemiseAchat,
            createProductCommand.Tva,
            createProductCommand.Prix,
            createProductCommand.PrixAchat,
            createProductCommand.Visibilite
        );
      

        _context.Produit.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogEntityCreatedSuccessfully("Product", product.Refe);
        return product.Refe;
    }
}
