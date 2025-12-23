using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Interceptors;

/// <summary>
/// Interceptor qui met à jour l'AsyncLocal avec l'année comptable active avant chaque requête.
/// Cela garantit que les Global Query Filters utilisent toujours la bonne valeur, même si
/// le filtre est évalué avant que le middleware ne s'exécute.
/// </summary>
public class ActiveAccountingYearQueryInterceptor : DbCommandInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    public ActiveAccountingYearQueryInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<System.Data.Common.DbDataReader> ReaderExecuting(
        System.Data.Common.DbCommand command,
        CommandEventData eventData,
        InterceptionResult<System.Data.Common.DbDataReader> result)
    {
        EnsureActiveAccountingYearSet(eventData.Context);
        ModifyCommandForAccountingYearFilter(command, eventData.Context);
        
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<System.Data.Common.DbDataReader>> ReaderExecutingAsync(
        System.Data.Common.DbCommand command,
        CommandEventData eventData,
        InterceptionResult<System.Data.Common.DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        EnsureActiveAccountingYearSet(eventData.Context);
        ModifyCommandForAccountingYearFilter(command, eventData.Context);
        
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    private void ModifyCommandForAccountingYearFilter(System.Data.Common.DbCommand command, DbContext? context)
    {
        if (context == null || context is not SalesContext)
            return;

        var activeYearId = SalesContext.GetActiveAccountingYearId();
        if (!activeYearId.HasValue)
            return;

        // Liste des tables qui implémentent IAccountingYearEntity
        var accountingYearTables = new[] { "Facture", "FactureFournisseur", "FactureAvoirFournisseur", "FactureAvoirClient", 
            "BonDeLivraison", "BonDeReception", "Avoirs", "AvoirFournisseur", "Inventaire" };

        var commandText = command.CommandText;
        if (string.IsNullOrEmpty(commandText))
            return;

        // Vérifier si la requête concerne une table avec AccountingYearId
        // EF Core génère des requêtes avec des alias comme "FROM [Facture] AS [f]"
        var tableName = accountingYearTables.FirstOrDefault(t => 
            commandText.Contains($"[{t}]", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains($"FROM {t}", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains($"FROM [{t}]", StringComparison.OrdinalIgnoreCase));

        if (tableName == null)
            return;

        // Ajouter la condition WHERE si elle n'existe pas déjà
        // On cherche si AccountingYearId est déjà dans une clause WHERE (pas seulement dans SELECT)
        var whereIndexCheck = commandText.LastIndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
        if (whereIndexCheck >= 0 && commandText.Substring(whereIndexCheck).Contains("AccountingYearId", StringComparison.OrdinalIgnoreCase))
        {
            return; // Le filtre est déjà présent dans WHERE
        }

        // Trouver l'alias de la table (ex: "FROM [Facture] AS [f]" -> alias = "f")
        var aliasMatch = System.Text.RegularExpressions.Regex.Match(commandText, 
            $@"FROM\s+\[{tableName}\]\s+AS\s+\[(\w+)\]", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        var tableAlias = aliasMatch.Success ? aliasMatch.Groups[1].Value : tableName;

        // Ajouter la condition WHERE
        var hasWhere = commandText.Contains("WHERE", StringComparison.OrdinalIgnoreCase);
        
        if (hasWhere)
        {
            // Ajouter AND à la fin de la clause WHERE existante
            var whereIndex = commandText.LastIndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
            if (whereIndex >= 0)
            {
                // Trouver la fin de la clause WHERE (avant ORDER BY, GROUP BY, etc.)
                var orderByIndex = commandText.IndexOf("ORDER BY", whereIndex, StringComparison.OrdinalIgnoreCase);
                var groupByIndex = commandText.IndexOf("GROUP BY", whereIndex, StringComparison.OrdinalIgnoreCase);
                var havingIndex = commandText.IndexOf("HAVING", whereIndex, StringComparison.OrdinalIgnoreCase);
                
                var insertIndex = commandText.Length;
                if (orderByIndex >= 0) insertIndex = Math.Min(insertIndex, orderByIndex);
                if (groupByIndex >= 0) insertIndex = Math.Min(insertIndex, groupByIndex);
                if (havingIndex >= 0) insertIndex = Math.Min(insertIndex, havingIndex);

                var filterClause = $" AND [{tableAlias}].[AccountingYearId] = {activeYearId.Value}";
                command.CommandText = commandText.Insert(insertIndex, filterClause);
            }
        }
        else
        {
            // Ajouter WHERE avant ORDER BY, GROUP BY, etc.
            var orderByIndex = commandText.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
            var groupByIndex = commandText.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase);
            
            var insertIndex = commandText.Length;
            if (orderByIndex >= 0) insertIndex = Math.Min(insertIndex, orderByIndex);
            if (groupByIndex >= 0) insertIndex = Math.Min(insertIndex, groupByIndex);

            var filterClause = $" WHERE [{tableAlias}].[AccountingYearId] = {activeYearId.Value}";
            command.CommandText = commandText.Insert(insertIndex, filterClause);
        }
    }

    private void EnsureActiveAccountingYearSet(DbContext? context)
    {
        if (context == null || context is not SalesContext)
            return;

        // Si l'AsyncLocal est déjà défini, ne rien faire (le middleware l'a déjà fait)
        var currentValue = SalesContext.GetActiveAccountingYearId();
        if (currentValue.HasValue)
            return;

        // Sinon, charger depuis le service et mettre à jour l'AsyncLocal
        try
        {
            var service = _serviceProvider.GetService<IActiveAccountingYearService>();
            if (service != null)
            {
                var activeYearId = service.GetActiveAccountingYearId();
                if (activeYearId.HasValue)
                {
                    SalesContext.SetActiveAccountingYearId(activeYearId.Value);
                }
            }
        }
        catch
        {
            // Ignorer les erreurs pour ne pas bloquer les requêtes
        }
    }
}

