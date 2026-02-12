using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Inventaire;

public interface IInventaireApiClient
{
    Task<PagedList<InventaireResponse>> GetPagedAsync(
        GetInventairesQueryParams queryParameters,
        CancellationToken cancellationToken);

    Task<OneOf<FullInventaireResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<CreateInventaireRequest, BadRequestResponse>> CreateAsync(
        CreateInventaireRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        UpdateInventaireRequest request,
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, Stream>> DeleteAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> ValiderAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> CloturerAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<decimal, bool>> GetDernierPrixAchatAsync(
        string refProduit,
        CancellationToken cancellationToken);

    Task<OneOf<List<HistoriqueAchatVenteResponse>, bool>> GetHistoriqueAchatVenteAsync(
        int productId,
        CancellationToken cancellationToken);
}

