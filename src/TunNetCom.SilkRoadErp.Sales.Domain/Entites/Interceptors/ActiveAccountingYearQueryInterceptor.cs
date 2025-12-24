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

        var commandText = command.CommandText;
        if (string.IsNullOrEmpty(commandText))
            return;

        // Ne modifier que les requêtes SELECT, pas les INSERT, UPDATE, DELETE
        var trimmedCommand = commandText.TrimStart();
        if (!trimmedCommand.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            return;

        // Liste des tables qui implémentent IAccountingYearEntity
        var accountingYearTables = new[] { "Facture", "FactureFournisseur", "FactureAvoirFournisseur", "FactureAvoirClient", 
            "BonDeLivraison", "BonDeReception", "Avoirs", "AvoirFournisseur", "Inventaire" };

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

        // Ignorer les requêtes complexes avec sous-requêtes EXISTS, IN, ou autres sous-SELECT
        // Ces requêtes peuvent avoir des alias de table différents dans les sous-requêtes
        // et l'intercepteur ne peut pas les modifier correctement
        if (commandText.Contains("EXISTS", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("IN (SELECT", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("NOT EXISTS", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("NOT IN (SELECT", StringComparison.OrdinalIgnoreCase))
        {
            // Pour les requêtes complexes, on ne modifie pas le SQL
            // Les requêtes doivent utiliser .FilterByActiveAccountingYear() explicitement
            return;
        }

        // Ignorer les requêtes qui utilisent des collections avec .Select() dans le SELECT principal
        // Ces requêtes génèrent des sous-requêtes corrélées qui ne peuvent pas être modifiées correctement
        // Exemple: SELECT ... FactureIds = p.Factures.Select(f => f.FactureId).ToList() ...
        // Ces sous-requêtes sont générées par EF Core et ne doivent pas être modifiées par l'intercepteur
        // On détecte cela en cherchant des patterns de sous-SELECT corrélés dans le SELECT principal
        // Pattern: (SELECT ... FROM [Table] AS [alias] WHERE [alias].[ForeignKey] = [outer].[PrimaryKey])
        var correlatedSubqueryPattern = @"\(SELECT\s+.*?FROM\s+\[(\w+)\]\s+AS\s+\[(\w+)\]\s+WHERE\s+\[(\w+)\]\.\[(\w+)\]\s*=\s*\[(\w+)\]\.\[(\w+)\]";
        var correlatedMatches = System.Text.RegularExpressions.Regex.Matches(commandText, correlatedSubqueryPattern, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
        
        // Si on trouve des sous-requêtes corrélées avec des tables qui ont AccountingYearId,
        // on ignore la modification pour éviter les erreurs SQL
        if (correlatedMatches.Count > 0)
        {
            var correlatedTables = correlatedMatches.Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Groups[1].Value)
                .Where(t => accountingYearTables.Contains(t, StringComparer.OrdinalIgnoreCase))
                .ToList();
            
            if (correlatedTables.Any())
            {
                // Il y a des sous-requêtes corrélées avec des tables qui ont AccountingYearId
                // On ne modifie pas ces requêtes pour éviter les erreurs SQL
                return;
            }
        }
        
        // Détection alternative: si la requête contient plusieurs FROM avec des tables qui ont AccountingYearId
        // et qu'il y a des références croisées (ce qui indique des sous-requêtes corrélées)
        var fromPatternAlt = @"FROM\s+\[(\w+)\]\s+AS\s+\[(\w+)\]";
        var fromMatches = System.Text.RegularExpressions.Regex.Matches(commandText, fromPatternAlt, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        var allTables = fromMatches.Cast<System.Text.RegularExpressions.Match>()
            .Select(m => m.Groups[1].Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        var tablesWithAccountingYear = allTables
            .Where(t => accountingYearTables.Contains(t, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        // Si on trouve plusieurs tables avec AccountingYearId dans la requête,
        // c'est probablement une sous-requête corrélée (même si la table principale n'est pas dans la liste)
        if (tablesWithAccountingYear.Count > 1)
        {
            // Il y a des sous-requêtes avec des tables qui ont AccountingYearId
            // On ne modifie pas ces requêtes pour éviter les erreurs SQL
            return;
        }
        
        // Si la table principale n'est pas dans la liste des tables avec AccountingYearId,
        // mais qu'il y a des sous-requêtes avec des tables qui ont AccountingYearId,
        // on ignore la modification (c'est probablement une sous-requête corrélée)
        if (tableName != null && !accountingYearTables.Contains(tableName, StringComparer.OrdinalIgnoreCase) && tablesWithAccountingYear.Any())
        {
            // La table principale n'a pas AccountingYearId, mais il y a des sous-requêtes qui en ont
            // On ne modifie pas ces requêtes pour éviter les erreurs SQL
            return;
        }

        // Trouver l'alias de la table principale (ex: "FROM [Facture] AS [f]" -> alias = "f")
        // On cherche seulement dans la requête principale, pas dans les sous-requêtes
        var fromPattern = $@"FROM\s+\[{tableName}\]\s+AS\s+\[(\w+)\]";
        var aliasMatches = System.Text.RegularExpressions.Regex.Matches(commandText, fromPattern, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        // Utiliser le premier alias trouvé (celui de la requête principale)
        var tableAlias = tableName; // Par défaut, utiliser le nom de la table
        if (aliasMatches.Count > 0)
        {
            // Prendre le premier match qui devrait être la requête principale
            tableAlias = aliasMatches[0].Groups[1].Value;
        }

        // Ajouter la condition WHERE seulement dans la requête principale
        // Trouver le WHERE principal (pas dans les sous-requêtes)
        var mainWhereIndex = -1;
        var whereMatches = System.Text.RegularExpressions.Regex.Matches(commandText, @"\bWHERE\b", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        // Le premier WHERE devrait être celui de la requête principale
        if (whereMatches.Count > 0)
        {
            mainWhereIndex = whereMatches[0].Index;
        }
        
        if (mainWhereIndex >= 0)
        {
            // Trouver la fin de la clause WHERE principale (avant ORDER BY, GROUP BY, etc.)
            var orderByIndex = commandText.IndexOf("ORDER BY", mainWhereIndex, StringComparison.OrdinalIgnoreCase);
            var groupByIndex = commandText.IndexOf("GROUP BY", mainWhereIndex, StringComparison.OrdinalIgnoreCase);
            var havingIndex = commandText.IndexOf("HAVING", mainWhereIndex, StringComparison.OrdinalIgnoreCase);
            
            var insertIndex = commandText.Length;
            if (orderByIndex >= 0) insertIndex = Math.Min(insertIndex, orderByIndex);
            if (groupByIndex >= 0) insertIndex = Math.Min(insertIndex, groupByIndex);
            if (havingIndex >= 0) insertIndex = Math.Min(insertIndex, havingIndex);

            var filterClause = $" AND [{tableAlias}].[AccountingYearId] = {activeYearId.Value}";
            command.CommandText = commandText.Insert(insertIndex, filterClause);
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

