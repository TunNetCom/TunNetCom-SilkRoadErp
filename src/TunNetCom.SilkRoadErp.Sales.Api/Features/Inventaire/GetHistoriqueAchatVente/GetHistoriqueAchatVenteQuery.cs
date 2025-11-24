using FluentResults;
using MediatR;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetHistoriqueAchatVente;

public record GetHistoriqueAchatVenteQuery(string RefProduit) : IRequest<Result<List<HistoriqueAchatVenteResponse>>>;

