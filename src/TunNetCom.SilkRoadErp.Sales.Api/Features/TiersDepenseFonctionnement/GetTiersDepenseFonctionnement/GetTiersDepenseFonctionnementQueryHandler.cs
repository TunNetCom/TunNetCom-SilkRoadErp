using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.GetTiersDepenseFonctionnement;

public class GetTiersDepenseFonctionnementQueryHandler(SalesContext _context, ILogger<GetTiersDepenseFonctionnementQueryHandler> _logger)
    : IRequestHandler<GetTiersDepenseFonctionnementQuery, PagedList<TiersDepenseFonctionnementResponse>>
{
    public async Task<PagedList<TiersDepenseFonctionnementResponse>> Handle(GetTiersDepenseFonctionnementQuery query, CancellationToken cancellationToken)
    {
        IQueryable<TiersDepenseFonctionnementResponse> q = _context.TiersDepenseFonctionnement
            .AsNoTracking()
            .Select(t => new TiersDepenseFonctionnementResponse
            {
                Id = t.Id,
                Nom = t.Nom,
                Tel = t.Tel,
                Adresse = t.Adresse,
                Matricule = t.Matricule,
                Code = t.Code,
                CodeCat = t.CodeCat,
                EtbSec = t.EtbSec,
                Mail = t.Mail
            });

        if (!string.IsNullOrWhiteSpace(query.SearchKeyword))
        {
            var kw = query.SearchKeyword.Trim();
            q = q.Where(t =>
                (t.Nom != null && t.Nom.Contains(kw)) ||
                (t.Tel != null && t.Tel.Contains(kw)) ||
                (t.Adresse != null && t.Adresse.Contains(kw)) ||
                (t.Mail != null && t.Mail.Contains(kw)) ||
                (t.Code != null && t.Code.Contains(kw)) ||
                (t.Matricule != null && t.Matricule.Contains(kw)));
        }


        var paged = await PagedList<TiersDepenseFonctionnementResponse>.ToPagedListAsync(q, query.PageNumber, query.PageSize, cancellationToken);
        _logger.LogInformation("GetTiersDepenseFonctionnement returned {Count} items", paged.Items.Count);
        return paged;
    }
}
