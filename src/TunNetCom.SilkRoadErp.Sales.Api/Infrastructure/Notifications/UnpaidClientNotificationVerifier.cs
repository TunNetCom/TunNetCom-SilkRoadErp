using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

public class UnpaidClientNotificationVerifier : INotificationVerifier
{
    private readonly SalesContext _context;
    private readonly ILogger<UnpaidClientNotificationVerifier> _logger;
    private readonly IMediator _mediator;
    private readonly IActiveAccountingYearService _activeAccountingYearService;

    public UnpaidClientNotificationVerifier(
        SalesContext context,
        ILogger<UnpaidClientNotificationVerifier> logger,
        IMediator mediator,
        IActiveAccountingYearService activeAccountingYearService)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
        _activeAccountingYearService = activeAccountingYearService;
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

            var appParams = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var timbre = appParams.Value.Timbre;

            // Get all clients with their related data
            var clientsData = await _context.Client
                .AsNoTracking()
                .Select(client => new
                {
                    Client = client,
                    FacturesCount = _context.Facture
                        .Where(f => f.IdClient == client.Id && f.AccountingYearId == accountingYearId.Value)
                        .Count(),
                    TotalFacturesNetPayer = _context.Facture
                        .Where(f => f.IdClient == client.Id && f.AccountingYearId == accountingYearId.Value)
                        .SelectMany(f => f.BonDeLivraison)
                        .Sum(b => (decimal?)b.NetPayer) ?? 0,
                    TotalBonsLivraisonNonFactures = _context.BonDeLivraison
                        .Where(b => b.ClientId == client.Id
                            && b.AccountingYearId == accountingYearId.Value
                            && b.NumFacture == null)
                        .Sum(b => (decimal?)b.NetPayer) ?? 0,
                    TotalAvoirs = _context.Avoirs
                        .Where(a => a.ClientId == client.Id
                            && a.AccountingYearId == accountingYearId.Value
                            && a.NumFactureAvoirClient == null)
                        .SelectMany(a => a.LigneAvoirs)
                        .Sum(l => (decimal?)l.TotTtc) ?? 0,
                    TotalFacturesAvoir = _context.FactureAvoirClient
                        .Where(fa => fa.IdClient == client.Id && fa.AccountingYearId == accountingYearId.Value)
                        .SelectMany(fa => fa.Avoirs)
                        .SelectMany(a => a.LigneAvoirs)
                        .Sum(l => (decimal?)l.TotTtc) ?? 0,
                    TotalPaiements = _context.PaiementClient
                        .Where(p => p.ClientId == client.Id && p.AccountingYearId == accountingYearId.Value)
                        .Sum(p => (decimal?)p.Montant) ?? 0
                })
                .ToListAsync(cancellationToken);

            // Calculate quantités non livrées
            var clientIds = clientsData.Select(x => x.Client.Id).ToList();
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

            // Find clients with problems
            foreach (var clientData in clientsData)
            {
                var totalFactures = clientData.TotalFacturesNetPayer + (timbre * clientData.FacturesCount);
                var solde = clientData.TotalAvoirs + clientData.TotalFacturesAvoir + clientData.TotalPaiements
                    - totalFactures - clientData.TotalBonsLivraisonNonFactures;
                var nombreQuantitesNonLivrees = quantitesParClient.GetValueOrDefault(clientData.Client.Id, 0);

                if (solde < 0 || nombreQuantitesNonLivrees > 0)
                {
                    var title = solde < 0
                        ? $"Client non réglé: {clientData.Client.Nom}"
                        : $"Quantités non livrées: {clientData.Client.Nom}";

                    var message = solde < 0
                        ? $"Le client {clientData.Client.Nom} a un solde impayé de {Math.Abs(solde):F2} TND."
                        : $"Le client {clientData.Client.Nom} a {nombreQuantitesNonLivrees} quantités non livrées.";

                    notifications.Add(new NotificationData
                    {
                        Type = NotificationType.UnpaidClient,
                        Title = title,
                        Message = message,
                        RelatedEntityId = clientData.Client.Id,
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

