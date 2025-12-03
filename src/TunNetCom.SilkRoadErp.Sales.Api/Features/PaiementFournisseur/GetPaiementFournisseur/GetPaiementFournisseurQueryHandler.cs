using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementFournisseur;

public class GetPaiementFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetPaiementFournisseurQueryHandler> _logger)
    : IRequestHandler<GetPaiementFournisseurQuery, Result<PaiementFournisseurResponse>>
{
    public async Task<Result<PaiementFournisseurResponse>> Handle(GetPaiementFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching PaiementFournisseur with Id {Id}", query.Id);

        var paiement = await _context.PaiementFournisseur
            .AsNoTracking()
            .Where(p => p.Id == query.Id)
            .Select(p => new PaiementFournisseurResponse
            {
                Id = p.Id,
                Numero = p.Numero,
                FournisseurId = p.FournisseurId,
                FournisseurNom = p.Fournisseur.Nom,
                AccountingYearId = p.AccountingYearId,
                Montant = p.Montant,
                DatePaiement = p.DatePaiement,
                MethodePaiement = p.MethodePaiement.ToString(),
                FactureFournisseurId = p.FactureFournisseurId,
                BonDeReceptionId = p.BonDeReceptionId,
                NumeroChequeTraite = p.NumeroChequeTraite,
                BanqueId = p.BanqueId,
                BanqueNom = p.Banque != null ? p.Banque.Nom : null,
                DateEcheance = p.DateEcheance,
                Commentaire = p.Commentaire,
                RibCodeEtab = p.RibCodeEtab,
                RibCodeAgence = p.RibCodeAgence,
                RibNumeroCompte = p.RibNumeroCompte,
                RibCle = p.RibCle,
                DateModification = p.DateModification
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (paiement == null)
        {
            _logger.LogWarning("PaiementFournisseur with Id {Id} not found", query.Id);
            return Result.Fail("paiement_fournisseur_not_found");
        }

        _logger.LogInformation("PaiementFournisseur with Id {Id} fetched successfully", query.Id);
        return Result.Ok(paiement);
    }
}

