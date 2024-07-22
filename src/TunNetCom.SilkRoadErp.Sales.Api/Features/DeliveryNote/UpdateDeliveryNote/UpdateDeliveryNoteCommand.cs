namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;

public record UpdateDeliveryNoteCommand(
    int Num,
    DateTime Date,
    decimal TotHTva,
    decimal TotTva,
    decimal NetPayer,
    TimeOnly TempBl,
    int? NumFacture,
    int? ClientId
    ) : IRequest<Result>;

