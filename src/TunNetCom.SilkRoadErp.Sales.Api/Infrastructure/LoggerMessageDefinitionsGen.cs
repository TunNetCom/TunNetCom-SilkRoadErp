namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;

public static partial class LoggerMessageDefinitionsGen
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Creating customer with values: {@Model}")]
    public static partial void LogCustomerCreated(this ILogger logger, object model);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, 
        Message = "Fetching customers with pageIndex: {PageIndex} and pageSize: {PageSize}")]
    public static partial void LogPaginationRequest(this ILogger logger, int pageIndex, int pageSize);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, 
        Message = "Fetched {Count} customers")]
    public static partial void LogCustomersFetched(this ILogger logger, int count);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, 
        Message = "Attempting to delete customer with ID: {Id}")]
    public static partial void LogCustomerDeletionAttempt(this ILogger logger, int id);

    [LoggerMessage(EventId = 5, Level = LogLevel.Warning, 
        Message = "customer with ID: {Id} not found")]
    public static partial void LogCustomerNotFound(this ILogger logger, int id);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information, 
        Message = "customer with ID: {Id} deleted successfully")]
    public static partial void LogCustomerDeleted(this ILogger logger, int id);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, 
        Message = "Fetching customer with ID: {Id}")]
    public static partial void LogFetchingCustomerById(this ILogger logger, int id);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information, 
        Message = "Fetched customer with ID: {Id}")]
    public static partial void LogCustomerFetchedById(this ILogger logger, int id);

    [LoggerMessage(EventId = 9, Level = LogLevel.Information, 
        Message = "Attempting to update customer with ID: {Id}")]
    public static partial void LogCustomerUpdateAttempt(this ILogger logger, int id);

    [LoggerMessage(EventId = 10, Level = LogLevel.Information, 
        Message = "customer with ID: {Id} updated successfully")]
    public static partial void LogCustomerUpdated(this ILogger logger, int id);

    [LoggerMessage(EventId = 11, Level = LogLevel.Information, 
        Message = "customer created successfully with ID: {Id}")]
    public static partial void LogCustomerCreatedSuccessfully(this ILogger logger, int id);
}