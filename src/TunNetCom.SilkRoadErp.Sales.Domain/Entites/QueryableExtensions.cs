using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public static class QueryableExtensions
{
    /// <summary>
    /// Applique le filtre d'année comptable active à une requête IQueryable.
    /// </summary>
    public static IQueryable<T> FilterByActiveAccountingYear<T>(this IQueryable<T> query) 
        where T : class, IAccountingYearEntity
    {
        var activeYearId = SalesContext.GetActiveAccountingYearId();
        if (!activeYearId.HasValue)
        {
            // Si aucune année n'est active, retourner une requête vide
            return query.Where(x => false);
        }
        
        return query.Where(x => x.AccountingYearId == activeYearId.Value);
    }
}

