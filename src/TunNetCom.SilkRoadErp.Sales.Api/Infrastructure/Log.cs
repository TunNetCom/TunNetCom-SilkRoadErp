namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;

public static partial class Log
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information,
        Message = "Fetching clients with pageIndex: {PageIndex} and pageSize: {PageSize}")]
    public static partial void FetchingClients(ILogger logger, int pageIndex, int pageSize);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Fetched {Count} clients")]
    public static partial void FetchedClients(ILogger logger, int count);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "Creating client")]
    public static partial void CreatingClient(ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information,
        Message = "Client created successfully with ID: {Id}")]
    public static partial void ClientCreated(ILogger logger, int id);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information,
        Message = "Attempting to delete client with ID: {Id}")]
    public static partial void DeletingClient(ILogger logger, int id);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information,
        Message = "Client with ID: {Id} deleted successfully")]
    public static partial void ClientDeleted(ILogger logger, int id);

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning,
        Message = "Client with ID: {Id} not found")]
    public static partial void ClientNotFound(ILogger logger, int id);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, 
        Message = "Fetching client with ID: {Id}")]
    public static partial void FetchingClientById(ILogger logger, int id);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information,
        Message = "Attempting to update client with ID: {Id}")]
    public static partial void UpdatingClient(ILogger logger, int id);

    [LoggerMessage(EventId = 9, Level = LogLevel.Information,
        Message = "Client with ID: {Id} updated successfully")]
    public static partial void ClientUpdated(ILogger logger, int id);
}