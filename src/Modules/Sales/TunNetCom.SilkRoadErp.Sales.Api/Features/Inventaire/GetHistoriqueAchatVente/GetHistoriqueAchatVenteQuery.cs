using FluentResults;
using MediatR;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetHistoriqueAchatVente;

public record GetHistoriqueAchatVenteQuery(int ProductId) : IRequest<Result<List<HistoriqueAchatVenteResponse>>>;

