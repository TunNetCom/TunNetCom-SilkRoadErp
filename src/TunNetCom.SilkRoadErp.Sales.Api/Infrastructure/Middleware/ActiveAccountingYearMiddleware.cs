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
        // Précharger l'exercice actif au début de la requête
        var activeYearId = await activeAccountingYearService.GetActiveAccountingYearIdAsync(context.RequestAborted);
        
        _logger.LogDebug("ActiveAccountingYearMiddleware: Loaded active year ID {ActiveYearId} for request {Path}", 
            activeYearId, context.Request.Path);
        
        // Mettre à jour la variable statique thread-safe dans SalesContext
        // Cela permet à l'extension method FilterByActiveAccountingYear() d'utiliser cette valeur
        Domain.Entites.SalesContext.SetActiveAccountingYearId(activeYearId);

        await _next(context);
        
        // Nettoyer la valeur après la requête pour éviter les fuites entre requêtes
        Domain.Entites.SalesContext.SetActiveAccountingYearId(null);
        
        _logger.LogDebug("ActiveAccountingYearMiddleware: Cleared active year ID after request {Path}", context.Request.Path);
    }
}

