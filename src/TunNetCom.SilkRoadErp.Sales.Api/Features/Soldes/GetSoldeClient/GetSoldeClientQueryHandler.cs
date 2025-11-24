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

        // Calculate total payments
        var totalPaiements = await _context.PaiementClient
            .Where(p => p.ClientId == query.ClientId && p.AccountingYearId == accountingYearId.Value)
            .SumAsync(p => p.Montant, cancellationToken);

        var solde = totalFactures + totalBonsLivraisonNonFactures - totalPaiements;

        // Get documents
        var documents = new List<DocumentSoldeClient>();

        // Add factures
        var factures = await _context.Facture
            .Where(f => f.IdClient == query.ClientId && f.AccountingYearId == accountingYearId.Value)
            .Select(f => new DocumentSoldeClient
            {
                Type = "Facture",
                Id = f.Id,
                Numero = f.Num,
                Date = f.Date,
                Montant = f.BonDeLivraison.Sum(b => b.NetPayer) + appParams.Value.Timbre
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(factures);

        // Add non-factured BLs
        var bonsLivraison = await _context.BonDeLivraison
            .Where(b => b.ClientId == query.ClientId 
                && b.AccountingYearId == accountingYearId.Value 
                && b.NumFacture == null)
            .Select(b => new DocumentSoldeClient
            {
                Type = "BonDeLivraison",
                Id = b.Id,
                Numero = b.Num,
                Date = b.Date,
                Montant = b.NetPayer
            })
            .ToListAsync(cancellationToken);
        documents.AddRange(bonsLivraison);

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
            TotalPaiements = totalPaiements,
            Solde = solde,
            Documents = documents.OrderByDescending(d => d.Date).ToList(),
            Paiements = paiements
        };

        return Result.Ok(response);
    }
}

