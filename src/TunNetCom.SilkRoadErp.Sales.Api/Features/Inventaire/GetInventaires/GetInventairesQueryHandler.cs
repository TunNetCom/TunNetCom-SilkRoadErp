using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;
using TunNetCom.SilkRoadErp.Sales.Contracts.Sorting;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetInventaires;

public class GetInventairesQueryHandler(
    SalesContext _context,
    ILogger<GetInventairesQueryHandler> _logger)
    : IRequestHandler<GetInventairesQuery, PagedList<InventaireResponse>>
{
    private const string _numColumn = nameof(InventaireResponse.Num);
    private const string _dateColumn = nameof(InventaireResponse.DateInventaire);
    private const string _statutColumn = nameof(InventaireResponse.Statut);

    public async Task<PagedList<InventaireResponse>> Handle(GetInventairesQuery query, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Domain.Entites.Inventaire), query.PageNumber, query.PageSize);

        var inventairesQuery = _context.Inventaire
            .FilterByActiveAccountingYear()
            .Include(i => i.AccountingYear)
            .Select(i => new InventaireResponse
            {
                Id = i.Id,
                Num = i.Num,
                AccountingYearId = i.AccountingYearId,
                AccountingYear = i.AccountingYear.Year,
                DateInventaire = i.DateInventaire,
                Description = i.Description,
                Statut = (int)i.Statut,
                StatutLibelle = i.Statut == Domain.Entites.InventaireStatut.Brouillon ? "Brouillon" :
                               i.Statut == Domain.Entites.InventaireStatut.Valide ? "Validé" : "Clôturé"
            })
            .AsQueryable();

        // Filtre par exercice comptable
        if (query.AccountingYearId.HasValue)
        {
            inventairesQuery = inventairesQuery.Where(i => i.AccountingYearId == query.AccountingYearId.Value);
        }

        // Recherche
        if (!string.IsNullOrEmpty(query.SearchKeyword))
        {
            inventairesQuery = inventairesQuery.Where(i =>
                i.Num.ToString().Contains(query.SearchKeyword) ||
                (i.Description != null && i.Description.Contains(query.SearchKeyword)));
        }

        // Tri
        if (query.SortOrder != null && query.SortProperty != null)
        {
            _logger.LogInformation("Sorting inventaires column: {column} order: {order}", query.SortProperty, query.SortOrder);
            inventairesQuery = ApplySorting(inventairesQuery, query.SortProperty, query.SortOrder);
        }

        var pagedInventaires = await PagedList<InventaireResponse>.ToPagedListAsync(
            inventairesQuery,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        _logger.LogEntitiesFetched(nameof(Domain.Entites.Inventaire), pagedInventaires.Items.Count);
        return pagedInventaires;
    }

    private IQueryable<InventaireResponse> ApplySorting(
        IQueryable<InventaireResponse> query,
        string sortProperty,
        string sortOrder)
    {
        return (sortProperty, sortOrder) switch
        {
            (_numColumn, SortConstants.Ascending) => query.OrderBy(i => i.Num),
            (_numColumn, SortConstants.Descending) => query.OrderByDescending(i => i.Num),
            (_dateColumn, SortConstants.Ascending) => query.OrderBy(i => i.DateInventaire),
            (_dateColumn, SortConstants.Descending) => query.OrderByDescending(i => i.DateInventaire),
            (_statutColumn, SortConstants.Ascending) => query.OrderBy(i => i.Statut),
            (_statutColumn, SortConstants.Descending) => query.OrderByDescending(i => i.Statut),
            _ => query
        };
    }
}

