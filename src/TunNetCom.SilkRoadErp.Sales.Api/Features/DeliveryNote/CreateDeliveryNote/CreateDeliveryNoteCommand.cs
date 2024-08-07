﻿namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public record CreateDeliveryNoteCommand(
    DateTime Date,
    decimal TotHTva,
    decimal TotTva,
    decimal NetPayer,
    TimeOnly TempBl,
    int? NumFacture,
    int? ClientId,
    IEnumerable<LigneBlRequest> Lignes)
    : IRequest<Result<int>>;
