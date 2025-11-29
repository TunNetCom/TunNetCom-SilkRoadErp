namespace TunNetCom.SilkRoadErp.Sales.Contracts.Users;

public record ChangePasswordRequest
{
    public string NewPassword { get; init; } = string.Empty;
}

