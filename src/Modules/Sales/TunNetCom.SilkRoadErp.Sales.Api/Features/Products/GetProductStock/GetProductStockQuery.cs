using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductStock;

public record GetProductStockQuery(string RefProduit) : IRequest<Result<ProductStockResponse>>;

