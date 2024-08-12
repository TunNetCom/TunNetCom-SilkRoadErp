using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.GetProvider;
public class GetProviderQueryHandler(SalesContext _context, ILogger<GetProviderQueryHandler> _logger) : IRequestHandler<GetProviderQuery, PagedList<ProviderResponse>>
{
    public async Task<PagedList<ProviderResponse>> Handle(GetProviderQuery getProviderQuery, CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(Fournisseur), getProviderQuery.PageNumber, getProviderQuery.PageSize);

        IQueryable<ProviderResponse> ProvidersQuery = _context.Fournisseur.Select(t =>
            new ProviderResponse
            {
                Id = t.Id,                          
                Nom = t.Nom,
                Tel = t.Tel,
                Fax = t.Fax,
                Matricule = t.Matricule,
                Code = t.Code,
                CodeCat = t.CodeCat,
                EtbSec = t.EtbSec,
                Mail = t.Mail,
                MailDeux = t.MailDeux,
                Constructeur = t.Constructeur,
                Adresse = t.Adresse
            })
            .AsQueryable();

        if (!string.IsNullOrEmpty(getProviderQuery.SearchKeyword))
        {
            ProvidersQuery = ProvidersQuery.Where(
                p => p.Nom.Contains(getProviderQuery.SearchKeyword)
                || p.Tel!.Contains(getProviderQuery.SearchKeyword)
                || p.Fax!.Contains(getProviderQuery.SearchKeyword)
                || p.Matricule!.Contains(getProviderQuery.SearchKeyword)
                || p.Code!.Contains(getProviderQuery.SearchKeyword)
                || p.CodeCat!.Contains(getProviderQuery.SearchKeyword)
                || p.EtbSec!.Contains(getProviderQuery.SearchKeyword)
                || p.Mail!.Contains(getProviderQuery.SearchKeyword)
                || p.MailDeux!.Contains(getProviderQuery.SearchKeyword)
                || p.Constructeur.Equals(getProviderQuery.SearchKeyword)
                || p.Adresse!.Contains(getProviderQuery.SearchKeyword));
        }

        PagedList<ProviderResponse> pagedProviders = await PagedList<ProviderResponse>.ToPagedListAsync(
            ProvidersQuery,
            getProviderQuery.PageNumber,
            getProviderQuery.PageSize,
            cancellationToken);

        _logger.LogEntitiesFetched(nameof(Fournisseur), pagedProviders.Count);
        return pagedProviders;
    }
}

