using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.GetCompteBancaires;

public class GetCompteBancairesQueryHandler(
    SalesContext _context,
    ILogger<GetCompteBancairesQueryHandler> _logger)
    : IRequestHandler<GetCompteBancairesQuery, Result<List<CompteBancaireResponse>>>
{
    public async Task<Result<List<CompteBancaireResponse>>> Handle(GetCompteBancairesQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetCompteBancairesQuery called");

        var list = await _context.CompteBancaire
            .AsNoTracking()
            .Include(c => c.Banque)
            .OrderBy(c => c.Banque!.Nom)
            .ThenBy(c => c.NumeroCompte)
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
            .ToListAsync(cancellationToken);

        return Result.Ok(list);
    }
}
