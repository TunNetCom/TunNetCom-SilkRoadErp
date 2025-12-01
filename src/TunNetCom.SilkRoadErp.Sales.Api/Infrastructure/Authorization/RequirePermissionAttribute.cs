namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Authorization;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
        : base($"Permission:{permission}")
    {
        Permission = permission;
    }

    public string Permission { get; }
}

