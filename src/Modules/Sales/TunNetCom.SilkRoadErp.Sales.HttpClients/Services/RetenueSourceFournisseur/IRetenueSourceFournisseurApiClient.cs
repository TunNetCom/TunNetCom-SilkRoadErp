using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceFournisseur;

public interface IRetenueSourceFournisseurApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateRetenueSourceFournisseurAsync(
        CreateRetenueSourceFournisseurRequest request,
        CancellationToken cancellationToken);

    Task<Result<RetenueSourceFournisseurResponse>> GetRetenueSourceFournisseurAsync(
        int numFactureFournisseur,
        CancellationToken cancellationToken);

    Task<Result<byte[]>> GetRetenueSourceFournisseurPdfAsync(
        int numFactureFournisseur,
        CancellationToken cancellationToken);

    Task<Result> UpdateRetenueSourceFournisseurAsync(
        int numFactureFournisseur,
        UpdateRetenueSourceFournisseurRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeleteRetenueSourceFournisseurAsync(
        int numFactureFournisseur,
        CancellationToken cancellationToken);
}


