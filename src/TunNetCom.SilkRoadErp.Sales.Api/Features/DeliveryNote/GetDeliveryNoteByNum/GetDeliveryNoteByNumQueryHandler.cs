using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;

public class GetDeliveryNoteByNumQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteByNumQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNoteByNumQuery, Result<DeliveryNoteResponse>>
{
    public async Task<Result<DeliveryNoteResponse>> Handle(GetDeliveryNoteByNumQuery getDeliveryNoteByNumQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

        var deliveryNoteResponse = await _context.BonDeLivraison
            .Select(LigneBl => new DeliveryNoteResponse
            {
                Num = LigneBl.Num,
                Date = LigneBl.Date,
                ClientId = LigneBl.ClientId,
                TotTva = LigneBl.TotTva,
                NetPayer = LigneBl.NetPayer,
                NumFacture = LigneBl.NumFacture,
                TempBl = LigneBl.TempBl,
                TotHT = LigneBl.TotHTva,
                Lignes = LigneBl.LigneBl.Select(l => new DeliveryNoteDetailResponse
                {
                    RefProduit = l.RefProduit,
                    DesignationLi = l.DesignationLi,
                    QteLi = l.QteLi,
                    PrixHt = l.PrixHt,
                    Remise = l.Remise,
                    Tva = l.Tva,
                    TotHt = l.TotHt,
                    TotTtc = l.TotTtc
                }).ToList(),
            })
            .FirstOrDefaultAsync(d => d.Num == getDeliveryNoteByNumQuery.Num, cancellationToken);

        if (deliveryNoteResponse is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(nameof(BonDeLivraison), getDeliveryNoteByNumQuery.Num);

        return deliveryNoteResponse;
    }
}
