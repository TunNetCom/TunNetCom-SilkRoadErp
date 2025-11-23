using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFournisseur;

public interface IAvoirFournisseurApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateAvoirFournisseur(
        CreateAvoirFournisseurRequest request,
        CancellationToken cancellationToken);

    Task<Result<AvoirFournisseurResponse>> GetAvoirFournisseurAsync(
        int num,
        CancellationToken cancellationToken);

    Task<Result<FullAvoirFournisseurResponse>> GetFullAvoirFournisseurAsync(
        int num,
        CancellationToken cancellationToken);

    Task<GetAvoirFournisseurWithSummariesResponse> GetAvoirFournisseurWithSummariesAsync(
        int? fournisseurId,
        int? numFactureAvoirFournisseur,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken);

    Task<Result> UpdateAvoirFournisseurAsync(
        int num,
        UpdateAvoirFournisseurRequest request,
        CancellationToken cancellationToken);
}

