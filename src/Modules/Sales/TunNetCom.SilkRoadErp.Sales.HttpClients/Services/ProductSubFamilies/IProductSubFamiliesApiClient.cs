using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductSubFamilies;

public interface IProductSubFamiliesApiClient
{
    Task<List<SousFamilleProduitResponse>> GetAllAsync(int? familleProduitId = null, CancellationToken cancellationToken = default);
    Task<OneOf<SousFamilleProduitResponse, BadRequestResponse>> CreateAsync(CreateSousFamilleProduitRequest request, CancellationToken cancellationToken = default);
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(int id, UpdateSousFamilleProduitRequest request, CancellationToken cancellationToken = default);
    Task<OneOf<ResponseTypes, BadRequestResponse>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

