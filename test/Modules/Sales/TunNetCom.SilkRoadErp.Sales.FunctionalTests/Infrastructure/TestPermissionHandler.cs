using Microsoft.AspNetCore.Authorization;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Authorization;

namespace TunNetCom.SilkRoadErp.Sales.FunctionalTests.Infrastructure;

public class TestPermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
