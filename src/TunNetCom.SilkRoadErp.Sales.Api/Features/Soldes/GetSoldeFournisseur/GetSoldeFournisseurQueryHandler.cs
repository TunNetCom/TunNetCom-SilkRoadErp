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

        // Get all retenues for factures fournisseur of this fournisseur
        var retenues = await _context.RetenueSourceFournisseur
            .Where(r => _context.FactureFournisseur
                .Where(f => f.IdFournisseur == query.FournisseurId && f.AccountingYearId == accountingYearId.Value)
                .Select(f => f.Num)
                .Contains(r.NumFactureFournisseur))
            .ToDictionaryAsync(r => r.NumFactureFournisseur, cancellationToken);

        // Calculate total from factures fournisseur (factured BRs)
        // For factures with retenue, use montant après retenue; otherwise use TotTtc
        var facturesFournisseur = await _context.FactureFournisseur
            .Where(f => f.IdFournisseur == query.FournisseurId && f.AccountingYearId == accountingYearId.Value)
            .Include(f => f.BonDeReception)
                .ThenInclude(br => br.LigneBonReception)
            .ToListAsync(cancellationToken);

        var totalFactures = facturesFournisseur.Sum(f =>
        {
            if (retenues.TryGetValue(f.Num, out var retenue))
            {
                // Use montant après retenue
                return retenue.MontantApresRetenu;
            }
            else
            {
                // Use TotTtc for factures without retenue
                return f.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc);
            }
        });

        // Calculate total from non-factured BRs
        var totalBonsReceptionNonFactures = await _context.BonDeReception
            .Where(b => b.IdFournisseur == query.FournisseurId 
                && b.AccountingYearId == accountingYearId.Value 
                && b.NumFactureFournisseur == null)
            .SelectMany(br => br.LigneBonReception)
            .SumAsync(l => l.TotTtc, cancellationToken);

        // Calculate total from factures avoir (via AvoirFournisseur linked to FactureAvoirFournisseur)
        var totalFacturesAvoir = await _context.FactureAvoirFournisseur
            .Where(fa => fa.IdFournisseur == query.FournisseurId && fa.AccountingYearId == accountingYearId.Value)
            .SelectMany(fa => fa.AvoirFournisseur)
            .SelectMany(a => a.LigneAvoirFournisseur)
            .SumAsync(l => l.TotTtc, cancellationToken);

        // Calculate total payments
        var totalPaiements = await _context.PaiementFournisseur
            .Where(p => p.FournisseurId == query.FournisseurId && p.AccountingYearId == accountingYearId.Value)
            .SumAsync(p => p.Montant, cancellationToken);

        var solde = totalFactures + totalBonsReceptionNonFactures - totalFacturesAvoir - totalPaiements;

        // Get documents
        var documents = new List<DocumentSoldeFournisseur>();

        // Add factures fournisseur (factures already loaded above)
        var documentsFactures = facturesFournisseur.Select(f =>
        {
            // Use montant après retenue if retenue exists, otherwise use TotTtc
            decimal montant;
            if (retenues.TryGetValue(f.Num, out var retenue))
            {
                montant = retenue.MontantApresRetenu;
            }
            else
            {
                montant = f.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc);
            }

            return new DocumentSoldeFournisseur
            {
                Type = "FactureFournisseur",
                Id = f.Id,
                Numero = f.Num,
                Date = f.Date,
                Montant = montant
            };
        }).ToList();
        documents.AddRange(documentsFactures);

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

        // Add factures avoir
        var facturesAvoir = await _context.FactureAvoirFournisseur
            .Where(fa => fa.IdFournisseur == query.FournisseurId && fa.AccountingYearId == accountingYearId.Value)
            .Select(fa => new DocumentSoldeFournisseur
            {
                Type = "FactureAvoir",
                Id = fa.Id,
                Numero = fa.NumFactureAvoirFourSurPage,
                Date = fa.Date,
                Montant = fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc)
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(facturesAvoir);

        // Add retours marchandise fournisseur (note informative, doesn't affect balance calculation)
        var retours = await _context.RetourMarchandiseFournisseur
            .Where(r => r.IdFournisseur == query.FournisseurId && r.AccountingYearId == accountingYearId.Value)
            .Select(r => new DocumentSoldeFournisseur
            {
                Type = "RetourMarchandiseFournisseur",
                Id = r.Id,
                Numero = r.Num,
                Date = r.Date,
                Montant = r.LigneRetourMarchandiseFournisseur.Sum(l => l.TotTtc)
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(retours);

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
            TotalFacturesAvoir = totalFacturesAvoir,
            TotalPaiements = totalPaiements,
            Solde = solde,
            Documents = documents.OrderByDescending(d => d.Date).ToList(),
            Paiements = paiements
        };

        return Result.Ok(response);
    }
}

