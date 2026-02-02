using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementsTiersDepense;

public class GetPaiementsTiersDepenseQueryHandler(SalesContext _context, ILogger<GetPaiementsTiersDepenseQueryHandler> _logger)
    : IRequestHandler<GetPaiementsTiersDepenseQuery, Result<PagedList<PaiementTiersDepenseResponse>>>
{
    public async Task<Result<PagedList<PaiementTiersDepenseResponse>>> Handle(GetPaiementsTiersDepenseQuery query, CancellationToken cancellationToken)
    {
        var q = _context.PaiementTiersDepense
            .AsNoTracking()
            .Include(p => p.TiersDepenseFonctionnement)
            .Include(p => p.AccountingYear)
            .Include(p => p.Banque)
            .Include(p => p.FactureDepenses)
            .AsQueryable();

        if (query.TiersDepenseFonctionnementId.HasValue)
            q = q.Where(p => p.TiersDepenseFonctionnementId == query.TiersDepenseFonctionnementId.Value);

        if (query.AccountingYearId.HasValue)
            q = q.Where(p => p.AccountingYearId == query.AccountingYearId.Value);

        q = q.OrderByDescending(p => p.DatePaiement).ThenByDescending(p => p.Id);

        var projected = q.Select(p => new PaiementTiersDepenseResponse
        {
            Id = p.Id,
            NumeroTransactionBancaire = p.NumeroTransactionBancaire,
            TiersDepenseFonctionnementId = p.TiersDepenseFonctionnementId,
            TiersDepenseFonctionnementNom = p.TiersDepenseFonctionnement.Nom,
            AccountingYearId = p.AccountingYearId,
            AccountingYear = p.AccountingYear.Year,
            Montant = p.Montant,
            DatePaiement = p.DatePaiement,
            MethodePaiement = p.MethodePaiement.ToString(),
            FactureDepenseIds = p.FactureDepenses.Select(fd => fd.FactureDepenseId).ToList(),
            NumeroChequeTraite = p.NumeroChequeTraite,
            BanqueId = p.BanqueId,
            BanqueNom = p.Banque != null ? p.Banque.Nom : null,
            DateEcheance = p.DateEcheance,
            Commentaire = p.Commentaire,
            RibCodeEtab = p.RibCodeEtab,
            RibCodeAgence = p.RibCodeAgence,
            RibNumeroCompte = p.RibNumeroCompte,
            RibCle = p.RibCle,
            Mois = p.Mois,
            DateModification = p.DateModification
        });

        var paged = await PagedList<PaiementTiersDepenseResponse>.ToPagedListAsync(
            projected,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        _logger.LogInformation("GetPaiementsTiersDepense returned {Count} items", paged.Items.Count);
        return Result.Ok(paged);
    }
}
