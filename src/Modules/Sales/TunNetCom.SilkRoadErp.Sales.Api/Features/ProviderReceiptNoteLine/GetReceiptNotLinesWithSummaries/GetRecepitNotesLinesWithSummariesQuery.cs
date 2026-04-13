using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Response;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.GetReceiptNotLinesWithSummaries;

internal record class GetReceiptNotesLinesWithSummariesQuery(
    QueryStringParameters queryStringParameters,
    int IdReceiptNote
    ) : IRequest<Result<GetReceiptNoteLinesByReceiptNoteIdResponse>>;