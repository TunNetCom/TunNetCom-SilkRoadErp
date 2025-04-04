﻿using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByInvoiceId;

public record GetDeliveryNotesByInvoiceIdQuery (int NumFacture) : IRequest<Result<List<DeliveryNoteResponse>>>;
