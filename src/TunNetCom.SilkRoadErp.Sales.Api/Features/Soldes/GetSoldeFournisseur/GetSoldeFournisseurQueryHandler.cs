using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeFournisseur;

public class GetSoldeFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetSoldeFournisseurQueryHandler> _logger)
    : IRequestHandler<GetSoldeFournisseurQuery, Result<SoldeFournisseurResponse>>
{
    public async Task<Result<SoldeFournisseurResponse>> Handle(GetSoldeFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetSoldeFournisseurQuery called for FournisseurId {FournisseurId}", query.FournisseurId);

        var fournisseur = await _context.Fournisseur
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == query.FournisseurId, cancellationToken);

        if (fournisseur == null)
        {
            return Result.Fail("fournisseur_not_found");
        }

        var accountingYearId = query.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYear = await _context.AccountingYear
                .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);
            if (activeYear == null)
            {
                return Result.Fail("no_active_accounting_year");
            }
            accountingYearId = activeYear.Id;
        }

        // Calculate total from factures fournisseur (factured BRs)
        var totalFactures = await _context.FactureFournisseur
            .Where(f => f.IdFournisseur == query.FournisseurId && f.AccountingYearId == accountingYearId.Value)
            .SelectMany(f => f.BonDeReception)
            .SelectMany(br => br.LigneBonReception)
            .SumAsync(l => l.TotTtc, cancellationToken);

        // Calculate total from non-factured BRs
        var totalBonsReceptionNonFactures = await _context.BonDeReception
            .Where(b => b.IdFournisseur == query.FournisseurId 
                && b.AccountingYearId == accountingYearId.Value 
                && b.NumFactureFournisseur == null)
            .SelectMany(br => br.LigneBonReception)
            .SumAsync(l => l.TotTtc, cancellationToken);

        // Calculate total payments
        var totalPaiements = await _context.PaiementFournisseur
            .Where(p => p.FournisseurId == query.FournisseurId && p.AccountingYearId == accountingYearId.Value)
            .SumAsync(p => p.Montant, cancellationToken);

        var solde = totalFactures + totalBonsReceptionNonFactures - totalPaiements;

        // Get documents
        var documents = new List<DocumentSoldeFournisseur>();

        // Add factures fournisseur
        var factures = await _context.FactureFournisseur
            .Where(f => f.IdFournisseur == query.FournisseurId && f.AccountingYearId == accountingYearId.Value)
            .Select(f => new DocumentSoldeFournisseur
            {
                Type = "FactureFournisseur",
                Id = f.Id,
                Numero = f.Num,
                Date = f.Date,
                Montant = f.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc)
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(factures);

        // Add non-factured BRs
        var bonsReception = await _context.BonDeReception
            .Where(b => b.IdFournisseur == query.FournisseurId 
                && b.AccountingYearId == accountingYearId.Value 
                && b.NumFactureFournisseur == null)
            .Select(b => new DocumentSoldeFournisseur
            {
                Type = "BonDeReception",
                Id = b.Id,
                Numero = b.Num,
                Date = b.Date,
                Montant = b.LigneBonReception.Sum(l => l.TotTtc)
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(bonsReception);

        // Get payments
        var paiements = await _context.PaiementFournisseur
            .Where(p => p.FournisseurId == query.FournisseurId && p.AccountingYearId == accountingYearId.Value)
            .Select(p => new PaiementSoldeFournisseur
            {
                Id = p.Id,
                Numero = p.Numero,
                DatePaiement = p.DatePaiement,
                Montant = p.Montant,
                MethodePaiement = p.MethodePaiement.ToString()
            })
            .OrderByDescending(p => p.DatePaiement)
            .ToListAsync(cancellationToken);

        var response = new SoldeFournisseurResponse
        {
            FournisseurId = query.FournisseurId,
            FournisseurNom = fournisseur.Nom,
            AccountingYearId = accountingYearId.Value,
            TotalFactures = totalFactures,
            TotalBonsReceptionNonFactures = totalBonsReceptionNonFactures,
            TotalPaiements = totalPaiements,
            Solde = solde,
            Documents = documents.OrderByDescending(d => d.Date).ToList(),
            Paiements = paiements
        };

        return Result.Ok(response);
    }
}

