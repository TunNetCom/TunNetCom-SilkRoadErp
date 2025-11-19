namespace TunNetCom.SilkRoadErp.Sales.Contracts.Common;

/// <summary>
/// Interface for line items that can be calculated (delivery notes, invoices, etc.)
/// </summary>
public interface ILineItem
{
    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price excluding tax
    /// </summary>
    decimal UnitPriceExcludingTax { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage
    /// </summary>
    double DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the VAT percentage
    /// </summary>
    double VatPercentage { get; set; }

    /// <summary>
    /// Gets or sets the total excluding tax
    /// </summary>
    decimal TotalExcludingTax { get; set; }

    /// <summary>
    /// Gets or sets the total including tax
    /// </summary>
    decimal TotalIncludingTax { get; set; }
}

