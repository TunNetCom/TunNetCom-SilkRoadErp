using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetDocumentTagsBatch;

public class GetDocumentTagsBatchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tags/batch", HandleAsync)
            .WithTags(EndpointTags.Tags)
            .Produces<GetDocumentTagsBatchResponse>();
    }

    public static async Task<Ok<GetDocumentTagsBatchResponse>> HandleAsync(
        IMediator mediator,
        GetDocumentTagsBatchRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetDocumentTagsBatchQuery(request.DocumentType, request.DocumentIds);
        var result = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}

