using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.RemoveTagsFromDocument;

public class RemoveTagsFromDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tags/{documentType}/{documentId:int}/remove", HandleRemoveTagsFromDocumentAsync)
            .WithTags(EndpointTags.Tags)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
    }

    public static async Task<Results<NoContent, ValidationProblem>> HandleRemoveTagsFromDocumentAsync(
        IMediator mediator,
        string documentType,
        int documentId,
        RemoveTagsFromDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RemoveTagsFromDocumentCommand(
            DocumentType: documentType,
            DocumentId: documentId,
            TagIds: request.TagIds
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

