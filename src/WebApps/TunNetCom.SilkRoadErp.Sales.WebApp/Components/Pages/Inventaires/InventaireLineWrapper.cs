using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;
using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.Inventaires;

public class InventaireLineWrapper : ILineItem
{
    public string ProductReference { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Id { get; set; }
    public int InventaireId { get; set; }

    // Propriétés spécifiques inventaire
    public int QuantiteTheorique { get; set; }
    public decimal DernierPrixAchat { get; set; }

    // ILineItem properties - mapping vers les propriétés inventaire
    public int Quantity
    {
        get => QuantiteReelle;
        set => QuantiteReelle = value;
    }

    public decimal UnitPriceExcludingTax
    {
        get => PrixHt;
        set => PrixHt = value;
    }

    // Remise pour l'inventaire (utilisée pour le calcul des totaux mais non sauvegardée en base)
    public double DiscountPercentage
    {
        get => _discountPercentage;
        set => _discountPercentage = value;
    }

    // Pour l'inventaire, on n'utilise pas de TVA, toujours 0
    public double VatPercentage
    {
        get => 0;
        set { } // Ignore les modifications
    }

    public decimal TotalExcludingTax { get; set; }

    // Pour l'inventaire, TotalIncludingTax = TotalExcludingTax (pas de TVA)
    public decimal TotalIncludingTax
    {
        get => TotalExcludingTax;
        set => TotalExcludingTax = value;
    }

    // Propriétés internes pour le mapping
    private int QuantiteReelle { get; set; }
    private decimal PrixHt { get; set; }
    private double _discountPercentage = 0; // Stockage de la remise

    public string ProductReferenceAndDescription => $"{ProductReference} - {Description ?? string.Empty}";

    public CreateLigneInventaireRequest ToCreateRequest()
    {
        return new CreateLigneInventaireRequest
        {
            RefProduit = ProductReference,
            QuantiteReelle = QuantiteReelle,
            PrixHt = PrixHt,
            DernierPrixAchat = DernierPrixAchat
        };
    }

    public UpdateLigneInventaireRequest ToUpdateRequest()
    {
        return new UpdateLigneInventaireRequest
        {
            Id = Id > 0 ? Id : null,
            RefProduit = ProductReference,
            QuantiteReelle = QuantiteReelle,
            PrixHt = PrixHt,
            DernierPrixAchat = DernierPrixAchat
        };
    }

    public static InventaireLineWrapper FromResponse(LigneInventaireResponse response)
    {
        return new InventaireLineWrapper
        {
            Id = response.Id,
            InventaireId = response.InventaireId,
            ProductReference = response.RefProduit,
            Description = response.NomProduit,
            QuantiteTheorique = response.QuantiteTheorique,
            QuantiteReelle = response.QuantiteReelle,
            PrixHt = response.PrixHt,
            DernierPrixAchat = response.DernierPrixAchat,
            TotalExcludingTax = response.QuantiteReelle * response.PrixHt
        };
    }
}

