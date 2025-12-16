using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetourMarchandiseFournisseur;

public interface IRetourMarchandiseFournisseurApiClient
{
    Task<Result<int>> CreateRetourMarchandiseFournisseurAsync(
        CreateRetourMarchandiseFournisseurRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateRetourMarchandiseFournisseurAsync(
        UpdateRetourMarchandiseFournisseurRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<RetourMarchandiseFournisseurResponse>> GetRetourMarchandiseFournisseurAsync(
        int num,
        CancellationToken cancellationToken = default);

    Task<Result> ValidateRetourMarchandiseFournisseurAsync(
        List<int> ids,
        CancellationToken cancellationToken = default);

    Task<Result<VerifyReceptionResponse>> VerifyReceptionAsync(
        VerifyReceptionRequest request,
        CancellationToken cancellationToken = default);
}

