using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeClient;

public class GetSoldeClientQueryHandler(
    SalesContext _context,
    ILogger<GetSoldeClientQueryHandler> _logger,
    IMediator mediator)
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
            var activeYear = await _context.AccountingYear
                .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);
            if (activeYear == null)
            {
                return Result.Fail("no_active_accounting_year");
            }
            accountingYearId = activeYear.Id;
        }

        var appParams = await mediator.Send(new GetAppParametersQuery());

        // Calculate total from factures (factured BLs)
        var facturesCount = await _context.Facture
            .Where(f => f.IdClient == query.ClientId && f.AccountingYearId == accountingYearId.Value)
            .CountAsync(cancellationToken);
        
        var totalFactures = await _context.Facture
            .Where(f => f.IdClient == query.ClientId && f.AccountingYearId == accountingYearId.Value)
            .SelectMany(f => f.BonDeLivraison)
            .SumAsync(b => b.NetPayer, cancellationToken) + (appParams.Value.Timbre * facturesCount);

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

        var solde = totalAvoirs + totalFacturesAvoir + totalPaiements - totalFactures - totalBonsLivraisonNonFactures;

        // Get documents
        var documents = new List<DocumentSoldeClient>();

        // Helper method to create BL document with lines
        DocumentSoldeClient CreateBlDocument(BonDeLivraison b)
        {
            var lignes = b.LigneBl.Select(l =>
            {
                var qteLivree = l.QteLivree ?? l.QteLi;
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

        // Add factures with their BLs
        var factures = await _context.Facture
            .Where(f => f.IdClient == query.ClientId && f.AccountingYearId == accountingYearId.Value)
            .Include(f => f.BonDeLivraison)
                .ThenInclude(b => b.LigneBl)
            .ToListAsync(cancellationToken);

        var documentsFactures = factures.Select(f =>
        {
            var bls = f.BonDeLivraison.Select(CreateBlDocument).ToList();
            var hasQuantitesNonLivrees = bls.Any(bl => bl.HasQuantitesNonLivrees);

            return new DocumentSoldeClient
            {
                Type = "Facture",
                Id = f.Id,
                Numero = f.Num,
                Date = f.Date,
                Montant = f.BonDeLivraison.Sum(b => b.NetPayer) + appParams.Value.Timbre,
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

        // Get payments
        var paiements = await _context.PaiementClient
            .Where(p => p.ClientId == query.ClientId && p.AccountingYearId == accountingYearId.Value)
            .Select(p => new PaiementSoldeClient
            {
                Id = p.Id,
                Numero = p.Numero,
                DatePaiement = p.DatePaiement,
                Montant = p.Montant,
                MethodePaiement = p.MethodePaiement.ToString()
            })
            .OrderByDescending(p => p.DatePaiement)
            .ToListAsync(cancellationToken);

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

