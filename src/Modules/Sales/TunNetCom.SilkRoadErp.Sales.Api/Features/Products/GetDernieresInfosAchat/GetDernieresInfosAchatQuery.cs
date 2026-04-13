using FluentResults;
using MediatR;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetDernieresInfosAchat;

public record GetDernieresInfosAchatQuery(string RefProduit) : IRequest<Result<GetDernieresInfosAchatResponse>>;

