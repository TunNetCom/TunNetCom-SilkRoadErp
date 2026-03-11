using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

public class UnpaidClientNotificationVerifier : INotificationVerifier
{
    private readonly SalesContext _context;
    private readonly ILogger<UnpaidClientNotificationVerifier> _logger;
    private readonly IActiveAccountingYearService _activeAccountingYearService;
    private readonly ISoldeClientCalculationService _soldeClientCalculationService;

    public UnpaidClientNotificationVerifier(
        SalesContext context,
        ILogger<UnpaidClientNotificationVerifier> logger,
        IActiveAccountingYearService activeAccountingYearService,
        ISoldeClientCalculationService soldeClientCalculationService)
    {
        _context = context;
        _logger = logger;
        _activeAccountingYearService = activeAccountingYearService;
        _soldeClientCalculationService = soldeClientCalculationService;
    }

    public async Task<List<NotificationData>> VerifyAsync(CancellationToken cancellationToken)
    {
        var notifications = new List<NotificationData>();

        try
        {
            var accountingYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
            if (!accountingYearId.HasValue)
            {
                _logger.LogWarning("No active accounting year found, skipping unpaid client verification");
                return notifications;
            }

            var soldes = await _soldeClientCalculationService.GetSoldesClientsForAccountingYearAsync(accountingYearId.Value, cancellationToken);
            var clientIds = soldes.Select(x => x.ClientId).ToList();

            var quantitesParClient = new Dictionary<int, int>();
            if (clientIds.Any())
            {
                var quantitesNonLivrees = await _context.BonDeLivraison
                    .Where(b => b.ClientId.HasValue && clientIds.Contains(b.ClientId.Value) && b.AccountingYearId == accountingYearId.Value)
                    .Include(b => b.LigneBl)
                    .ToListAsync(cancellationToken);

                quantitesParClient = quantitesNonLivrees
                    .Where(b => b.ClientId.HasValue)
                    .GroupBy(b => b.ClientId!.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => g.SelectMany(b => b.LigneBl)
                            .Select(l => l.QteLi - (l.QteLivree ?? l.QteLi))
                            .Where(q => q > 0)
                            .DefaultIfEmpty(0)
                            .Sum()
                    );
            }

            foreach (var item in soldes)
            {
                var nombreQuantitesNonLivrees = quantitesParClient.GetValueOrDefault(item.ClientId, 0);
                if (item.Solde < 0 || nombreQuantitesNonLivrees > 0)
                {
                    var title = item.Solde < 0
                        ? $"Client non réglé: {item.ClientNom}"
                        : $"Quantités non livrées: {item.ClientNom}";

                    var message = item.Solde < 0
                        ? $"Le client {item.ClientNom} a un solde impayé de {Math.Abs(item.Solde):F2} TND."
                        : $"Le client {item.ClientNom} a {nombreQuantitesNonLivrees} quantités non livrées.";

                    notifications.Add(new NotificationData
                    {
                        Type = NotificationType.UnpaidClient,
                        Title = title,
                        Message = message,
                        RelatedEntityId = item.ClientId,
                        RelatedEntityType = "Client"
                    });
                }
            }

            _logger.LogInformation("UnpaidClientNotificationVerifier found {Count} clients with problems", notifications.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying unpaid clients");
        }

        return notifications;
    }
}
