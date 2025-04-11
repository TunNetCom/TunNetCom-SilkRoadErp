namespace TunNetCom.SilkRoadErp.Sales.Api.Exceptions;

public class InvalidPaginationParamsException : Exception
{
    public InvalidPaginationParamsException()
       : base("Invalid pagination parameters: PageNumber and PageSize must be greater than 0.")
    {
    }

    public InvalidPaginationParamsException(string message)
        : base(message)
    {
    }

    public InvalidPaginationParamsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
