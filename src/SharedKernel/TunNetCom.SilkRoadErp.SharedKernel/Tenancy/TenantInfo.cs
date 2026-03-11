namespace TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

public sealed record TenantInfo
{
    public required string Id { get; init; }
    public required string Identifier { get; init; }
    public required string Name { get; init; }
    public required TenancyStrategy Strategy { get; init; }
    public required string ConnectionString { get; init; }
    public string? SchemaName { get; init; }
    public bool IsActive { get; init; } = true;
    public Dictionary<string, string> Metadata { get; init; } = [];
}
