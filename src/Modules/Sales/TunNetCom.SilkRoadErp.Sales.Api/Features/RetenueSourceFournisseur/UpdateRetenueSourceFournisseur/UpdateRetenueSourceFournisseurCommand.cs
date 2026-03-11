using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.UpdateRetenueSourceFournisseur;

public record UpdateRetenueSourceFournisseurCommand(
    int NumFactureFournisseur,
    string? NumTej,
    string? PdfContent) : IRequest<Result>;


