using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly HashSet<string> ExcludedEntityNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "AuditLog",
        "__EFMigrationsHistory"
    };

    public AuditSaveChangesInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SaveChangesInternal(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SaveChangesInternal(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SaveChangesInternal(DbContext? context)
    {
        if (context == null)
            return;

        var auditLogs = new List<AuditLog>();
        var currentUserProvider = _serviceProvider.GetService<ICurrentUserProvider>();
        
        var userId = currentUserProvider?.GetUserId();
        var username = currentUserProvider?.GetUsername();
        
        // Log for debugging
        var logger = _serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<AuditSaveChangesInterceptor>>();
        if (currentUserProvider == null)
        {
            logger?.LogWarning("AuditLog: CurrentUserProvider is NULL");
        }
        else
        {
            logger?.LogDebug("AuditLog: UserId={UserId}, Username={Username}, IsAuthenticated={IsAuthenticated}", 
                userId, username ?? "null", currentUserProvider.IsAuthenticated());
        }
        
        // Use "System" as fallback if username is null
        username = username ?? "System";

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added 
                     || e.State == EntityState.Modified 
                     || e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType();
            var entityName = entityType.Name;

            // Exclude system entities
            if (ExcludedEntityNames.Contains(entityName) || entityName.StartsWith("sys.", StringComparison.OrdinalIgnoreCase))
                continue;

            var entityId = GetEntityId(entry);
            if (entityId == null)
                continue;

            AuditAction action;
            Dictionary<string, object?>? oldValues = null;
            Dictionary<string, object?>? newValues = null;
            List<string>? changedProperties = null;

            if (entry.State == EntityState.Added)
            {
                action = AuditAction.Created;
                newValues = GetPropertyValues(entry, includeAll: true);
            }
            else if (entry.State == EntityState.Modified)
            {
                action = AuditAction.Updated;
                oldValues = GetOriginalPropertyValues(entry);
                newValues = GetPropertyValues(entry, includeAll: false);
                changedProperties = GetChangedProperties(entry);
            }
            else if (entry.State == EntityState.Deleted)
            {
                action = AuditAction.Deleted;
                oldValues = GetOriginalPropertyValues(entry);
            }
            else
            {
                continue;
            }

            var auditLog = AuditLog.Create(
                entityName: entityName,
                entityId: entityId,
                action: action,
                userId: userId,
                username: username,
                oldValues: oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                newValues: newValues != null ? JsonSerializer.Serialize(newValues) : null,
                changedProperties: changedProperties != null ? JsonSerializer.Serialize(changedProperties) : null
            );

            auditLogs.Add(auditLog);
        }

        // Add audit logs to context
        if (auditLogs.Any())
        {
            context.Set<AuditLog>().AddRange(auditLogs);
        }
    }

    private string? GetEntityId(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key == null)
            return null;

        var keyValues = key.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? string.Empty)
            .ToArray();

        return string.Join("|", keyValues);
    }

    private Dictionary<string, object?> GetPropertyValues(EntityEntry entry, bool includeAll)
    {
        var values = new Dictionary<string, object?>();
        // entry.Properties only contains scalar properties, not navigation properties
        var properties = entry.Properties.ToList();

        foreach (var property in properties)
        {
            if (includeAll || property.IsModified)
            {
                var value = property.CurrentValue;
                values[property.Metadata.Name] = FormatPropertyValue(value);
            }
        }

        return values;
    }

    private Dictionary<string, object?> GetOriginalPropertyValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        // entry.Properties only contains scalar properties, not navigation properties
        var properties = entry.Properties
            .Where(p => p.IsModified)
            .ToList();

        foreach (var property in properties)
        {
            var value = property.OriginalValue;
            values[property.Metadata.Name] = FormatPropertyValue(value);
        }

        return values;
    }

    private List<string> GetChangedProperties(EntityEntry entry)
    {
        // entry.Properties only contains scalar properties, not navigation properties
        return entry.Properties
            .Where(p => p.IsModified)
            .Select(p => p.Metadata.Name)
            .ToList();
    }

    private object? FormatPropertyValue(object? value)
    {
        if (value == null)
            return null;

        // Handle DateTime to ensure consistent formatting
        if (value is DateTime dateTime)
            return dateTime.ToString("O"); // ISO 8601 format

        if (value is DateTimeOffset dateTimeOffset)
            return dateTimeOffset.ToString("O");

        // Handle collections and complex types
        if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            return null; // Skip collections
        }

        return value;
    }
}

