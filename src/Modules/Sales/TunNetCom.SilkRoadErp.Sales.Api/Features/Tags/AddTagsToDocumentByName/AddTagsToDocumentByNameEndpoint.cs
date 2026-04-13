using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.AddTagsToDocumentByName;

public class AddTagsToDocumentByNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tags/{documentType}/{documentId:int}/add-by-name", HandleAddTagsToDocumentByNameAsync)
            .WithTags(EndpointTags.Tags)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();
    }

    public static async Task<Results<Ok, ValidationProblem>> HandleAddTagsToDocumentByNameAsync(
        IMediator mediator,
        string documentType,
        int documentId,
        AddTagsToDocumentByNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTagsToDocumentByNameCommand(
            DocumentType: documentType,
            DocumentId: documentId,
            TagNames: request.TagNames
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok();
    }
}

