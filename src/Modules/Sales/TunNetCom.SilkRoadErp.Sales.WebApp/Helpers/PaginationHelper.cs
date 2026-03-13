using Radzen;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

/// <summary>
/// Helper pour les calculs de pagination à partir de Radzen LoadDataArgs.
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Retourne le nombre d'éléments à ignorer (skip), jamais null.
    /// </summary>
    public static int GetSafeSkip(LoadDataArgs args) => args.Skip ?? 0;

    /// <summary>
    /// Retourne la taille de page (top), toujours &gt; 0. Si args.Top est null ou 0, utilise defaultPageSize.
    /// </summary>
    public static int GetSafeTop(LoadDataArgs args, int defaultPageSize)
    {
        var top = args.Top ?? defaultPageSize;
        return top > 0 ? top : defaultPageSize;
    }

    /// <summary>
    /// Calcule le numéro de page (1-based) à partir de skip et top.
    /// </summary>
    public static int GetPageNumber(int skip, int top)
    {
        if (top <= 0) return 1;
        return (skip / top) + 1;
    }

    /// <summary>
    /// DTO pour les paramètres de pagination dérivés de LoadDataArgs.
    /// </summary>
    public readonly record struct PagingParams(int Skip, int Top, int PageNumber)
    {
        public int PageSize => Top;
    }

    /// <summary>
    /// Extrait des paramètres de pagination sûrs à partir de LoadDataArgs.
    /// </summary>
    /// <param name="args">Arguments du RadzenDataGrid LoadData.</param>
    /// <param name="defaultPageSize">Taille de page par défaut si args.Top est null ou 0.</param>
    /// <returns>Skip, Top (toujours &gt; 0), et PageNumber (1-based).</returns>
    public static PagingParams FromLoadDataArgs(LoadDataArgs args, int defaultPageSize)
    {
        var skip = GetSafeSkip(args);
        var top = GetSafeTop(args, defaultPageSize);
        var pageNumber = GetPageNumber(skip, top);
        return new PagingParams(skip, top, pageNumber);
    }
}
