using FluentResults;
using MediatR;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;
using TunNetCom.SilkRoadErp.Sales.Contracts;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetInventaires;

public record GetInventairesQuery(
    int PageNumber,
    int PageSize,
    string? SearchKeyword,
    string? SortProperty,
    string? SortOrder,
    int? AccountingYearId
) : IRequest<PagedList<InventaireResponse>>;

