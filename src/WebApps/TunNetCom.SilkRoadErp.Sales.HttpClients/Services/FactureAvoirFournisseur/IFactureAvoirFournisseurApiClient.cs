using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.FactureAvoirFournisseur;

public interface IFactureAvoirFournisseurApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateFactureAvoirFournisseur(
        CreateFactureAvoirFournisseurRequest request,
        CancellationToken cancellationToken);

    Task<Result<FactureAvoirFournisseurResponse>> GetFactureAvoirFournisseurAsync(
        int num,
        CancellationToken cancellationToken);

    Task<Result<FullFactureAvoirFournisseurResponse>> GetFullFactureAvoirFournisseurAsync(
        int num,
        CancellationToken cancellationToken);

    Task<GetFactureAvoirFournisseurWithSummariesResponse> GetFactureAvoirFournisseurWithSummariesAsync(
        int? idFournisseur,
        int? numFactureFournisseur,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken);

    Task<Result> UpdateFactureAvoirFournisseurAsync(
        int num,
        UpdateFactureAvoirFournisseurRequest request,
        CancellationToken cancellationToken);
}

