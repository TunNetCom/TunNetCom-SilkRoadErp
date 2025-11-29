using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Avoirs;

public interface IAvoirsApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateAvoir(
        CreateAvoirRequest request,
        CancellationToken cancellationToken);

    Task<Result<AvoirResponse>> GetAvoirAsync(
        int num,
        CancellationToken cancellationToken);

    Task<Result<FullAvoirResponse>> GetFullAvoirAsync(
        int num,
        CancellationToken cancellationToken);

    Task<GetAvoirsWithSummariesResponse> GetAvoirsWithSummariesAsync(
        int? clientId,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken,
        int? status = null);

    Task<Result> UpdateAvoirAsync(
        int num,
        UpdateAvoirRequest request,
        CancellationToken cancellationToken);

    Task<Result> ValidateAvoirsAsync(List<int> ids, CancellationToken cancellationToken);
}

