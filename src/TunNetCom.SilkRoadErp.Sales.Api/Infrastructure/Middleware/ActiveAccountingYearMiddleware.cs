namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Middleware;

using Microsoft.Extensions.Logging;

/// <summary>
/// Middleware pour mettre en cache l'ID de l'exercice comptable actif au début de chaque requête HTTP.
/// Cela permet aux Global Query Filters d'utiliser cette valeur de manière synchrone.
/// </summary>
public class ActiveAccountingYearMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActiveAccountingYearMiddleware> _logger;

    public ActiveAccountingYearMiddleware(RequestDelegate next, ILogger<ActiveAccountingYearMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IActiveAccountingYearService activeAccountingYearService)
    {
        // Optimisation: Vérifier d'abord le cache de manière synchrone (zero overhead)
        // Cela évite les appels async inutiles, les verrous de semaphore, et la création de scopes
        var activeYearId = activeAccountingYearService.GetActiveAccountingYearId();
        
        // Seulement appeler la méthode async si le cache est vide
        if (!activeYearId.HasValue)
        {
            activeYearId = await activeAccountingYearService.GetActiveAccountingYearIdAsync(context.RequestAborted);
        }
        
        // Mettre à jour la variable statique thread-safe dans SalesContext seulement si nécessaire
        // Vérifier d'abord la valeur actuelle pour éviter les mises à jour inutiles
        var currentAsyncLocalValue = Domain.Entites.SalesContext.GetActiveAccountingYearId();
        if (currentAsyncLocalValue != activeYearId)
        {
            Domain.Entites.SalesContext.SetActiveAccountingYearId(activeYearId);
        }

        await _next(context);
        
        // Nettoyer la valeur après la requête pour éviter les fuites entre requêtes
        Domain.Entites.SalesContext.SetActiveAccountingYearId(null);
    }
}

