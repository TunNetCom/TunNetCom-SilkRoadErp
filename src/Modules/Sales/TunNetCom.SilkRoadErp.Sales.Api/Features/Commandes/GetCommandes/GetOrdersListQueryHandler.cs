using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
public class GetOrdersListQueryHandler(
    SalesContext _context,
    ILogger<GetOrdersListQueryHandler> _logger)
    : IRequestHandler<GetOrdersListQuery, Result<List<OrderSummaryResponse>>>
{
    public async Task<Result<List<OrderSummaryResponse>>> Handle(
        GetOrdersListQuery query,
        CancellationToken cancellationToken)
    {
        var orders = await GetOrdersListAsync(cancellationToken);

        if (!orders.Any())
        {
            _logger.LogInformation("No orders found in the database.");
            return Result.Ok(new List<OrderSummaryResponse>());
        }

        _logger.LogInformation("Retrieved {Count} orders from the database.", orders.Count);
        return Result.Ok(orders);
    }

    private async Task<List<OrderSummaryResponse>> GetOrdersListAsync(
        CancellationToken cancellationToken)
    {
        var query = await _context.Commandes
            .AsNoTracking()
            .Include(c => c.Fournisseur)
            .Include(c => c.LigneCommandes)
            .Select(c => new
            {
                c.Num,
                c.FournisseurId,
                SupplierName = c.Fournisseur != null ? c.Fournisseur.Nom : null,
                c.Date,
                c.Statut,
                TotalExcludingVat = c.LigneCommandes.Sum(lc => lc.TotHt),
                NetToPay = c.LigneCommandes.Sum(lc => lc.TotTtc),
                TotalVat = c.LigneCommandes.Sum(lc => lc.TotTtc) - c.LigneCommandes.Sum(lc => lc.TotHt)
            })
            .ToListAsync(cancellationToken);

        return query.Select(c => new OrderSummaryResponse
        {
            OrderNumber = c.Num,
            SupplierId = c.FournisseurId,
            SupplierName = c.SupplierName,
            Date = c.Date,
            TotalExcludingVat = c.TotalExcludingVat,
            NetToPay = c.NetToPay,
            TotalVat = c.TotalVat,
            Statut = (int)c.Statut
        }).ToList();
    }
}