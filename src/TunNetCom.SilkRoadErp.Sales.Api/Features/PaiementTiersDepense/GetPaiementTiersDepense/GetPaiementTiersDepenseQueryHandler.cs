using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementTiersDepense;

public class GetPaiementTiersDepenseQueryHandler(SalesContext _context, ILogger<GetPaiementTiersDepenseQueryHandler> _logger)
    : IRequestHandler<GetPaiementTiersDepenseQuery, Result<PaiementTiersDepenseResponse>>
{
    public async Task<Result<PaiementTiersDepenseResponse>> Handle(GetPaiementTiersDepenseQuery query, CancellationToken cancellationToken)
    {
        var entity = await _context.PaiementTiersDepense
            .AsNoTracking()
            .Include(p => p.TiersDepenseFonctionnement)
            .Include(p => p.AccountingYear)
            .Include(p => p.Banque)
            .Include(p => p.FactureDepenses)
            .Where(p => p.Id == query.Id)
            .Select(p => new PaiementTiersDepenseResponse
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
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            _logger.LogWarning("PaiementTiersDepense not found: Id {Id}", query.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        return Result.Ok(entity);
    }
}
