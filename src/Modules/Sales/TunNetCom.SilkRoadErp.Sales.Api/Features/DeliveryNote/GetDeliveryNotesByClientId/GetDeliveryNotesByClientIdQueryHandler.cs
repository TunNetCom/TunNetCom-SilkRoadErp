using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByClientId
{
    public class GetDeliveryNotesByClientIdQueryHandler : IRequestHandler<GetDeliveryNoteByClientIdQuery, Result<List<DeliveryNoteResponse>>>
    {
        private readonly SalesContext _context;
        private readonly ILogger<GetDeliveryNotesByClientIdQueryHandler> _logger;

        public GetDeliveryNotesByClientIdQueryHandler(
            SalesContext context,
            ILogger<GetDeliveryNotesByClientIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<DeliveryNoteResponse>>> Handle(
            GetDeliveryNoteByClientIdQuery query,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching delivery notes for ClientId {ClientId}", query.ClientId);

            var deliveryNotes = await _context.BonDeLivraison
                .Where(d => d.ClientId == query.ClientId)
                .Select(d => new DeliveryNoteResponse
                {
                    DeliveryNoteNumber = d.Num,
                    Date = d.Date,
                    CreationTime = d.TempBl,
                    CustomerId = d.ClientId,
                    InvoiceNumber = d.NumFacture,
                    TotalExcludingTax = d.TotHTva,
                    TotalVat = d.TotTva,
                    TotalAmount = d.NetPayer,
                    Items = new List<DeliveryNoteDetailResponse>() // Empty or populate as needed
                })
                .ToListAsync(cancellationToken);

            if (deliveryNotes.Count == 0)
            {
                _logger.LogWarning("No delivery notes found for ClientId {ClientId}", query.ClientId);
                return Result.Fail(EntityNotFound.Error("not_found"));
            }

            return Result.Ok(deliveryNotes);
        }
    }
}