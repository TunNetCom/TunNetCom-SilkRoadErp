namespace TunNetCom.SilkRoadErp.Sales.Contracts.Users;

public record UpdateUserRequest
{
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public bool? IsActive { get; init; }
    public List<int>? RoleIds { get; init; }
}

