using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.CompteBancaire;

public interface ICompteBancaireApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateCompteBancaireAsync(
        CreateCompteBancaireRequest request,
        CancellationToken cancellationToken);

    Task<List<CompteBancaireResponse>> GetCompteBancairesAsync(
        CancellationToken cancellationToken);

    Task<CompteBancaireResponse?> GetCompteBancaireByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<bool, BadRequestResponse>> UpdateCompteBancaireAsync(
        int id,
        UpdateCompteBancaireRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<bool, BadRequestResponse>> DeleteCompteBancaireAsync(
        int id,
        CancellationToken cancellationToken);
}
