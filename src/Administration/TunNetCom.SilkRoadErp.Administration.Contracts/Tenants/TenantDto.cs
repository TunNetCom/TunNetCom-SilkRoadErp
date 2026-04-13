using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Administration.Contracts.Tenants;

public record TenantDto(
    string Id,
    string Identifier,
    string Name,
    TenancyStrategy Strategy,
    string ConnectionString,
    string? SchemaName,
    bool IsActive,
    int Status,
    IReadOnlyList<string> EnabledBoundedContextKeys,
    IReadOnlyList<string> EnabledFeatureKeys);

public record TenantSummaryDto(
    string Id,
    string Identifier,
    string Name,
    int Status,
    DateTime CreatedAt);

public record CreateTenantDto(
    string Identifier,
    string CompanyName,
    string AdminName,
    string AdminEmail,
    string AdminPassword,
    int? PlanId,
    string? Currency,
    string? Language,
    IReadOnlyList<int>? BoundedContextIds);

public record UpdateTenantDto(
    string Name,
    string? ConnectionString,
    string? SchemaName);
