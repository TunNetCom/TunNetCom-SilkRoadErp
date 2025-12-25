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

        var deliveryNote = await _context.BonDeLivraison
            .FilterByActiveAccountingYear()
            .Include(b => b.InstallationTechnician)
            .Include(b => b.LigneBl)
            .FirstOrDefaultAsync(d => d.Num == getDeliveryNoteByNumQuery.Num, cancellationToken);

        if (deliveryNote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

        var deliveryNoteResponse = new DeliveryNoteResponse
        {
            Id = deliveryNote.Id,
            DeliveryNoteNumber = deliveryNote.Num,
            Date = deliveryNote.Date,
            CustomerId = deliveryNote.ClientId,
            InstallationTechnicianId = deliveryNote.InstallationTechnicianId,
            InstallationTechnicianName = deliveryNote.InstallationTechnician?.Nom,
            DeliveryCarId = deliveryNote.DeliveryCarId,
            TotalVat = deliveryNote.TotTva,
            TotalAmount = deliveryNote.NetPayer,
            InvoiceNumber = deliveryNote.NumFacture,
            CreationTime = deliveryNote.TempBl,
            TotalExcludingTax = deliveryNote.TotHTva,
            Statut = (int)deliveryNote.Statut,
            StatutLibelle = deliveryNote.Statut.ToString(),
            Items = deliveryNote.LigneBl.Select(l => new DeliveryNoteDetailResponse
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
        };

        return Result.Ok(deliveryNoteResponse);
    }
}
