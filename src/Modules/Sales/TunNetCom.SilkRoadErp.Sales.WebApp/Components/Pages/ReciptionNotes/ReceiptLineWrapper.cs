using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Classes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Pages.ReciptionNotes;

public class ReceiptLineWrapper : ReceiptLine
{
    public string ProductReferenceAndDescription => 
        !string.IsNullOrEmpty(ProductReference) && !string.IsNullOrEmpty(ItemDescription)
            ? $"{ProductReference} - {ItemDescription}"
            : ProductReference ?? string.Empty;
}
