using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBasedOnProductReference;

public class GetDeliveryNotesBasedOnProductReferenceHandler(
    SalesContext _context,
    ILogger<GetDeliveryNotesBasedOnProductReferenceHandler> _logger)
    : IRequestHandler<GetDeliveryNotesBasedOnProductReferenceQuery, PagedList<DeliveryNoteDetailResponse>>
{
    public async Task<PagedList<DeliveryNoteDetailResponse>> Handle(GetDeliveryNotesBasedOnProductReferenceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductReference))
        {
            return new PagedList<DeliveryNoteDetailResponse>(new List<DeliveryNoteDetailResponse>(), 0, request.PageNumber, request.PageSize);
        }

        // Query LigneBl (delivery note lines) for sales history
        var deliveryNotesQuery = _context.LigneBl
            .Include(ligne => ligne.NumBlNavigation)
                .ThenInclude(bon => bon.Client)
            .Where(ligne => ligne.RefProduit != null &&
                           ligne.RefProduit.ToLower() == request.ProductReference.ToLower() &&
                           ligne.NumBlNavigation != null)
            .OrderByDescending(ligne => ligne.NumBlNavigation.Date)
            .Select(ligne => new
            {
                ligne,
                NetTtcUnitaire = CalculateNetTtcUnitaire(ligne),
                PrixHtFodec = (decimal?)null // Fodec is not applicable for sales (delivery notes)
            })
            .Select(x => new DeliveryNoteDetailResponse
            {
                Id = x.ligne.IdLi,
                ProductReference = x.ligne.RefProduit,
                Description = x.ligne.DesignationLi,
                Quantity = x.ligne.QteLi,
                UnitPriceExcludingTax = x.ligne.PrixHt,
                DiscountPercentage = x.ligne.Remise,
                VatPercentage = x.ligne.Tva,
                TotalExcludingTax = x.ligne.TotHt,
                TotalIncludingTax = x.ligne.TotTtc,
                Provider = x.ligne.NumBlNavigation.Client != null ? x.ligne.NumBlNavigation.Client.Nom : string.Empty,
                Date = x.ligne.NumBlNavigation.Date,
                NetTtcUnitaire = x.NetTtcUnitaire,
                PrixHtFodec = x.PrixHtFodec
            });

        var pagedDeliveryNotes = await PagedList<DeliveryNoteDetailResponse>.ToPagedListAsync(
            deliveryNotesQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return pagedDeliveryNotes;
    }

    private static decimal CalculateNetTtcUnitaire(LigneBl ligne)
    {
        var valeurRemise = DecimalHelper.RoundAmount(ligne.PrixHt * (decimal)ligne.Remise / 100);
        var totTmp = DecimalHelper.RoundAmount(ligne.PrixHt - valeurRemise);
        return DecimalHelper.RoundAmount(totTmp + (totTmp * (decimal)ligne.Tva / 100));
    }
}