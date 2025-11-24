using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;

public class GetDeliveryNoteByNumQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteByNumQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteByNumQuery, Result<DeliveryNoteResponse>>
{
    public async Task<Result<DeliveryNoteResponse>> Handle(
        GetDeliveryNoteByNumQuery getDeliveryNoteByNumQuery,
        CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

        var deliveryNoteResponse = await _context.BonDeLivraison
            .Select(LigneBl => new DeliveryNoteResponse
            {
                DeliveryNoteNumber = LigneBl.Num,
                Date = LigneBl.Date,
                CustomerId = LigneBl.ClientId,
                TotalVat = LigneBl.TotTva,
                TotalAmount = LigneBl.NetPayer,
                InvoiceNumber = LigneBl.NumFacture,
                CreationTime = LigneBl.TempBl,
                TotalExcludingTax = LigneBl.TotHTva,
                Items = LigneBl.LigneBl.Select(l => new DeliveryNoteDetailResponse
                {
                    Id = l.IdLi,
                    ProductReference = l.RefProduit,
                    Description = l.DesignationLi,
                    Quantity = l.QteLi,
                    DeliveredQuantity = l.QteLivree ?? l.QteLi,
                    HasPartialDelivery = l.QteLivree.HasValue && l.QteLivree.Value < l.QteLi,
                    UnitPriceExcludingTax = l.PrixHt,
                    DiscountPercentage = l.Remise,
                    VatPercentage = l.Tva,
                    TotalExcludingTax = l.TotHt,
                    TotalIncludingTax = l.TotTtc
                }).ToList(),
            })
            .FirstOrDefaultAsync(d => d.DeliveryNoteNumber == getDeliveryNoteByNumQuery.Num, cancellationToken);

        if (deliveryNoteResponse is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

        return deliveryNoteResponse;
    }
}
