using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdQueryHandler(
    SalesContext _context,
    ILogger<GetReceiptNoteByIdQueryHandler> _logger)
    : IRequestHandler<GetReceiptNoteByIdQuery, Result<ReceiptNoteResponse>>
{
    public async Task<Result<ReceiptNoteResponse>> Handle(GetReceiptNoteByIdQuery getReceiptNoteByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        
        // Search by Num (receipt note number), not by Id, and load lines directly like GetDeliveryNoteByNum
        var receiptNoteResponse = await _context.BonDeReception
            .Select(br => new ReceiptNoteResponse
            {
                Num = br.Num,
                NumBonFournisseur = br.NumBonFournisseur,
                DateLivraison = br.DateLivraison,
                IdFournisseur = br.IdFournisseur,
                NomFournisseur = br.IdFournisseurNavigation.Nom,
                Date = br.Date,
                NumFactureFournisseur = br.NumFactureFournisseur,
                Items = br.LigneBonReception.Select(l => new ReceiptNoteDetailResponse
                {
                    Id = l.IdLigne,
                    ProductReference = l.RefProduit,
                    Description = l.DesignationLi,
                    Quantity = l.QteLi,
                    UnitPriceExcludingTax = l.PrixHt,
                    DiscountPercentage = l.Remise,
                    TotalExcludingTax = l.TotHt,
                    VatPercentage = l.Tva,
                    TotalIncludingTax = l.TotTtc,
                    Provider = br.IdFournisseurNavigation.Nom,
                    Date = br.Date
                }).ToList()
            })
            .FirstOrDefaultAsync(b => b.Num == getReceiptNoteByIdQuery.Num, cancellationToken);
            
        if (receiptNoteResponse is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);

            return Result.Fail(EntityNotFound.Error());
        }
        _logger.LogEntityFetchedById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        return receiptNoteResponse;
    }
}
