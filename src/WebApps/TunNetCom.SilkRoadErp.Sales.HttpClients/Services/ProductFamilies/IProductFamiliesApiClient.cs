using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductFamilies;

public interface IProductFamiliesApiClient
{
    Task<List<FamilleProduitResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OneOf<FamilleProduitResponse, BadRequestResponse>> CreateAsync(CreateFamilleProduitRequest request, CancellationToken cancellationToken = default);
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(int id, UpdateFamilleProduitRequest request, CancellationToken cancellationToken = default);
    Task<OneOf<ResponseTypes, BadRequestResponse>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

