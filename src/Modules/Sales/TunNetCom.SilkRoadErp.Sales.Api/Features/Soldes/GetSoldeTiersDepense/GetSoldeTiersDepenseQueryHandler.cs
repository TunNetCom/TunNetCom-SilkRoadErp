using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeTiersDepense;

public class GetSoldeTiersDepenseQueryHandler(SalesContext _context, ILogger<GetSoldeTiersDepenseQueryHandler> _logger)
    : IRequestHandler<GetSoldeTiersDepenseQuery, Result<SoldeTiersDepenseResponse>>
{
    public async Task<Result<SoldeTiersDepenseResponse>> Handle(GetSoldeTiersDepenseQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetSoldeTiersDepenseQuery called for TiersDepenseFonctionnementId {Id}", query.TiersDepenseFonctionnementId);

        var tiers = await _context.TiersDepenseFonctionnement
            .AsNoTracking()
            .Where(t => t.Id == query.TiersDepenseFonctionnementId)
            .Select(t => new { t.Id, t.Nom })
            .FirstOrDefaultAsync(cancellationToken);

        if (tiers == null)
        {
            _logger.LogWarning("TiersDepenseFonctionnement not found: Id {Id}", query.TiersDepenseFonctionnementId);
            return Result.Fail(EntityNotFound.Error());
        }

        var accountingYearId = query.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYear = await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);
            if (activeYear == null)
            {
                return Result.Fail("no_active_accounting_year");
            }
            accountingYearId = activeYear.Id;
        }

        var totalFacturesDepense = await _context.FactureDepense
            .Where(f => f.IdTiersDepenseFonctionnement == query.TiersDepenseFonctionnementId && f.AccountingYearId == accountingYearId.Value)
            .SumAsync(f => f.MontantTotal, cancellationToken);

        var totalPaiements = await _context.PaiementTiersDepense
            .Where(p => p.TiersDepenseFonctionnementId == query.TiersDepenseFonctionnementId && p.AccountingYearId == accountingYearId.Value)
            .SumAsync(p => p.Montant, cancellationToken);

        var solde = totalFacturesDepense - totalPaiements;

        var documents = await _context.FactureDepense
            .Where(f => f.IdTiersDepenseFonctionnement == query.TiersDepenseFonctionnementId && f.AccountingYearId == accountingYearId.Value)
            .OrderByDescending(f => f.Date)
            .Select(f => new DocumentSoldeTiersDepense
            {
                Type = "FactureDepense",
                Id = f.Id,
                Numero = f.Num,
                Date = f.Date,
                Montant = f.MontantTotal,
                Description = f.Description
            })
            .ToListAsync(cancellationToken);

        var paiements = await _context.PaiementTiersDepense
            .Where(p => p.TiersDepenseFonctionnementId == query.TiersDepenseFonctionnementId && p.AccountingYearId == accountingYearId.Value)
            .Include(p => p.FactureDepenses)
            .ThenInclude(pf => pf.FactureDepense)
            .OrderByDescending(p => p.DatePaiement)
            .Select(p => new PaiementSoldeTiersDepense
            {
                Id = p.Id,
                NumeroTransactionBancaire = p.NumeroTransactionBancaire,
                DatePaiement = p.DatePaiement,
                Montant = p.Montant,
                MethodePaiement = p.MethodePaiement.ToString(),
                Factures = p.FactureDepenses.Select(pf => new FactureDepenseRattacheeSolde
                {
                    Numero = pf.FactureDepense.Num,
                    MontantTtc = pf.FactureDepense.MontantTotal
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var response = new SoldeTiersDepenseResponse
        {
            TiersDepenseFonctionnementId = tiers.Id,
            TiersDepenseFonctionnementNom = tiers.Nom,
            AccountingYearId = accountingYearId.Value,
            TotalFacturesDepense = totalFacturesDepense,
            TotalPaiements = totalPaiements,
            Solde = solde,
            Documents = documents,
            Paiements = paiements
        };

        return Result.Ok(response);
    }
}
