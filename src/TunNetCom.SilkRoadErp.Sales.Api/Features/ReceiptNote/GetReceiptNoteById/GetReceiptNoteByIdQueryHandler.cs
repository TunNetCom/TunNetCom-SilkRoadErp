using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdQueryHandler(
    SalesContext _context,
    ILogger<GetReceiptNoteByIdQueryHandler> _logger,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetReceiptNoteByIdQuery, Result<ReceiptNoteResponse>>
{
    public async Task<Result<ReceiptNoteResponse>> Handle(GetReceiptNoteByIdQuery getReceiptNoteByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        
        // Get FODEC rate from financial parameters service
        var fodecRate = await _financialParametersService.GetPourcentageFodecAsync(0, cancellationToken);
        
        // Search by Num (receipt note number), not by Id, and load lines directly like GetDeliveryNoteByNum
        // Include navigation properties for provider to check Constructeur
        var receiptNoteResponse = await _context.BonDeReception
            .Include(br => br.IdFournisseurNavigation)
            .Include(br => br.LigneBonReception)
            .Where(b => b.Num == getReceiptNoteByIdQuery.Num)
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
                    // Calculate FODEC if provider is constructor
                    PrixHtFodec = br.IdFournisseurNavigation.Constructeur && l.TotHt > 0
                        ? l.TotHt * (fodecRate / 100)
                        : (decimal?)null,
                    // Add FODEC to TotalIncludingTax if applicable
                    TotalIncludingTax = l.TotTtc + (br.IdFournisseurNavigation.Constructeur && l.TotHt > 0
                        ? l.TotHt * (fodecRate / 100)
                        : 0),
                    Provider = br.IdFournisseurNavigation.Nom,
                    Date = br.Date
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
            
        if (receiptNoteResponse is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);

            return Result.Fail(EntityNotFound.Error());
        }
        _logger.LogEntityFetchedById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        return receiptNoteResponse;
    }
}
