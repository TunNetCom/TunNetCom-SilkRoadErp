namespace TunNetCom.SilkRoadErp.Sales.Contracts.Users;

public record CreateUserRequest
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public bool IsActive { get; init; } = true;
    public List<int> RoleIds { get; init; } = new();
}

