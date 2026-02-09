using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.GetCompteBancaire;

public class GetCompteBancaireQueryHandler(
    SalesContext _context,
    ILogger<GetCompteBancaireQueryHandler> _logger)
    : IRequestHandler<GetCompteBancaireQuery, Result<CompteBancaireResponse>>
{
    public async Task<Result<CompteBancaireResponse>> Handle(GetCompteBancaireQuery query, CancellationToken cancellationToken)
    {
        var compte = await _context.CompteBancaire
            .AsNoTracking()
            .Include(c => c.Banque)
            .Where(c => c.Id == query.Id)
            .Select(c => new CompteBancaireResponse
            {
                Id = c.Id,
                BanqueId = c.BanqueId,
                BanqueNom = c.Banque != null ? c.Banque.Nom : null,
                CodeEtablissement = c.CodeEtablissement,
                CodeAgence = c.CodeAgence,
                NumeroCompte = c.NumeroCompte,
                CleRib = c.CleRib,
                Libelle = c.Libelle
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (compte == null)
        {
            return Result.Fail("compte_bancaire_not_found");
        }

        return Result.Ok(compte);
    }
}
