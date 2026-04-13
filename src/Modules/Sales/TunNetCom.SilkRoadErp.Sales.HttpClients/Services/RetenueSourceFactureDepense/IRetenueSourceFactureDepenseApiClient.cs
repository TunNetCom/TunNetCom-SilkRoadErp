using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceFactureDepense;

public interface IRetenueSourceFactureDepenseApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateRetenueSourceFactureDepenseAsync(
        CreateRetenueSourceFactureDepenseRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<RetenueSourceFactureDepenseResponse>> GetRetenueSourceFactureDepenseAsync(
        int factureDepenseId,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> GetRetenueSourceFactureDepensePdfAsync(
        int factureDepenseId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateRetenueSourceFactureDepenseAsync(
        int factureDepenseId,
        UpdateRetenueSourceFactureDepenseRequest request,
        CancellationToken cancellationToken = default);
}
