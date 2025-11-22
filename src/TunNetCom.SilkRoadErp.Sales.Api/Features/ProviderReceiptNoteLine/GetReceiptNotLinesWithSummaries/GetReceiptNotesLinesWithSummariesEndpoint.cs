using TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.GetReceiptNotLinesWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteLines;

public class GetReceiptNotesLinesWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/receipt_note/lines/{idReceiptNote:int}",
            async (int idReceiptNote, [AsParameters] GetReceiptNoteLinesWithSummariesQueryParams request,
            IMediator mediator) =>
        {
            var validationResults = new GetReceiptNotesLinesWithSummariesValidator().Validate(request);

            if (!validationResults.IsValid)
            {
                var errors = validationResults.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(new { Errors = errors });
            }

            var query = new GetReceiptNotesLinesWithSummariesQuery(
                queryStringParameters : new QueryStringParameters
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    SortOrder = request.SortOrder,
                    SortProprety = request.SortProperty,
                    SearchKeyword = request.SearchKeyword,
                },
                IdReceiptNote : idReceiptNote
            );

            var result = await mediator.Send(query);

            if(result.IsFailed)
                return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
            
            return Results.Ok(result.Value);

        })
        .WithTags(EndpointTags.ProviderReceiptNoteLines)
        .WithName("Get receipt note lines by receipt note id")
        .WithSummary("Get receipt note lines by receipt note id")
        .Produces<object>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
