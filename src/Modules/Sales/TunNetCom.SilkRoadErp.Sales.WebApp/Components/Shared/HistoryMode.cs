namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Shared;

/// <summary>
/// Defines the display mode for the ProductHistoryDialog.
/// </summary>
public enum HistoryMode
{
    /// <summary>
    /// Purchase mode: Shows both Achats and Ventes tabs.
    /// </summary>
    Achat,
    
    /// <summary>
    /// Sales mode: Shows current product info card + Ventes history.
    /// </summary>
    Vente
}
