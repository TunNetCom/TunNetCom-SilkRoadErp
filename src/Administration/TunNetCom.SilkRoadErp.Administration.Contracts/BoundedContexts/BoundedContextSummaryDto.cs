namespace TunNetCom.SilkRoadErp.Administration.Contracts.BoundedContexts;

public record BoundedContextSummaryDto(
    int Id,
    string Key,
    string Name,
    string? Description,
    string? Icon,
    bool IsCore,
    int DisplayOrder,
    int FeaturesCount);

public record BoundedContextDetailDto(
    int Id,
    string Key,
    string Name,
    string? Description,
    string? Icon,
    bool IsCore,
    int DisplayOrder,
    IReadOnlyList<FeatureSummaryDto> Features);

public record CreateBoundedContextDto(
    string Key,
    string Name,
    string? Description,
    string? Icon,
    bool IsCore,
    int DisplayOrder);

public record UpdateBoundedContextDto(
    string Name,
    string? Description,
    string? Icon,
    bool IsCore,
    int DisplayOrder);

public record FeatureSummaryDto(
    int Id,
    string Key,
    string Name,
    string? Description,
    bool IsCore);

public record CreateFeatureDto(
    string Key,
    string Name,
    string? Description,
    bool IsCore);

public record UpdateFeatureDto(
    string Name,
    string? Description,
    bool IsCore);
