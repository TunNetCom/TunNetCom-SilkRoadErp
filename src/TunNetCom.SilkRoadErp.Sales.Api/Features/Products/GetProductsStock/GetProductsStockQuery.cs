using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductsStock;

public record GetProductsStockQuery(List<string> RefProduits) : IRequest<Result<List<ProductStockResponse>>>;

