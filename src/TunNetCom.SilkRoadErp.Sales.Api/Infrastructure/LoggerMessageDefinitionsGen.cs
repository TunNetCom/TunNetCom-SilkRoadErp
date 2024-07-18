namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;

public static partial class LoggerMessageDefinitionsGen
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Creating {EntityName} with values: {@Model}")]
    public static partial void LogEntityCreated(this ILogger logger, string entityName, object model);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "Fetching {EntityName} with pageIndex: {PageIndex} and pageSize: {PageSize}")]
    public static partial void LogPaginationRequest(this ILogger logger, string entityName, int pageIndex, int pageSize);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information,
        Message = "Fetched {Count} {EntityName}")]
    public static partial void LogEntitiesFetched(this ILogger logger, string entityName, int count);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information,
        Message = "Attempting to delete {EntityName} with ID: {EntityId}")]
    public static partial void LogEntityDeletionAttempt(this ILogger logger, string entityName, object entityId);

    [LoggerMessage(EventId = 5, Level = LogLevel.Warning,
        Message = "{EntityName} with ID: {EntityId} not found")]
    public static partial void LogEntityNotFound(this ILogger logger, string entityName, object entityId);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information,
        Message = "{EntityName} with ID: {EntityId} deleted successfully")]
    public static partial void LogEntityDeleted(this ILogger logger, string entityName, object entityId);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information,
        Message = "Fetching {EntityName} with ID: {EntityId}")]
    public static partial void LogFetchingEntityById(this ILogger logger, string entityName, object entityId);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information,
        Message = "Fetched {EntityName} with ID: {EntityId}")]
    public static partial void LogEntityFetchedById(this ILogger logger, string entityName, object entityId);

    [LoggerMessage(EventId = 9, Level = LogLevel.Information,
        Message = "Attempting to update {EntityName} with ID: {EntityId}")]
    public static partial void LogEntityUpdateAttempt(this ILogger logger, string entityName, object entityId);

    [LoggerMessage(EventId = 10, Level = LogLevel.Information,
        Message = "{EntityName} with ID: {EntityId} updated successfully")]
    public static partial void LogEntityUpdated(this ILogger logger, string entityName, object entityId);

    [LoggerMessage(EventId = 11, Level = LogLevel.Information,
        Message = "{EntityName} created successfully with ID: {EntityId}")]
    public static partial void LogEntityCreatedSuccessfully(this ILogger logger, string entityName, object entityId);
}
