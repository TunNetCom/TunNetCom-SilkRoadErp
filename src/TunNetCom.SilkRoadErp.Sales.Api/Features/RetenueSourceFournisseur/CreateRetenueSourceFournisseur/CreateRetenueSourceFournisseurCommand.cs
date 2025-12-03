using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.CreateRetenueSourceFournisseur;

public record CreateRetenueSourceFournisseurCommand(
    int NumFactureFournisseur,
    string? NumTej,
    string? PdfContent) : IRequest<Result<int>>;


