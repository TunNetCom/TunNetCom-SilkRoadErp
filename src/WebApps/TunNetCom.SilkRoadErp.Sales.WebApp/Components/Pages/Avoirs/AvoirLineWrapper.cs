using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;
using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.Avoirs;

public class AvoirLineWrapper : ILineItem
{
    public string ProductReference { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Id { get; set; }

    // ILineItem properties
    public int Quantity
    {
        get => QteLi;
        set => QteLi = value;
    }

    public decimal UnitPriceExcludingTax
    {
        get => PrixHt;
        set => PrixHt = value;
    }

    public double DiscountPercentage
    {
        get => Remise;
        set => Remise = value;
    }

    public double VatPercentage
    {
        get => Tva;
        set => Tva = value;
    }

    public decimal TotalExcludingTax
    {
        get => TotHt;
        set => TotHt = value;
    }

    public decimal TotalIncludingTax
    {
        get => TotTtc;
        set => TotTtc = value;
    }

    // AvoirLineRequest properties
    public int QteLi { get; set; }
    public decimal PrixHt { get; set; }
    public double Remise { get; set; }
    public double Tva { get; set; }
    public decimal TotHt { get; set; }
    public decimal TotTtc { get; set; }

    public string ProductReferenceAndDescription => $"{ProductReference} - {Description ?? string.Empty}";

    public AvoirLineRequest ToRequest()
    {
        return new AvoirLineRequest
        {
            RefProduit = ProductReference,
            DesignationLi = Description,
            QteLi = QteLi,
            PrixHt = PrixHt,
            Remise = Remise,
            Tva = Tva
        };
    }

    public static AvoirLineWrapper FromResponse(AvoirLineResponse response)
    {
        return new AvoirLineWrapper
        {
            Id = response.IdLi,
            ProductReference = response.RefProduit,
            Description = response.DesignationLi,
            QteLi = response.QteLi,
            PrixHt = response.PrixHt,
            Remise = response.Remise,
            Tva = response.Tva,
            TotHt = response.TotHt,
            TotTtc = response.TotTtc
        };
    }
}

