using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNotesBasedOnProductReference;

public class GetReceiptNotesBasedOnProductReferenceHandler(
    SalesContext _context,
    ILogger<GetReceiptNotesBasedOnProductReferenceHandler> _logger)
    : IRequestHandler<GetReceiptNotesBasedOnProductReferenceQuery, PagedList<ReceiptNoteDetailResponse>>
{
    public async Task<PagedList<ReceiptNoteDetailResponse>> Handle(GetReceiptNotesBasedOnProductReferenceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductReference))
        {
            return new PagedList<ReceiptNoteDetailResponse>(new List<ReceiptNoteDetailResponse>(), 0, request.PageNumber, request.PageSize);
        }

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

