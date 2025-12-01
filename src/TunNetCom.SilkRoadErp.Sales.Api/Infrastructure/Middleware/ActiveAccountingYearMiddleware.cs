namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Middleware;

/// <summary>
/// Middleware pour mettre en cache l'ID de l'exercice comptable actif au début de chaque requête HTTP.
/// Cela permet aux Global Query Filters d'utiliser cette valeur de manière synchrone.
/// </summary>
public class ActiveAccountingYearMiddleware
{
    private readonly RequestDelegate _next;

    public ActiveAccountingYearMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IActiveAccountingYearService activeAccountingYearService)
    {
        // Précharger l'exercice actif au début de la requête pour qu'il soit disponible dans les Global Query Filters
        var activeYearId = await activeAccountingYearService.GetActiveAccountingYearIdAsync(context.RequestAborted);
        
        // Mettre à jour la variable statique thread-safe dans SalesContext
        // Cela permettra aux Global Query Filters d'utiliser cette valeur
        Domain.Entites.SalesContext.SetActiveAccountingYearId(activeYearId);

        await _next(context);
        
        // Nettoyer la valeur après la requête
        Domain.Entites.SalesContext.SetActiveAccountingYearId(null);
    }
}

