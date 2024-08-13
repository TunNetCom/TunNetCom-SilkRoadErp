namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.CreateProduct;

public class CreateProductCommandHandler(
    SalesContext _context,
    ILogger<CreateProductCommandHandler> _logger)
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
        _logger.LogEntityCreatedSuccessfully(nameof(Produit), product.Refe);
        return product.Refe;
    }
}
