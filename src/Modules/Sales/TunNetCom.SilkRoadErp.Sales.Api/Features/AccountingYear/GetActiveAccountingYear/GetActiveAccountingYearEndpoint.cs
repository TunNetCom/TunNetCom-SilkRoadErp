namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetActiveAccountingYear;

public class GetActiveAccountingYearEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/accountingYear/active", HandleGetActiveAccountingYearAsync)
            .WithTags("AccountingYear");
    }

    public async Task<Results<Ok<GetActiveAccountingYearResponse>, NotFound>> HandleGetActiveAccountingYearAsync(
        IMediator mediator,
        HttpContext httpContext,
        ILogger<GetActiveAccountingYearEndpoint> logger,
        CancellationToken cancellationToken)
    {
        var requestId = httpContext.TraceIdentifier;
        var clientIp = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
        var userAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "(none)";
        var hasAuth = httpContext.Request.Headers.Authorization.Count > 0;

        logger.LogInformation(
            "GET /accountingYear/active requested at {Timestamp:O} | RequestId: {RequestId} | ClientIP: {ClientIP} | UserAgent: {UserAgent} | HasAuth: {HasAuth}",
            DateTime.UtcNow,
            requestId,
            clientIp,
            userAgent,
            hasAuth);

        var result = await mediator.Send(new GetActiveAccountingYearQuery(), cancellationToken);

        if (result.IsFailed)
        {
            logger.LogWarning("GET /accountingYear/active returned NotFound | RequestId: {RequestId}", requestId);
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

