using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBasedOnProductReference;

public class GetDeliveryNotesBasedOnProductReferenceHandler(
    SalesContext _context,
    ILogger<GetDeliveryNotesBasedOnProductReferenceHandler> _logger)
    : IRequestHandler<GetDeliveryNotesBasedOnProductReferenceQuery, List<DeliveryNoteDetailResponse>>
{
    public async Task<List<DeliveryNoteDetailResponse>> Handle(GetDeliveryNotesBasedOnProductReferenceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductReference))
        {
            return new List<DeliveryNoteDetailResponse>(); // Or throw an exception, depending on your logic
        }

        // Use ToLower for case-insensitive comparison (database-friendly)
        var deliveryNotes = await _context.LigneBonReception
    .Include(ligne => ligne.NumBonRecNavigation)
        .ThenInclude(bon => bon.IdFournisseurNavigation)
    .Where(ligne => ligne.RefProduit != null &&
                   ligne.RefProduit.ToLower() == request.ProductReference.ToLower() &&
                   ligne.NumBonRecNavigation != null &&
                   ligne.NumBonRecNavigation.IdFournisseurNavigation != null)
    .OrderByDescending(ligne => ligne.NumBonRecNavigation.Date)
    .Select(ligne => new
    {
        ligne,
        NetTtcUnitaire = CalculateNetTtcUnitaire(ligne),
        PrixHtFodec = ligne.NumBonRecNavigation.IdFournisseurNavigation.Constructeur ? ligne.PrixHt * 1.01m : (decimal?)null
    })
    .Select(x => new DeliveryNoteDetailResponse
    {
        ProductReference = x.ligne.RefProduit,
        Description = x.ligne.DesignationLi,
        Quantity = x.ligne.QteLi,
        UnitPriceExcludingTax = x.ligne.PrixHt,
        DiscountPercentage = x.ligne.Remise,
        VatPercentage = x.ligne.Tva,
        TotalExcludingTax = x.ligne.TotHt,
        TotalIncludingTax = x.ligne.TotTtc,
        Provider = x.ligne.NumBonRecNavigation.IdFournisseurNavigation.Nom,
        Date = x.ligne.NumBonRecNavigation.Date,
        NetTtcUnitaire = x.NetTtcUnitaire,
        PrixHtFodec = x.PrixHtFodec
    })
    .ToListAsync(cancellationToken); 

        return deliveryNotes;
    }

    private static decimal CalculateNetTtcUnitaire(LigneBonReception ligne)
    {
        var valeurRemise = ligne.PrixHt * (decimal)ligne.Remise / 100;
        var totTmp = ligne.PrixHt - valeurRemise;
        if (ligne.NumBonRecNavigation?.IdFournisseurNavigation?.Constructeur == true)
        {
            var fodec = ligne.PrixHt * 1.01m; // prix_HT + (prix_HT / 100)
            valeurRemise = fodec * (decimal)ligne.Remise / 100;
            totTmp = fodec - valeurRemise;
        }
        return Math.Round(totTmp + (totTmp * (decimal)ligne.Tva / 100));
    }
}