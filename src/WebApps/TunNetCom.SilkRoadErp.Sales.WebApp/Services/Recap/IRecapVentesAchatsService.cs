namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Recap;

public interface IRecapVentesAchatsService
{
    /// <summary>
    /// Loads recap data for the given period: ventes (factures, avoirs, net), achats (factures fournisseurs, avoirs fournisseur, factures dépenses, net), paiements, and résultat (ventes nettes - achats nets).
    /// </summary>
    Task<RecapVentesAchatsViewModel> GetRecapAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
