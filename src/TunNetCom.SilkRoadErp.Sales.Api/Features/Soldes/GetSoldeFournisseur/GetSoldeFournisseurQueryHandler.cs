using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeFournisseur;

public class GetSoldeFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetSoldeFournisseurQueryHandler> _logger,
    IMediator _mediator,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetSoldeFournisseurQuery, Result<SoldeFournisseurResponse>>
{
    public const string DocumentTypeAvoirFinancier = "AvoirFinancier";

    public async Task<Result<SoldeFournisseurResponse>> Handle(GetSoldeFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetSoldeFournisseurQuery called for FournisseurId {FournisseurId}", query.FournisseurId);

        var fournisseur = await _context.Fournisseur
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == query.FournisseurId, cancellationToken);

        if (fournisseur == null)
            return Result.Fail("fournisseur_not_found");

        var accountingYearId = query.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYear = await _context.AccountingYear
                .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);
            if (activeYear == null)
                return Result.Fail("no_active_accounting_year");
            accountingYearId = activeYear.Id;
        }

        var yearId = accountingYearId.Value;
        var fournisseurId = query.FournisseurId;

        var appParams = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        var facturesFournisseur = await _context.FactureFournisseur
            .Where(f => f.IdFournisseur == fournisseurId && f.AccountingYearId == yearId)
            .Include(f => f.BonDeReception)
                .ThenInclude(br => br.LigneBonReception)
            .ToListAsync(cancellationToken);

        var bonsReception = await _context.BonDeReception
            .Where(b => b.IdFournisseur == fournisseurId && b.AccountingYearId == yearId && b.NumFactureFournisseur == null)
            .Include(b => b.LigneBonReception)
            .ToListAsync(cancellationToken);

        var facturesAvoir = await _context.FactureAvoirFournisseur
            .Where(fa => fa.IdFournisseur == fournisseurId && fa.AccountingYearId == yearId)
            .Include(fa => fa.AvoirFournisseur)
                .ThenInclude(a => a.LigneAvoirFournisseur)
            .ToListAsync(cancellationToken);

        var avoirsFinanciers = await (from af in _context.AvoirFinancierFournisseurs
                                      join ff in _context.FactureFournisseur on af.NumFactureFournisseur equals ff.Num
                                      where ff.IdFournisseur == fournisseurId && ff.AccountingYearId == yearId
                                      select new DocumentSoldeFournisseur
                                      {
                                          Type = DocumentTypeAvoirFinancier,
                                          Id = af.Id,
                                          Numero = af.NumSurPage,
                                          Date = af.Date,
                                          Montant = af.TotTtc
                                      })
            .ToListAsync(cancellationToken);

        var retours = await _context.RetourMarchandiseFournisseur
            .Where(r => r.IdFournisseur == fournisseurId && r.AccountingYearId == yearId)
            .Include(r => r.LigneRetourMarchandiseFournisseur)
            .ToListAsync(cancellationToken);

        var paiementsData = await _context.PaiementFournisseur
            .Where(p => p.FournisseurId == fournisseurId && p.AccountingYearId == yearId)
            .OrderByDescending(p => p.DatePaiement)
            .Select(p => new
            {
                p.Id,
                p.NumeroTransactionBancaire,
                p.DatePaiement,
                p.Montant,
                MethodePaiement = p.MethodePaiement.ToString(),
                FactureNums = p.FactureFournisseurs.Select(pf => pf.FactureFournisseur.Num).ToList()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var factureNums = facturesFournisseur.Select(f => f.Num).ToList();
        var retenues = await _context.RetenueSourceFournisseur
            .AsNoTracking()
            .Where(r => factureNums.Contains(r.NumFactureFournisseur))
            .Select(r => new { r.NumFactureFournisseur, r.MontantApresRetenu, r.TauxRetenu })
            .ToListAsync(cancellationToken);
        var retenueByNumFacture = retenues.ToDictionary(r => r.NumFactureFournisseur);

        var avoirsFinanciersByNum = await (from af in _context.AvoirFinancierFournisseurs
                                           join ff in _context.FactureFournisseur on af.NumFactureFournisseur equals ff.Num
                                           where ff.IdFournisseur == fournisseurId && ff.AccountingYearId == yearId
                                           group af by af.NumFactureFournisseur into g
                                           select new { Num = g.Key!.Value, Total = g.Sum(af => af.TotTtc) })
            .ToListAsync(cancellationToken);
        var avoirsFinanciersByNumFacture = avoirsFinanciersByNum.ToDictionary(x => x.Num, x => x.Total);

        var avoirsFinanciersList = await _context.AvoirFinancierFournisseurs
            .AsNoTracking()
            .Where(af => af.NumFactureFournisseur != null && factureNums.Contains(af.NumFactureFournisseur.Value))
            .Select(af => new { af.NumFactureFournisseur!.Value, af.NumSurPage, af.TotTtc })
            .ToListAsync(cancellationToken);
        var avoirsFinanciersDetailByNum = avoirsFinanciersList
            .GroupBy(x => x.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => new AvoirRattacheSolde { Type = "AvoirFinancier", Numero = x.NumSurPage, Montant = x.TotTtc }).ToList());

        var factureAvoirTotalByFactureId = facturesAvoir
            .Where(fa => fa.FactureFournisseurId.HasValue)
            .GroupBy(fa => fa.FactureFournisseurId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(fa => fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc)));

        var totalFacturesAvoirFournisseur = facturesAvoir.Sum(fa => fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc));

        var facturesAvoirDetailByFactureId = facturesAvoir
            .Where(fa => fa.FactureFournisseurId.HasValue)
            .GroupBy(fa => fa.FactureFournisseurId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(fa => new AvoirRattacheSolde
            {
                Type = "FactureAvoir",
                Numero = fa.NumFactureAvoirFourSurPage,
                Montant = fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc)
            }).ToList());

        var defaultTauxRetenu = fournisseur.TauxRetenu ?? await _financialParametersService.GetPourcentageRetenuAsync(appParams.Value.PourcentageRetenu, cancellationToken);

        var documents = new List<DocumentSoldeFournisseur>();
        var montantTtcParFactureNum = new Dictionary<int, decimal>();

        foreach (var f in facturesFournisseur)
        {
            var sumLigneBr = f.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc);
            var avoirFinancier = avoirsFinanciersByNumFacture.GetValueOrDefault(f.Num, 0m);
            var factureAvoirTotal = factureAvoirTotalByFactureId.GetValueOrDefault(f.Id, 0m);
            if (facturesFournisseur.Count == 1 && factureAvoirTotal == 0 && totalFacturesAvoirFournisseur > 0)
                factureAvoirTotal = totalFacturesAvoirFournisseur;
            var tauxRetenu = retenueByNumFacture.GetValueOrDefault(f.Num)?.TauxRetenu ?? defaultTauxRetenu;

            var (montantAvecRetenue, formuleMontant) = SoldeFournisseurCalculator.ComputeMontantEtFormuleFactureFournisseur(
                sumLigneBr, timbre, avoirFinancier, factureAvoirTotal, tauxRetenu);

            var avoirsRattaches = new List<AvoirRattacheSolde>();
            if (avoirsFinanciersDetailByNum.TryGetValue(f.Num, out var listAf))
                avoirsRattaches.AddRange(listAf);
            if (facturesAvoirDetailByFactureId.TryGetValue(f.Id, out var listFa))
                avoirsRattaches.AddRange(listFa);
            else if (facturesFournisseur.Count == 1 && facturesAvoir.Count > 0)
                avoirsRattaches.AddRange(facturesAvoir.Select(fa => new AvoirRattacheSolde
                {
                    Type = "FactureAvoir",
                    Numero = fa.NumFactureAvoirFourSurPage,
                    Montant = fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc)
                }));

            documents.Add(new DocumentSoldeFournisseur
            {
                Type = "FactureFournisseur",
                Id = f.Id,
                Numero = f.Num,
                Date = f.Date,
                Montant = montantAvecRetenue,
                FormuleMontant = formuleMontant,
                AvoirsRattaches = avoirsRattaches.Count > 0 ? avoirsRattaches : null
            });
            montantTtcParFactureNum[f.Num] = montantAvecRetenue;
        }

        var totalFactures = facturesFournisseur.Sum(f =>
            f.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc) + timbre);

        var totalMontantsApresRetenue = montantTtcParFactureNum.Values.Sum();
        var totalBonsReceptionNonFactures = bonsReception.Sum(b => b.LigneBonReception.Sum(l => l.TotTtc));
        var totalFacturesAvoir = facturesAvoir.Sum(fa => fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc));
        var totalAvoirsFinanciers = avoirsFinanciers.Sum(d => d.Montant);
        var totalPaiements = paiementsData.Sum(p => p.Montant);
        var solde = SoldeFournisseurCalculator.ComputeSoldeAvecRetenue(totalMontantsApresRetenue, totalBonsReceptionNonFactures, totalPaiements);

        foreach (var b in bonsReception)
        {
            documents.Add(new DocumentSoldeFournisseur
            {
                Type = "BonDeReception",
                Id = b.Id,
                Numero = b.Num,
                Date = b.Date,
                Montant = b.LigneBonReception.Sum(l => l.TotTtc)
            });
        }

        foreach (var fa in facturesAvoir)
        {
            var montant = fa.AvoirFournisseur.SelectMany(a => a.LigneAvoirFournisseur).Sum(l => l.TotTtc);
            documents.Add(new DocumentSoldeFournisseur
            {
                Type = "FactureAvoir",
                Id = fa.Id,
                Numero = fa.NumFactureAvoirFourSurPage,
                Date = fa.Date,
                Montant = montant
            });
        }

        documents.AddRange(avoirsFinanciers);

        foreach (var r in retours)
        {
            documents.Add(new DocumentSoldeFournisseur
            {
                Type = "RetourMarchandiseFournisseur",
                Id = r.Id,
                Numero = r.Num,
                Date = r.Date,
                Montant = r.LigneRetourMarchandiseFournisseur.Sum(l => l.TotTtc)
            });
        }

        var paiements = paiementsData.Select(p =>
        {
            var facturesRattachees = p.FactureNums.Select(numFacture => new FactureRattacheeSolde
            {
                Numero = numFacture,
                MontantTtc = montantTtcParFactureNum.GetValueOrDefault(numFacture, 0m)
            }).ToList();
            return new PaiementSoldeFournisseur
            {
                Id = p.Id,
                NumeroTransactionBancaire = p.NumeroTransactionBancaire,
                DatePaiement = p.DatePaiement,
                Montant = p.Montant,
                MethodePaiement = p.MethodePaiement,
                Factures = facturesRattachees
            };
        }).ToList();

        var response = new SoldeFournisseurResponse
        {
            FournisseurId = query.FournisseurId,
            FournisseurNom = fournisseur.Nom,
            AccountingYearId = yearId,
            TotalFactures = totalFactures,
            TotalBonsReceptionNonFactures = totalBonsReceptionNonFactures,
            TotalFacturesAvoir = totalFacturesAvoir,
            TotalAvoirsFinanciers = totalAvoirsFinanciers,
            TotalPaiements = totalPaiements,
            Solde = solde,
            Documents = documents.OrderByDescending(d => d.Date).ToList(),
            Paiements = paiements
        };

        return Result.Ok(response);
    }
}
