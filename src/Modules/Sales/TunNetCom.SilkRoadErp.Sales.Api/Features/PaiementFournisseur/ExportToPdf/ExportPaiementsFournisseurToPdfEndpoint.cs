using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.ExportToExcel;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.ExportToPdf;

public class ExportPaiementsFournisseurToPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/paiement-fournisseur/export/pdf", HandleExportToPdfAsync)
            .WithTags(EndpointTags.PaiementFournisseur)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToPdfAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] PdfListExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportPaiementsFournisseurToPdfEndpoint> logger,
        [FromQuery] int? fournisseurId = null,
        [FromQuery] int[]? accountingYearIds = null,
        [FromQuery] DateTime? dateEcheanceFrom = null,
        [FromQuery] DateTime? dateEcheanceTo = null,
        [FromQuery] decimal? montantMin = null,
        [FromQuery] decimal? montantMax = null,
        [FromQuery] bool? hasNumeroTransactionBancaire = null,
        [FromQuery] int? mois = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accountingYearIdsList = accountingYearIds != null && accountingYearIds.Length > 0 ? accountingYearIds.ToList() : null;
            logger.LogInformation(
                "ExportPaiementsFournisseurToPdfEndpoint called with fournisseurId: {FournisseurId}, accountingYearIds: {AccountingYearIds}, dateEcheanceFrom: {DateEcheanceFrom}, dateEcheanceTo: {DateEcheanceTo}, montantMin: {MontantMin}, montantMax: {MontantMax}, hasNumeroTransactionBancaire: {HasNumeroTransactionBancaire}, mois: {Mois}",
                fournisseurId, accountingYearIdsList != null ? string.Join(",", accountingYearIdsList) : null, dateEcheanceFrom, dateEcheanceTo, montantMin, montantMax, hasNumeroTransactionBancaire, mois);

            // Get financial parameters from service
            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            // Build query with same filters as GetPaiementsFournisseurQueryHandler
            var paiementsQuery = context.PaiementFournisseur
                .AsNoTracking()
                .AsQueryable();

            if (fournisseurId.HasValue)
            {
                paiementsQuery = paiementsQuery.Where(p => p.FournisseurId == fournisseurId.Value);
            }

            if (accountingYearIdsList != null && accountingYearIdsList.Count > 0)
            {
                paiementsQuery = paiementsQuery.Where(p => accountingYearIdsList.Contains(p.AccountingYearId));
            }

            if (dateEcheanceFrom.HasValue)
            {
                paiementsQuery = paiementsQuery.Where(p => p.DateEcheance.HasValue && p.DateEcheance >= dateEcheanceFrom.Value);
            }

            if (dateEcheanceTo.HasValue)
            {
                var dateEcheanceToInclusive = dateEcheanceTo.Value.Date.AddDays(1).AddTicks(-1);
                paiementsQuery = paiementsQuery.Where(p => p.DateEcheance.HasValue && p.DateEcheance <= dateEcheanceToInclusive);
            }

            if (montantMin.HasValue)
            {
                paiementsQuery = paiementsQuery.Where(p => p.Montant >= montantMin.Value);
            }

            if (montantMax.HasValue)
            {
                paiementsQuery = paiementsQuery.Where(p => p.Montant <= montantMax.Value);
            }

            if (hasNumeroTransactionBancaire.HasValue)
            {
                if (hasNumeroTransactionBancaire.Value)
                {
                    paiementsQuery = paiementsQuery.Where(p => !string.IsNullOrEmpty(p.NumeroTransactionBancaire));
                }
                else
                {
                    paiementsQuery = paiementsQuery.Where(p => string.IsNullOrEmpty(p.NumeroTransactionBancaire));
                }
            }

            if (mois.HasValue)
            {
                paiementsQuery = paiementsQuery.Where(p => p.Mois.HasValue && p.Mois.Value == mois.Value);
            }

            // Get all paiements (no pagination for export)
            var paiements = await paiementsQuery
                .Select(p => new PaiementFournisseurExportInfo
                {
                    NumeroTransactionBancaire = p.NumeroTransactionBancaire ?? string.Empty,
                    NumeroChequeTraite = p.NumeroChequeTraite ?? string.Empty,
                    FournisseurNom = p.Fournisseur.Nom,
                    Montant = p.Montant,
                    DatePaiement = p.DatePaiement,
                    MethodePaiement = p.MethodePaiement.ToString(),
                    BanqueNom = p.Banque != null ? p.Banque.Nom : string.Empty,
                    DateEcheance = p.DateEcheance,
                    StatutReglement = !string.IsNullOrEmpty(p.NumeroTransactionBancaire) ? "Régle" : "Non réglé"
                })
                .OrderByDescending(p => p.DatePaiement)
                .ToListAsync(cancellationToken);

            if (!paiements.Any())
            {
                logger.LogWarning("No paiements found to export");
                // Return empty PDF file
                var emptyFileBytes = await exportService.ExportToPdfAsync(
                    new List<PaiementFournisseurExportInfo>(),
                    new List<ColumnMapping>(),
                    "Paiements Fournisseurs",
                    decimalPlaces,
                    cancellationToken: cancellationToken);
                var emptyFilename = $"Paiements_Fournisseurs_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return TypedResults.File(
                    emptyFileBytes,
                    contentType: "application/pdf",
                    fileDownloadName: emptyFilename);
            }

            // Define columns to export
            var columns = new List<ColumnMapping>
            {
                new() { PropertyName = "NumeroTransactionBancaire", DisplayName = "Numéro Transaction Bancaire" },
                new() { PropertyName = "NumeroChequeTraite", DisplayName = "Numéro Traite" },
                new() { PropertyName = "FournisseurNom", DisplayName = "Fournisseur" },
                new() { PropertyName = "Montant", DisplayName = "Montant" },
                new() { PropertyName = "DatePaiement", DisplayName = "Date Paiement" },
                new() { PropertyName = "MethodePaiement", DisplayName = "Méthode Paiement" },
                new() { PropertyName = "BanqueNom", DisplayName = "Banque" },
                new() { PropertyName = "DateEcheance", DisplayName = "Date Échéance" },
                new() { PropertyName = "StatutReglement", DisplayName = "Statut Règlement" }
            };

            logger.LogInformation("Exporting {Count} paiements fournisseur to PDF", paiements.Count);

            // Calculate totals
            var totalMontant = paiements.Sum(p => p.Montant);

            var fileBytes = await exportService.ExportToPdfAsync(
                paiements,
                columns,
                "Liste des Paiements Fournisseurs",
                decimalPlaces,
                totalNetAmount: totalMontant,
                cancellationToken: cancellationToken);

            var filename = $"Paiements_Fournisseurs_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return TypedResults.File(
                fileBytes,
                contentType: "application/pdf",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting paiements fournisseur to PDF format");
            return TypedResults.StatusCode(500);
        }
    }
}

