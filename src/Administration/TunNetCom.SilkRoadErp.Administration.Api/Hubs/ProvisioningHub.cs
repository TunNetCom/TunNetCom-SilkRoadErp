using Microsoft.AspNetCore.SignalR;

namespace TunNetCom.SilkRoadErp.Administration.Api.Hubs;

public interface IProvisioningClient
{
    Task OnStepStarted(string stepId, string stepName, int stepNumber, int totalSteps);
    Task OnStepCompleted(string stepId, bool success, string? message);
    Task OnProgress(int percentComplete, string currentActivity);
    Task OnProvisioningCompleted(string tenantId, string tenantUrl);
    Task OnProvisioningFailed(string error);
}

public sealed class ProvisioningHub : Hub<IProvisioningClient>
{
    public async Task JoinProvisioningGroup(string provisioningJobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, provisioningJobId);
    }
}
