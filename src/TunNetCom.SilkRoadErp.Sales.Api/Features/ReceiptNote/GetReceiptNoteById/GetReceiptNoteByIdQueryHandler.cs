using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DeleteReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdQueryHandler(
    SalesContext _context,
    ILogger<GetReceiptNoteByIdQueryHandler> _logger)
    : IRequestHandler<GetReceiptNoteByIdQuery, Result<ReceiptNoteResponse>>
{
    public async Task<Result<ReceiptNoteResponse>> Handle(GetReceiptNoteByIdQuery getReceiptNoteByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        var receiptnote = await _context.BonDeReception.FindAsync(getReceiptNoteByIdQuery.Num, cancellationToken);
        if (receiptnote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);

            return Result.Fail("receiptnote_not_found");
        }
        _logger.LogEntityFetchedById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        return receiptnote.Adapt<ReceiptNoteResponse>();
    }
}
