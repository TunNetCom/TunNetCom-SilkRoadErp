using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Administration.Api.Hubs;
using TunNetCom.SilkRoadErp.Administration.Domain.Entities;
using TunNetCom.SilkRoadErp.Administration.Domain.Enums;

namespace TunNetCom.SilkRoadErp.Administration.Api.Infrastructure.Provisioning;

public sealed class TenantProvisioningService
{
    private readonly AdminContext _adminContext;
    private readonly IHubContext<ProvisioningHub, IProvisioningClient> _hubContext;
    private readonly ILogger<TenantProvisioningService> _logger;

    public TenantProvisioningService(
        AdminContext adminContext,
        IHubContext<ProvisioningHub, IProvisioningClient> hubContext,
        ILogger<TenantProvisioningService> logger)
    {
        _adminContext = adminContext;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task ProvisionAsync(string tenantId, string connectionId, CancellationToken ct = default)
    {
        var client = _hubContext.Clients.Client(connectionId);
        var tenant = await _adminContext.Tenants.FindAsync([tenantId], ct)
            ?? throw new InvalidOperationException($"Tenant {tenantId} not found");

        var steps = new (string Id, string Name)[]
        {
            ("ValidatingInput", "Validating tenant configuration"),
            ("CreatingTenantRecord", "Registering tenant in catalog"),
            ("ProvisioningDatabase", "Creating database / schema"),
            ("ApplyingMigrations", "Running database migrations"),
            ("ConfiguringCompany", "Setting up company info"),
            ("ConfiguringLocale", "Applying currency, language, locale"),
            ("InstallingModules", "Activating selected ERP modules"),
            ("CreatingAdminUser", "Creating first admin user"),
            ("SeedingSampleData", "Loading demo data"),
            ("Finalizing", "Activating tenant")
        };

        for (int i = 0; i < steps.Length; i++)
        {
            var (stepId, stepName) = steps[i];
            await client.OnStepStarted(stepId, stepName, i + 1, steps.Length);
            await client.OnProgress((i * 100) / steps.Length, stepName);

            try
            {
                await ExecuteStep(stepId, tenant, ct);
                await client.OnStepCompleted(stepId, true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provisioning step {Step} failed for tenant {TenantId}", stepId, tenantId);
                await client.OnStepCompleted(stepId, false, ex.Message);
                await client.OnProvisioningFailed($"Step '{stepName}' failed: {ex.Message}");

                tenant.Status = TenantStatus.Deactivated;
                await _adminContext.SaveChangesAsync(ct);
                return;
            }
        }

        tenant.Status = TenantStatus.Active;
        tenant.UpdatedAt = DateTime.UtcNow;
        await _adminContext.SaveChangesAsync(ct);

        await client.OnProgress(100, "Complete");
        await client.OnProvisioningCompleted(tenantId, $"https://{tenant.Identifier}.erp.example.com");
    }

    private Task ExecuteStep(string stepId, Tenant tenant, CancellationToken ct)
    {
        return stepId switch
        {
            "ValidatingInput" => Task.Delay(500, ct),
            "CreatingTenantRecord" => Task.Delay(300, ct),
            "ProvisioningDatabase" => Task.Delay(2000, ct),
            "ApplyingMigrations" => Task.Delay(3000, ct),
            "ConfiguringCompany" => Task.Delay(500, ct),
            "ConfiguringLocale" => Task.Delay(300, ct),
            "InstallingModules" => Task.Delay(1000, ct),
            "CreatingAdminUser" => Task.Delay(500, ct),
            "SeedingSampleData" => Task.Delay(2000, ct),
            "Finalizing" => Task.Delay(500, ct),
            _ => Task.CompletedTask
        };
    }
}
