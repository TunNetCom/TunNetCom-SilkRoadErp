using TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseurWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetAvoirsWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFacturesDepenseWithSummaries;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceTotals;
using TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementsClient;
using TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementsFournisseur;
using TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementsTiersDepense;
using TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.GetProviderInvoiceTotals;
using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Dashboard.GetRecapVentesAchats;

public class GetRecapVentesAchatsQueryHandler(IMediator mediator, ILogger<GetRecapVentesAchatsQueryHandler> logger)
    : IRequestHandler<GetRecapVentesAchatsQuery, RecapVentesAchatsResponse>
{
    public async Task<RecapVentesAchatsResponse> Handle(GetRecapVentesAchatsQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate;
        var endDateInclusive = request.EndDate.Date.AddDays(1).AddTicks(-1);

        var response = new RecapVentesAchatsResponse
        {
            StartDate = startDate,
            EndDate = request.EndDate
        };

        // 1) Invoice totals (ventes factures)
        var invoiceTotals = await mediator.Send(new GetInvoiceTotalsQuery(startDate, endDateInclusive), cancellationToken);
        response.VentesFactures = MapFromInvoiceTotals(invoiceTotals);

        // 2) Avoirs clients
        var avoirsResponse = await mediator.Send(new GetAvoirsWithSummariesQuery(1, 1, null, null, null, null, startDate, endDateInclusive, null), cancellationToken);
        response.VentesAvoirs = new RecapTotalsSectionDto
        {
            TotalHT = avoirsResponse.TotalNetAmount,
            TotalTVA = avoirsResponse.TotalVatAmount,
            TotalTTC = avoirsResponse.TotalIncludingTaxAmount
        };

        // 3) Ventes nettes
        response.VentesNettes = Subtract(response.VentesFactures, response.VentesAvoirs);

        // 4) Provider invoice totals
        var providerTotals = await mediator.Send(new GetProviderInvoiceTotalsQuery(startDate, endDateInclusive), cancellationToken);
        response.AchatsFacturesFournisseurs = MapFromProviderTotals(providerTotals);

        // 5) Avoirs fournisseur
        var avoirFournisseurResponse = await mediator.Send(new GetAvoirFournisseurWithSummariesQuery(1, 1, null, null, null, null, null, startDate, endDateInclusive, null), cancellationToken);
        response.AchatsAvoirsFournisseur = new RecapTotalsSectionDto
        {
            TotalHT = avoirFournisseurResponse.TotalNetAmount,
            TotalTVA = avoirFournisseurResponse.TotalVatAmount,
            TotalTTC = avoirFournisseurResponse.TotalIncludingTaxAmount
        };

        // 6) Factures dépenses
        var facturesDepenseResponse = await mediator.Send(new GetFacturesDepenseWithSummariesQuery(1, 10000, null, null, null, startDate, request.EndDate), cancellationToken);
        response.AchatsFacturesDepensesTotal = facturesDepenseResponse.Items?.Sum(f => f.MontantTotal) ?? 0;

        // 7) Achats nets
        response.AchatsNets = Subtract(response.AchatsFacturesFournisseurs, response.AchatsAvoirsFournisseur);
        response.AchatsNets.TotalTTC += response.AchatsFacturesDepensesTotal;

        // 8) Paiements clients
        var paiementsClientResult = await mediator.Send(new GetPaiementsClientQuery(null, null, null, null, startDate, endDateInclusive, null, null, null, null, null, 1, 10000), cancellationToken);
        response.PaiementsClientsTotal = paiementsClientResult.Value?.Items?.Sum(p => p.Montant) ?? 0;

        // 9) Paiements fournisseurs
        var paiementsFournisseur = await mediator.Send(new GetPaiementsFournisseurQuery(null, null, null, null, startDate, endDateInclusive, null, null, null, null, 1, 10000), cancellationToken);
        response.PaiementsFournisseursTotal = paiementsFournisseur.Value?.Items?.Sum(p => p.Montant) ?? 0;

        // 10) Paiements tiers dépenses
        var paiementsTiersDepense = await mediator.Send(new GetPaiementsTiersDepenseQuery(null, null, startDate, endDateInclusive, 1, 10000), cancellationToken);
        response.PaiementsTiersDepensesTotal = paiementsTiersDepense.Value?.Items?.Sum(p => p.Montant) ?? 0;

        // 11) Résultat
        response.Resultat = Subtract(response.VentesNettes, response.AchatsNets);

        logger.LogInformation("Recap ventes/achats calculated for {StartDate} to {EndDate}", startDate, request.EndDate);
        return response;
    }

    private static RecapTotalsSectionDto MapFromInvoiceTotals(InvoiceTotalsResponse r) => new()
    {
        TotalHT = r.TotalHT,
        TotalTTC = r.TotalTTC,
        TotalTVA = r.TotalVat,
        TotalBase7 = r.TotalBase7,
        TotalBase13 = r.TotalBase13,
        TotalBase19 = r.TotalBase19,
        TotalVat7 = r.TotalVat7,
        TotalVat13 = r.TotalVat13,
        TotalVat19 = r.TotalVat19
    };

    private static RecapTotalsSectionDto MapFromProviderTotals(ProviderInvoiceTotalsResponse r) => new()
    {
        TotalHT = r.TotalHT,
        TotalTTC = r.TotalTTC,
        TotalTVA = r.TotalVat,
        TotalVat7 = r.TotalVat7,
        TotalVat13 = r.TotalVat13,
        TotalVat19 = r.TotalVat19
    };

    private static RecapTotalsSectionDto Subtract(RecapTotalsSectionDto a, RecapTotalsSectionDto b) => new()
    {
        TotalHT = a.TotalHT - b.TotalHT,
        TotalTTC = a.TotalTTC - b.TotalTTC,
        TotalTVA = a.TotalTVA - b.TotalTVA,
        TotalBase7 = a.TotalBase7 - b.TotalBase7,
        TotalBase13 = a.TotalBase13 - b.TotalBase13,
        TotalBase19 = a.TotalBase19 - b.TotalBase19,
        TotalVat7 = a.TotalVat7 - b.TotalVat7,
        TotalVat13 = a.TotalVat13 - b.TotalVat13,
        TotalVat19 = a.TotalVat19 - b.TotalVat19
    };
}
