using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.Import;

public record ImportProductsFromExcelCommand(IReadOnlyList<ProductImportRowDto> Rows) : IRequest<ProductImportResultResponse>;
