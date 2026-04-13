namespace TunNetCom.SilkRoadErp.Administration.Contracts.Plans;

public record PlanDto(
    int Id,
    string Name,
    string? Description,
    int MaxUsers,
    long MaxStorageBytes,
    decimal MonthlyPrice,
    decimal YearlyPrice,
    int ApiRateLimitPerMinute,
    int TrialDays,
    bool IsActive,
    IReadOnlyList<PlanBoundedContextDto> BoundedContexts);

public record PlanBoundedContextDto(
    string BoundedContextKey,
    string BoundedContextName,
    bool IncludesAllFeatures,
    IReadOnlyList<string> FeatureKeys);

public record CreatePlanDto(
    string Name,
    string? Description,
    int MaxUsers,
    long MaxStorageBytes,
    decimal MonthlyPrice,
    decimal YearlyPrice,
    int ApiRateLimitPerMinute,
    int TrialDays,
    bool IsActive);

public record UpdatePlanDto(
    string Name,
    string? Description,
    int MaxUsers,
    long MaxStorageBytes,
    decimal MonthlyPrice,
    decimal YearlyPrice,
    int ApiRateLimitPerMinute,
    int TrialDays,
    bool IsActive);
