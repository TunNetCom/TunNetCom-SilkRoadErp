using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFinancierFournisseurs;

public interface IAvoirFinancierFournisseursApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateAvoirFinancierFournisseurs(
        CreateAvoirFinancierFournisseursRequest request,
        CancellationToken cancellationToken);

    Task<Result<AvoirFinancierFournisseursResponse>> GetAvoirFinancierFournisseursAsync(
        int num,
        CancellationToken cancellationToken);

    Task<Result<FullAvoirFinancierFournisseursResponse>> GetFullAvoirFinancierFournisseursAsync(
        int num,
        CancellationToken cancellationToken);

    Task<GetAvoirFinancierFournisseursWithSummariesResponse> GetAvoirFinancierFournisseursWithSummariesAsync(
        int? providerId,
        int? numFactureFournisseur,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken);

    Task<Result> UpdateAvoirFinancierFournisseursAsync(
        int num,
        UpdateAvoirFinancierFournisseursRequest request,
        CancellationToken cancellationToken);

    Task<Result> ValidateAvoirFinancierFournisseursAsync(List<int> ids, CancellationToken cancellationToken);
}

