namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.DeleteProduct;
public class DeleteProductCommandHandler(SalesContext _context, ILogger<DeleteProductCommandHandler> _logger) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand deleteProductCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt("Product", deleteProductCommand.Refe);

        var product = await _context.Produit.FindAsync(deleteProductCommand.Refe);
        if (product is null)
        {
            _logger.LogEntityNotFound("Product", deleteProductCommand.Refe);
            return Result.Fail("product_not_found");
        }
        _context.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted("Product",deleteProductCommand.Refe);
        return Result.Ok();
    }
}
