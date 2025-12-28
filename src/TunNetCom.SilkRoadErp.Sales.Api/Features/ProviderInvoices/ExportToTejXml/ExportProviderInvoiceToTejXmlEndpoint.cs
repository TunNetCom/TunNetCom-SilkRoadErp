using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.ExportToTejXml;

public class ExportProviderInvoiceToTejXmlEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/provider-invoices/{invoiceNumber}/export/tej-xml", HandleExportToTejXmlAsync)
            .WithTags(EndpointTags.ProviderInvoices)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, BadRequest<string>, NotFound<string>, StatusCodeHttpResult>> HandleExportToTejXmlAsync(
        [FromServices] SalesContext context,
        [FromServices] TejXmlExportService exportService,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<ExportProviderInvoiceToTejXmlEndpoint> logger,
        [FromServices] IActiveAccountingYearService activeAccountingYearService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        int invoiceNumber,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("ExportProviderInvoiceToTejXmlEndpoint called for invoice {InvoiceNumber}", invoiceNumber);

            // Get app parameters
            var appParamsResult = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            if (appParamsResult.IsFailed)
            {
                logger.LogError("Failed to retrieve app parameters");
                return TypedResults.BadRequest("Impossible de récupérer les paramètres de l'application.");
            }
            var appParams = appParamsResult.Value;

            // Get invoice with related data
            var factureFournisseur = await context.FactureFournisseur
                .Include(f => f.IdFournisseurNavigation)
                .Include(f => f.BonDeReception)
                    .ThenInclude(br => br.LigneBonReception)
                .FirstOrDefaultAsync(f => f.Num == invoiceNumber, cancellationToken);

            if (factureFournisseur == null)
            {
                logger.LogWarning("Invoice {InvoiceNumber} not found", invoiceNumber);
                return TypedResults.NotFound($"Facture fournisseur {invoiceNumber} introuvable.");
            }

            if (factureFournisseur.IdFournisseurNavigation == null)
            {
                logger.LogWarning("Provider not found for invoice {InvoiceNumber}", invoiceNumber);
                return TypedResults.BadRequest($"Fournisseur introuvable pour la facture {invoiceNumber}.");
            }

            // Calculate TTC
            var montantTTC = factureFournisseur.BonDeReception
                .SelectMany(br => br.LigneBonReception)
                .Sum(l => l.TotTtc);

            // Get seuil from financial parameters service
            var seuilRetenueSource = await financialParametersService.GetSeuilRetenueSourceAsync(1000, cancellationToken);

            // Validate threshold
            if (montantTTC < seuilRetenueSource)
            {
                logger.LogWarning(
                    "Montant TTC {MontantTTC} is below threshold {Seuil} for Invoice {InvoiceNumber}",
                    montantTTC, seuilRetenueSource, invoiceNumber);
                return TypedResults.BadRequest(
                    $"Le montant TTC ({montantTTC:F2}) doit être supérieur ou égal au seuil ({seuilRetenueSource:F2}) pour pouvoir exporter en XML TEJ.");
            }

            // Get active accounting year ID
            var activeAccountingYearId = await activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
            if (!activeAccountingYearId.HasValue)
            {
                logger.LogError("No active accounting year found");
                return TypedResults.BadRequest("Aucun exercice comptable actif trouvé.");
            }

            // Get systeme data
            var systeme = await context.Systeme
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (systeme == null)
            {
                logger.LogError("Systeme parameters not found");
                return TypedResults.BadRequest("Paramètres système introuvables.");
            }

            // Get financial parameters for validation
            var financialParams = await financialParametersService.GetAllFinancialParametersAsync(cancellationToken);
            if (financialParams == null)
            {
                logger.LogError("No active accounting year found");
                return TypedResults.BadRequest("Aucun exercice comptable actif trouvé.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(systeme.MatriculeFiscale))
            {
                logger.LogWarning("MatriculeFiscale is missing in systeme parameters");
                return TypedResults.BadRequest("Le matricule fiscal de l'entreprise n'est pas configuré dans les paramètres système.");
            }

            if (string.IsNullOrWhiteSpace(factureFournisseur.IdFournisseurNavigation.Matricule))
            {
                logger.LogWarning("Provider Matricule is missing for invoice {InvoiceNumber}", invoiceNumber);
                return TypedResults.BadRequest($"Le matricule fiscal du fournisseur n'est pas configuré pour la facture {invoiceNumber}.");
            }

            // Generate XML
            var xmlBytes = exportService.ExportProviderInvoiceToTejXml(
                factureFournisseur,
                factureFournisseur.IdFournisseurNavigation,
                systeme,
                appParams,
                financialParams);

            // Generate filename
            var filename = $"Facture_Fournisseur_{invoiceNumber}_TEJ_{DateTime.Now:yyyyMMdd_HHmmss}.xml";

            logger.LogInformation("TEJ XML export completed successfully for invoice {InvoiceNumber}", invoiceNumber);

            return TypedResults.File(
                xmlBytes,
                contentType: "application/xml; charset=utf-8",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting invoice {InvoiceNumber} to TEJ XML format", invoiceNumber);
            return TypedResults.StatusCode(500);
        }
    }
}

