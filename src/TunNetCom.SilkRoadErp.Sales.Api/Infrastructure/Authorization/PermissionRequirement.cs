using Microsoft.AspNetCore.Authorization;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

