using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.ExportToExcel;

public class ExportPaiementsClientToExcelEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/paiement-client/export/excel", HandleExportToExcelAsync)
            .WithTags(EndpointTags.PaiementClient)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, StatusCodeHttpResult>> HandleExportToExcelAsync(
        [FromServices] SalesContext context,
        [FromServices] IMediator mediator,
        [FromServices] ExcelExportService exportService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromServices] ILogger<ExportPaiementsClientToExcelEndpoint> logger,
        [FromQuery] int? clientId = null,
        [FromQuery] int[]? accountingYearIds = null,
        [FromQuery] DateTime? dateEcheanceFrom = null,
        [FromQuery] DateTime? dateEcheanceTo = null,
        [FromQuery] decimal? montantMin = null,
        [FromQuery] decimal? montantMax = null,
        [FromQuery] bool? hasNumeroTransactionBancaire = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accountingYearIdsList = accountingYearIds != null && accountingYearIds.Length > 0 ? accountingYearIds.ToList() : null;
            logger.LogInformation(
                "ExportPaiementsClientToExcelEndpoint called with clientId: {ClientId}, accountingYearIds: {AccountingYearIds}, dateEcheanceFrom: {DateEcheanceFrom}, dateEcheanceTo: {DateEcheanceTo}, montantMin: {MontantMin}, montantMax: {MontantMax}, hasNumeroTransactionBancaire: {HasNumeroTransactionBancaire}",
                clientId, accountingYearIdsList != null ? string.Join(",", accountingYearIdsList) : null, dateEcheanceFrom, dateEcheanceTo, montantMin, montantMax, hasNumeroTransactionBancaire);

            var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            var decimalPlaces = await financialParametersService.GetDecimalPlacesAsync(appParams.Value.DecimalPlaces, cancellationToken);

            var paiementsQuery = context.PaiementClient
                .AsNoTracking()
                .AsQueryable();

            if (clientId.HasValue)
            {
                paiementsQuery = paiementsQuery.Where(p => p.ClientId == clientId.Value);
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

            var paiements = await paiementsQuery
                .Select(p => new PaiementClientExportInfo
                {
                    ClientNom = p.Client.Nom,
                    Montant = p.Montant,
                    DatePaiement = p.DatePaiement,
                    MethodePaiement = p.MethodePaiement.ToString(),
                    DateEcheance = p.DateEcheance,
                    StatutReglement = !string.IsNullOrEmpty(p.NumeroTransactionBancaire) ? "Régle" : "Non réglé"
                })
                .OrderByDescending(p => p.DatePaiement)
                .ToListAsync(cancellationToken);

            if (!paiements.Any())
            {
                logger.LogWarning("No paiements client found to export");
                var emptyFileBytes = exportService.ExportToExcel(
                    new List<PaiementClientExportInfo>(),
                    new List<ColumnMapping>(),
                    "Paiements Clients",
                    decimalPlaces);
                var emptyFilename = $"Paiements_Clients_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return TypedResults.File(
                    emptyFileBytes,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: emptyFilename);
            }

            var columns = new List<ColumnMapping>
            {
                new() { PropertyName = "ClientNom", DisplayName = "Client" },
                new() { PropertyName = "Montant", DisplayName = "Montant" },
                new() { PropertyName = "DatePaiement", DisplayName = "Date Paiement" },
                new() { PropertyName = "MethodePaiement", DisplayName = "Méthode Paiement" },
                new() { PropertyName = "DateEcheance", DisplayName = "Date Échéance" },
                new() { PropertyName = "StatutReglement", DisplayName = "Statut Règlement" }
            };

            logger.LogInformation("Exporting {Count} paiements client to Excel", paiements.Count);

            var totalMontant = paiements.Sum(p => p.Montant);

            var fileBytes = exportService.ExportToExcel(
                paiements,
                columns,
                "Paiements Clients",
                decimalPlaces,
                totalNetAmount: totalMontant);

            var filename = $"Paiements_Clients_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return TypedResults.File(
                fileBytes,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting paiements client to Excel format");
            return TypedResults.StatusCode(500);
        }
    }
}
