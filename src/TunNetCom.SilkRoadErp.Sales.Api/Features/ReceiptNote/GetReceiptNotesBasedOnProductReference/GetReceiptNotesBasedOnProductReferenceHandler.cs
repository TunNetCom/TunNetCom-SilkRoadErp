using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNotesBasedOnProductReference;

public class GetReceiptNotesBasedOnProductReferenceHandler(
    SalesContext _context,
    ILogger<GetReceiptNotesBasedOnProductReferenceHandler> _logger,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetReceiptNotesBasedOnProductReferenceQuery, PagedList<ReceiptNoteDetailResponse>>
{
    public async Task<PagedList<ReceiptNoteDetailResponse>> Handle(GetReceiptNotesBasedOnProductReferenceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductReference))
        {
            return new PagedList<ReceiptNoteDetailResponse>(new List<ReceiptNoteDetailResponse>(), 0, request.PageNumber, request.PageSize);
        }

        var fodecPercentage = await _financialParametersService.GetPourcentageFodecAsync(0, cancellationToken);

        var receiptNotesQuery = _context.LigneBonReception
            .Include(ligne => ligne.NumBonRecNavigation)
                .ThenInclude(bon => bon.IdFournisseurNavigation)
            .Where(ligne => ligne.RefProduit != null &&
                           ligne.RefProduit.ToLower() == request.ProductReference.ToLower() &&
                           ligne.NumBonRecNavigation != null &&
                           ligne.NumBonRecNavigation.IdFournisseurNavigation != null)
            .OrderByDescending(ligne => ligne.NumBonRecNavigation.Date)
            .Select(ligne => new ReceiptNoteDetailResponse
            {
                Id = ligne.IdLigne,
                ProductReference = ligne.RefProduit,
                Description = ligne.DesignationLi,
                Quantity = ligne.QteLi,
                UnitPriceExcludingTax = ligne.PrixHt,
                DiscountPercentage = ligne.Remise,
                VatPercentage = ligne.Tva,
                TotalExcludingTax = ligne.TotHt,
                TotalIncludingTax = ligne.TotTtc,
                Provider = ligne.NumBonRecNavigation.IdFournisseurNavigation.Nom,
                FournisseurId = ligne.NumBonRecNavigation.IdFournisseur,
                Constructeur = ligne.NumBonRecNavigation.IdFournisseurNavigation.Constructeur,
                FodecPercentage = fodecPercentage,
                PrixHtFodec = ligne.NumBonRecNavigation.IdFournisseurNavigation.Constructeur && ligne.TotHt > 0
                    ? ligne.TotHt * (fodecPercentage / 100)
                    : (decimal?)null,
                Date = ligne.NumBonRecNavigation.Date
            });

        var pagedReceiptNotes = await PagedList<ReceiptNoteDetailResponse>.ToPagedListAsync(
            receiptNotesQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return pagedReceiptNotes;
    }
}

