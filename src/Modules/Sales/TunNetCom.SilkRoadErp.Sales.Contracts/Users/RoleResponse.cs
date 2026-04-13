namespace TunNetCom.SilkRoadErp.Sales.Contracts.Users;

public record RoleResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

