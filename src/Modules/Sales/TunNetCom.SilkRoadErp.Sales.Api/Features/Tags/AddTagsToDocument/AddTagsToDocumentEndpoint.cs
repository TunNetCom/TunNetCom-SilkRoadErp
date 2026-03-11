using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.AddTagsToDocument;

public class AddTagsToDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tags/{documentType}/{documentId:int}", HandleAddTagsToDocumentAsync)
            .WithTags(EndpointTags.Tags)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();
    }

    public static async Task<Results<Ok, ValidationProblem>> HandleAddTagsToDocumentAsync(
        IMediator mediator,
        string documentType,
        int documentId,
        AddTagsToDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTagsToDocumentCommand(
            DocumentType: documentType,
            DocumentId: documentId,
            TagIds: request.TagIds
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok();
    }
}

