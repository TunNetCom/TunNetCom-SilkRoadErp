using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class NotificationCheckJob : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationCheckJob> _logger;

    public NotificationCheckJob(
        IServiceProvider serviceProvider,
        ILogger<NotificationCheckJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("NotificationCheckJob started at {Time}", DateTime.UtcNow);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var verifiers = scope.ServiceProvider.GetServices<INotificationVerifier>();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

            var allNotifications = new List<NotificationData>();

            foreach (var verifier in verifiers)
            {
                try
                {
                    var notifications = await verifier.VerifyAsync(context.CancellationToken);
                    allNotifications.AddRange(notifications);
                    _logger.LogInformation("Verifier {VerifierType} found {Count} notifications", 
                        verifier.GetType().Name, notifications.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in verifier {VerifierType}", verifier.GetType().Name);
                }
            }

            if (allNotifications.Any())
            {
                await notificationService.CreateNotificationsAsync(allNotifications, context.CancellationToken);
                _logger.LogInformation("NotificationCheckJob created {Count} notifications", allNotifications.Count);
            }
            else
            {
                _logger.LogInformation("NotificationCheckJob found no new notifications");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing NotificationCheckJob");
            throw;
        }

        _logger.LogInformation("NotificationCheckJob completed at {Time}", DateTime.UtcNow);
    }
}

