using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeClient;

public class GetSoldeClientQueryHandler(
    SalesContext _context,
    ILogger<GetSoldeClientQueryHandler> _logger,
    IMediator mediator,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetSoldeClientQuery, Result<SoldeClientResponse>>
{
    public async Task<Result<SoldeClientResponse>> Handle(GetSoldeClientQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetSoldeClientQuery called for ClientId {ClientId}", query.ClientId);

        var client = await _context.Client
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.ClientId, cancellationToken);

        if (client == null)
        {
            return Result.Fail("client_not_found");
        }

        var accountingYearId = query.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
            if (!activeYearId.HasValue)
            {
                return Result.Fail("no_active_accounting_year");
            }
            accountingYearId = activeYearId.Value;
        }

        // Get timbre from financial parameters service
        var appParams = await mediator.Send(new GetAppParametersQuery());
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        // Get all retenues for factures of this client
        var retenues = await _context.RetenueSourceClient
            .Where(r => _context.Facture
                .Where(f => f.IdClient == query.ClientId && f.AccountingYearId == accountingYearId.Value)
                .Select(f => f.Num)
                .Contains(r.NumFacture))
            .ToDictionaryAsync(r => r.NumFacture, cancellationToken);

        // Calculate total from factures (factured BLs): use calculator for retenue / NetPayer + timbre
        var factures = await _context.Facture
            .Where(f => f.IdClient == query.ClientId && f.AccountingYearId == accountingYearId.Value)
            .Include(f => f.BonDeLivraison)
                .ThenInclude(b => b.LigneBl)
            .ToListAsync(cancellationToken);

        var montantApresRetenuByNum = retenues.ToDictionary(r => r.Key, r => r.Value.MontantApresRetenu);
        var totalFactures = factures.Sum(f =>
            SoldeClientCalculator.ComputeMontantFactureClient(
                f.Num, montantApresRetenuByNum,
                f.BonDeLivraison.Sum(b => b.NetPayer), timbre));

        // Calculate total from non-factured BLs
        var totalBonsLivraisonNonFactures = await _context.BonDeLivraison
            .Where(b => b.ClientId == query.ClientId 
                && b.AccountingYearId == accountingYearId.Value 
                && b.NumFacture == null)
            .SumAsync(b => b.NetPayer, cancellationToken);

        // Calculate total from avoirs (direct avoirs not linked to FactureAvoirClient)
        var totalAvoirs = await _context.Avoirs
            .Where(a => a.ClientId == query.ClientId 
                && a.AccountingYearId == accountingYearId.Value 
                && a.NumFactureAvoirClient == null)
            .SelectMany(a => a.LigneAvoirs)
            .SumAsync(l => l.TotTtc, cancellationToken);

        // Calculate total from factures avoir (via Avoirs linked to FactureAvoirClient)
        var totalFacturesAvoir = await _context.FactureAvoirClient
            .Where(fa => fa.IdClient == query.ClientId && fa.AccountingYearId == accountingYearId.Value)
            .SelectMany(fa => fa.Avoirs)
            .SelectMany(a => a.LigneAvoirs)
            .SumAsync(l => l.TotTtc, cancellationToken);

        // Calculate total payments
        var totalPaiements = await _context.PaiementClient
            .Where(p => p.ClientId == query.ClientId && p.AccountingYearId == accountingYearId.Value)
            .SumAsync(p => p.Montant, cancellationToken);

        var solde = SoldeClientCalculator.ComputeSolde(
            totalFactures, totalBonsLivraisonNonFactures, totalAvoirs, totalFacturesAvoir, totalPaiements);

        // Get documents
        var documents = new List<DocumentSoldeClient>();

        // Helper method to create BL document with lines
        DocumentSoldeClient CreateBlDocument(BonDeLivraison b)
        {
            var lignes = b.LigneBl.Select(l =>
            {
                // Si QteLivree est null, cela signifie qu'aucune quantité n'a été livrée
                var qteLivree = l.QteLivree ?? 0;
                var quantiteNonLivree = l.QteLi - qteLivree;
                return new LigneBlSoldeClient
                {
                    RefProduit = l.RefProduit,
                    DesignationLi = l.DesignationLi,
                    QteLi = l.QteLi,
                    QteLivree = l.QteLivree,
                    QuantiteNonLivree = quantiteNonLivree
                };
            }).ToList();

            var hasQuantitesNonLivrees = lignes.Any(l => l.QuantiteNonLivree > 0);

            return new DocumentSoldeClient
            {
                Type = "BonDeLivraison",
                Id = b.Id,
                Numero = b.Num,
                Date = b.Date,
                Montant = b.NetPayer,
                LignesBl = lignes,
                HasQuantitesNonLivrees = hasQuantitesNonLivrees
            };
        }

        // Add factures with their BLs (factures already loaded above)
        var documentsFactures = factures.Select(f =>
        {
            var bls = f.BonDeLivraison.Select(CreateBlDocument).ToList();
            var hasQuantitesNonLivrees = bls.Any(bl => bl.HasQuantitesNonLivrees);
            var montant = SoldeClientCalculator.ComputeMontantFactureClient(
                f.Num, montantApresRetenuByNum,
                f.BonDeLivraison.Sum(b => b.NetPayer), timbre);
            return new DocumentSoldeClient
            {
                Type = DocumentTypes.Facture,
                Id = f.Id,
                Numero = f.Num,
                Date = f.Date,
                Montant = montant,
                BonsLivraison = bls,
                HasQuantitesNonLivrees = hasQuantitesNonLivrees
            };
        }).ToList();

        documents.AddRange(documentsFactures);

        // Add non-factured BLs
        var bonsLivraisonNonFactures = await _context.BonDeLivraison
            .Where(b => b.ClientId == query.ClientId 
                && b.AccountingYearId == accountingYearId.Value 
                && b.NumFacture == null)
            .Include(b => b.LigneBl)
            .ToListAsync(cancellationToken);

        var documentsBl = bonsLivraisonNonFactures.Select(CreateBlDocument).ToList();
        documents.AddRange(documentsBl);

        // Add avoirs (direct avoirs not linked to FactureAvoirClient)
        var avoirs = await _context.Avoirs
            .Where(a => a.ClientId == query.ClientId 
                && a.AccountingYearId == accountingYearId.Value 
                && a.NumFactureAvoirClient == null)
            .Select(a => new DocumentSoldeClient
            {
                Type = "Avoir",
                Id = a.Id,
                Numero = a.Num,
                Date = a.Date,
                Montant = a.LigneAvoirs.Sum(l => l.TotTtc)
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(avoirs);

        // Add factures avoir
        var facturesAvoir = await _context.FactureAvoirClient
            .Where(fa => fa.IdClient == query.ClientId && fa.AccountingYearId == accountingYearId.Value)
            .Select(fa => new DocumentSoldeClient
            {
                Type = "FactureAvoir",
                Id = fa.Id,
                Numero = fa.Num,
                Date = fa.Date,
                Montant = fa.Avoirs.SelectMany(a => a.LigneAvoirs).Sum(l => l.TotTtc)
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(facturesAvoir);

        // Get payments with attached factures (num + TTC)
        var paiementsEntities = await _context.PaiementClient
            .Where(p => p.ClientId == query.ClientId && p.AccountingYearId == accountingYearId.Value)
            .Include(p => p.Banque)
            .Include(p => p.Factures)
                .ThenInclude(pf => pf.Facture)
                .ThenInclude(f => f.BonDeLivraison)
            .OrderByDescending(p => p.DatePaiement)
            .ToListAsync(cancellationToken);

        var paiements = paiementsEntities.Select(p =>
        {
            var facturesRattachees = p.Factures.Select(pf =>
            {
                var f = pf.Facture;
                var montantTtc = SoldeClientCalculator.ComputeMontantFactureClient(
                    f.Num, montantApresRetenuByNum,
                    f.BonDeLivraison.Sum(b => b.NetPayer), timbre);
                return new FactureRattacheeSolde { Numero = f.Num, MontantTtc = montantTtc };
            }).ToList();
            return new PaiementSoldeClient
            {
                Id = p.Id,
                NumeroTransactionBancaire = p.NumeroTransactionBancaire,
                DatePaiement = p.DatePaiement,
                Montant = p.Montant,
                MethodePaiement = p.MethodePaiement.ToString(),
                NumeroChequeTraite = p.NumeroChequeTraite,
                BanqueNom = p.Banque != null ? p.Banque.Nom : null,
                DateEcheance = p.DateEcheance,
                Factures = facturesRattachees
            };
        }).ToList();

        var response = new SoldeClientResponse
        {
            ClientId = query.ClientId,
            ClientNom = client.Nom,
            AccountingYearId = accountingYearId.Value,
            TotalFactures = totalFactures,
            TotalBonsLivraisonNonFactures = totalBonsLivraisonNonFactures,
            TotalAvoirs = totalAvoirs,
            TotalFacturesAvoir = totalFacturesAvoir,
            TotalPaiements = totalPaiements,
            Solde = solde,
            Documents = documents.OrderByDescending(d => d.Date).ToList(),
            Paiements = paiements
        };

        return Result.Ok(response);
    }
}

