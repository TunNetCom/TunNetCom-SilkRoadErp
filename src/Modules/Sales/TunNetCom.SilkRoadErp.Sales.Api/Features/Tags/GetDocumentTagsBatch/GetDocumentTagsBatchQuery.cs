using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetDocumentTagsBatch;

public record GetDocumentTagsBatchQuery(
    string DocumentType,
    IReadOnlyList<int> DocumentIds) : IRequest<GetDocumentTagsBatchResponse>;

