using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementFournisseur;

public interface IPaiementFournisseurApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreatePaiementFournisseurAsync(
        CreatePaiementFournisseurRequest request,
        CancellationToken cancellationToken);

    Task<Result<PaiementFournisseurResponse>> GetPaiementFournisseurAsync(
        int id,
        CancellationToken cancellationToken);

    Task<PagedList<PaiementFournisseurResponse>> GetPaiementsFournisseurAsync(
        int? fournisseurId,
        int? accountingYearId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Result> UpdatePaiementFournisseurAsync(
        int id,
        UpdatePaiementFournisseurRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeletePaiementFournisseurAsync(
        int id,
        CancellationToken cancellationToken);
}








