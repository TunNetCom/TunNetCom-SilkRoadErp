using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdQueryHandler(
    SalesContext _context,
    ILogger<GetReceiptNoteByIdQueryHandler> _logger,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetReceiptNoteByIdQuery, Result<ReceiptNoteResponse>>
{
    public async Task<Result<ReceiptNoteResponse>> Handle(GetReceiptNoteByIdQuery getReceiptNoteByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        
        // Get active accounting year ID
        var activeAccountingYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        
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
                AccountingYearId = br.AccountingYearId,
                Items = br.LigneBonReception.Select(l => new ReceiptNoteDetailResponse
                {
                    Id = l.IdLigne,
                    ProductReference = l.RefProduit,
                    Description = l.DesignationLi,
                    FournisseurId = br.IdFournisseur,
                    Constructeur = br.IdFournisseurNavigation.Constructeur,
                    FodecPercentage = fodecRate,
                    Quantity = l.QteLi,
                    UnitPriceExcludingTax = l.PrixHt,
                    DiscountPercentage = l.Remise,
                    TotalExcludingTax = l.TotHt,
                    VatPercentage = l.Tva,
                    // Always calculate FODEC if provider is constructor
                    // Uses same logic as ReceiptNoteFodecCalculator.CalculateFodecAndTtc (cannot use service directly in LINQ)
                    PrixHtFodec = br.IdFournisseurNavigation.Constructeur && l.TotHt > 0
                        ? l.TotHt * (fodecRate / 100)
                        : (decimal?)null,
                    // Recalculate TTC with correct formula: HT + FODEC + TVA (where TVA is calculated on HT + FODEC)
                    // Formula matches ReceiptNoteFodecCalculator.CalculateFodecAndTtc
                    TotalIncludingTax = br.IdFournisseurNavigation.Constructeur && l.TotHt > 0
                        ? l.TotHt + (l.TotHt * (fodecRate / 100)) + ((l.TotHt + (l.TotHt * (fodecRate / 100))) * (decimal)(l.Tva / 100))
                        : l.TotTtc,
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
