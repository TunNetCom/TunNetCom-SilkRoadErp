using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetDocumentTags;

public class GetDocumentTagsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tags/{documentType}/{documentId:int}", HandleGetDocumentTagsAsync)
            .WithTags(EndpointTags.Tags)
            .Produces<DocumentTagResponse>();
    }

    public static async Task<Ok<DocumentTagResponse>> HandleGetDocumentTagsAsync(
        IMediator mediator,
        string documentType,
        int documentId,
        CancellationToken cancellationToken)
    {
        var query = new GetDocumentTagsQuery(documentType, documentId);
        var result = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}

