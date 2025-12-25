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

        // Détecter si la requête utilise IgnoreQueryFilters() en vérifiant si elle contient
        // un filtre explicite sur AccountingYearId avec une valeur spécifique (pas une variable)
        // Les requêtes avec IgnoreQueryFilters() ont souvent un pattern comme:
        // WHERE [alias].[AccountingYearId] = @p0 ou WHERE [alias].[AccountingYearId] = 123
        // Si on trouve un filtre explicite sur AccountingYearId, on ignore la modification
        // car cela indique que la requête utilise IgnoreQueryFilters() puis un Where explicite
        var whereIndexCheck = commandText.LastIndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
        if (whereIndexCheck >= 0)
        {
            var whereClause = commandText.Substring(whereIndexCheck);
            // Vérifier si AccountingYearId est déjà filtré avec une valeur spécifique
            // Pattern: [alias].[AccountingYearId] = @p0 ou [alias].[AccountingYearId] = 123
            var accountingYearIdFilterPattern = @"\[(\w+)\]\.\[AccountingYearId\]\s*=\s*[@]?[\w\d]+";
            var existingFilter = System.Text.RegularExpressions.Regex.Match(whereClause, accountingYearIdFilterPattern, 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            if (existingFilter.Success)
            {
                // Il y a déjà un filtre explicite sur AccountingYearId
                // Cela indique probablement que la requête utilise IgnoreQueryFilters() puis un Where explicite
                // On ne modifie pas ces requêtes
                return;
            }
        }

        // Ignorer les requêtes avec JOIN car elles sont généralement générées par Include()
        // Les requêtes avec Include() génèrent des JOIN complexes et l'intercepteur ne peut pas
        // les modifier correctement car les alias peuvent être différents dans les différentes parties
        if (commandText.Contains("JOIN", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("INNER JOIN", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("LEFT JOIN", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("RIGHT JOIN", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("OUTER JOIN", StringComparison.OrdinalIgnoreCase))
        {
            // Les requêtes avec JOIN sont généralement des requêtes avec Include()
            // On ne modifie pas ces requêtes pour éviter les erreurs SQL
            // Les requêtes doivent utiliser .FilterByActiveAccountingYear() explicitement
            return;
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

        // Trouver l'alias de la table principale (ex: "FROM [BonDeLivraison] AS [b0]" -> alias = "b0")
        // EF Core génère des alias comme [b0], [b1], etc., pas juste [b]
        // On cherche seulement dans la requête principale, pas dans les sous-requêtes
        // Pattern amélioré pour capturer l'alias réel (peut être b0, b1, f0, etc.)
        var fromPattern = $@"FROM\s+\[{tableName}\]\s+AS\s+\[(\w+)\]";
        var aliasMatches = System.Text.RegularExpressions.Regex.Matches(commandText, fromPattern, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        // Utiliser le premier alias trouvé (celui de la requête principale)
        // Si aucun alias n'est trouvé, on ne peut pas modifier la requête de manière sûre
        string? tableAlias = null;
        if (aliasMatches.Count > 0)
        {
            // Prendre le premier match qui devrait être la requête principale
            tableAlias = aliasMatches[0].Groups[1].Value;
        }
        
        // Si aucun alias n'a été trouvé, on ne peut pas modifier la requête de manière sûre
        if (string.IsNullOrEmpty(tableAlias))
        {
            return;
        }
        
        // Valider que l'alias existe bien dans la requête avant de l'utiliser
        // On vérifie que l'alias est référencé dans la requête (dans SELECT, WHERE, ORDER BY, etc.)
        // Pattern pour vérifier que l'alias est utilisé: [alias]. ou [alias]
        var aliasUsagePattern = $@"\[{System.Text.RegularExpressions.Regex.Escape(tableAlias)}\](\.|,|\s|$)";
        var aliasUsageMatches = System.Text.RegularExpressions.Regex.Matches(commandText, aliasUsagePattern, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        // Si l'alias n'est pas utilisé dans la requête, on ne peut pas l'utiliser
        if (aliasUsageMatches.Count == 0)
        {
            return;
        }
        
        // Vérifier que l'alias est bien défini dans une clause FROM principale (pas dans une sous-requête)
        // On cherche le pattern "FROM [Table] AS [alias]" et on vérifie que l'alias correspond
        var fromDefinitionPattern = $@"FROM\s+\[{System.Text.RegularExpressions.Regex.Escape(tableName)}\]\s+AS\s+\[{System.Text.RegularExpressions.Regex.Escape(tableAlias)}\]";
        var fromDefinitionMatch = System.Text.RegularExpressions.Regex.Match(commandText, fromDefinitionPattern, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        // Si l'alias n'est pas défini dans une clause FROM principale, on ne peut pas l'utiliser
        if (!fromDefinitionMatch.Success)
        {
            return;
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
        
        // À ce point, tableAlias ne peut pas être null car on a vérifié plus haut
        if (tableAlias == null)
        {
            return;
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

