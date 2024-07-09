namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;

public static partial class LoggerMessageDefinitionsGen
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Creating client with values: {@Model}")]
    public static partial void LogClientCreated(this ILogger logger, object model);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, 
        Message = "Fetching clients with pageIndex: {PageIndex} and pageSize: {PageSize}")]
    public static partial void LogPaginationRequest(this ILogger logger, int pageIndex, int pageSize);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, 
        Message = "Fetched {Count} clients")]
    public static partial void LogClientsFetched(this ILogger logger, int count);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, 
        Message = "Attempting to delete client with ID: {Id}")]
    public static partial void LogClientDeletionAttempt(this ILogger logger, int id);

    [LoggerMessage(EventId = 5, Level = LogLevel.Warning, 
        Message = "Client with ID: {Id} not found")]
    public static partial void LogClientNotFound(this ILogger logger, int id);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information, 
        Message = "Client with ID: {Id} deleted successfully")]
    public static partial void LogClientDeleted(this ILogger logger, int id);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, 
        Message = "Fetching client with ID: {Id}")]
    public static partial void LogFetchingClientById(this ILogger logger, int id);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information, 
        Message = "Fetched client with ID: {Id}")]
    public static partial void LogClientFetchedById(this ILogger logger, int id);

    [LoggerMessage(EventId = 9, Level = LogLevel.Information, 
        Message = "Attempting to update client with ID: {Id}")]
    public static partial void LogClientUpdateAttempt(this ILogger logger, int id);

    [LoggerMessage(EventId = 10, Level = LogLevel.Information, 
        Message = "Client with ID: {Id} updated successfully")]
    public static partial void LogClientUpdated(this ILogger logger, int id);

    [LoggerMessage(EventId = 11, Level = LogLevel.Information, 
        Message = "Client created successfully with ID: {Id}")]
    public static partial void LogClientCreatedSuccessfully(this ILogger logger, int id);
}