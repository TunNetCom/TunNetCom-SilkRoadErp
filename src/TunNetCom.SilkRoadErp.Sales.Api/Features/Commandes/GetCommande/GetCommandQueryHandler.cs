using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;

public class GetFullOrderQueryHandler(
    SalesContext _context,
    ILogger<GetFullOrderQueryHandler> _logger)
    : IRequestHandler<GetFullOrderQuery, Result<FullOrderResponse>>
{
    public async Task<Result<FullOrderResponse>> Handle(
        GetFullOrderQuery query,
        CancellationToken cancellationToken)
    {
        FullOrderResponse? order = await GetFullOrderByIdAsync(query, cancellationToken);

        if (order is null)
        {
            _logger.LogEntityNotFound(
                nameof(Commandes),
                query.Id);
            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(
            nameof(Commandes),
                query.Id);

        return order;
    }

    private async Task<FullOrderResponse?> GetFullOrderByIdAsync(
        GetFullOrderQuery query,
        CancellationToken cancellationToken)
    {
        var order = await _context.Commandes
            .AsNoTracking()
            .Where(c => c.Num == query.Id)
            .Include(c => c.Fournisseur)
            .Include(c => c.LigneCommandes)
            .Select(c => new FullOrderResponse
            {
                OrderNumber = c.Num,
                Date = c.Date,
                SupplierId = c.FournisseurId,
                Supplier = c.Fournisseur != null ? new SupplierInfos
                {
                    Id = c.Fournisseur.Id,
                    Name = c.Fournisseur.Nom,
                    Phone = c.Fournisseur.Tel,
                    Address = c.Fournisseur.Adresse,
                    RegistrationNumber = c.Fournisseur.Matricule,
                    Code = c.Fournisseur.Code,
                    CategoryCode = c.Fournisseur.CodeCat,
                    SecondaryEstablishment = c.Fournisseur.EtbSec,
                    Mail = c.Fournisseur.Mail
                } : null!,
                OrderLines = c.LigneCommandes.Select(lc => new OrderLine
                {
                    LineId = lc.IdLi,
                    ProductReference = lc.RefProduit,
                    ItemDescription = lc.DesignationLi,
                    ItemQuantity = lc.QteLi,
                    UnitPriceExcludingTax = lc.PrixHt,
                    Discount = (decimal)lc.Remise,
                    TotalExcludingTax = lc.TotHt,
                    VatRate = (decimal)lc.Tva,
                    TotalIncludingTax = lc.TotTtc
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order != null)
        {
            order.TotalExcludingVat = order.OrderLines.Sum(lc => lc.TotalExcludingTax);
            order.NetToPay = order.OrderLines.Sum(lc => lc.TotalIncludingTax);
            order.TotalVat = order.NetToPay - order.TotalExcludingVat;
        }

        return order;
    }
}

public record GetFullOrderQuery(int Id) : IRequest<Result<FullOrderResponse>>;


