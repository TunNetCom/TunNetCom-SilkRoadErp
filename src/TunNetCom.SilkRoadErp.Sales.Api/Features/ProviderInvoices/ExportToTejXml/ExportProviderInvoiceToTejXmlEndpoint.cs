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

            if (factureFournisseur.IdFournisseurNavigation.ExonereRetenueSource)
            {
                logger.LogWarning("TEJ export not applicable: provider is exempt from withholding tax for invoice {InvoiceNumber}", invoiceNumber);
                return TypedResults.BadRequest("Le fournisseur est exonéré de retenue à la source ; l'export TEJ n'est pas applicable.");
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

            // TEJ export requires provider email and phone (format XXXXXXXX = 8 digits)
            var provider = factureFournisseur.IdFournisseurNavigation;
            if (string.IsNullOrWhiteSpace(provider.Mail))
            {
                logger.LogWarning("Provider Mail missing for invoice {InvoiceNumber}, TEJ export blocked", invoiceNumber);
                return TypedResults.BadRequest(
                    "L'export TEJ exige l'adresse e-mail du fournisseur. Veuillez renseigner l'e-mail du fournisseur avant d'exporter.");
            }
            var telDigitsOnly = new string((provider.Tel ?? "").Where(char.IsDigit).ToArray());
            if (telDigitsOnly.Length != 8)
            {
                logger.LogWarning("Provider Tel not in format XXXXXXXX (8 digits) for invoice {InvoiceNumber}, TEJ export blocked", invoiceNumber);
                return TypedResults.BadRequest(
                    "L'export TEJ exige le téléphone du fournisseur au format XXXXXXXX (8 chiffres). Veuillez corriger le numéro du fournisseur.");
            }

            // Normalize and validate beneficiaire (provider) matricule fiscal: 7 digits + 1 letter (CCT pattern)
            var providerMatriculeNormalise = TryNormalizeMatriculeFiscal(factureFournisseur.IdFournisseurNavigation.Matricule.Trim());
            if (providerMatriculeNormalise is null)
            {
                logger.LogWarning("Provider Matricule format invalid for invoice {InvoiceNumber}: expected 7 digits + 1 letter", invoiceNumber);
                return TypedResults.BadRequest(
                    "Le matricule fiscal du fournisseur doit être au format 7 chiffres et une lettre clé (ex. 0001238L).");
            }

            // Normalize matricule fiscal for filename (7 digits + 1 letter)
            var matriculeNormaliseResult = TryNormalizeMatriculeFiscal(systeme.MatriculeFiscale.Trim());
            if (matriculeNormaliseResult is null)
            {
                logger.LogWarning("MatriculeFiscale format invalid: expected 7 digits + 1 letter, got {Value}", systeme.MatriculeFiscale);
                return TypedResults.BadRequest(
                    "Le matricule fiscal de l'entreprise doit être au format 7 chiffres et une lettre clé (ex. 0001238L).");
            }

            // Generate XML (pass normalized declarant/beneficiaire matricules and beneficiaire tel 8 digits for TEJ)
            var xmlBytes = exportService.ExportProviderInvoiceToTejXml(
                factureFournisseur,
                factureFournisseur.IdFournisseurNavigation,
                systeme,
                appParams,
                financialParams,
                normalizedDeclarantMatricule: matriculeNormaliseResult,
                normalizedBeneficiaireMatricule: providerMatriculeNormalise,
                beneficiaireTel8Digits: telDigitsOnly);

            // Validate against TEJ XSD when schema is available
            var validationErrors = TejXsdValidator.Validate(xmlBytes)
                .Where(e => !e.Contains("introuvable", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (validationErrors.Count > 0)
            {
                logger.LogWarning("TEJ XSD validation failed for invoice {InvoiceNumber}: {Errors}", invoiceNumber, string.Join("; ", validationErrors));
                return TypedResults.BadRequest(
                    "Le fichier XML n'est pas conforme au schéma TEJ: " + string.Join("; ", validationErrors));
            }

            // Generate filename per regulatory format: [MATRICULEFISCAL]-[EXERCICE]-[mois]-[code acte].xml
            var exercice = factureFournisseur.Date.Year;
            var mois = factureFournisseur.Date.Month.ToString("D2");
            const string codeActe = "0"; // 0 = déclaration initiale
            var filename = $"{matriculeNormaliseResult}-{exercice}-{mois}-{codeActe}.xml";

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

    /// <summary>
    /// Normalizes matricule fiscal to regulatory format: 7 digits + 1 letter (8 characters).
    /// Removes non-alphanumeric characters, pads numeric part with leading zeros, takes first letter as key.
    /// </summary>
    /// <returns>Normalized string (e.g. "0001238L") or null if format is invalid (no letter or empty).</returns>
    private static string? TryNormalizeMatriculeFiscal(string matriculeFiscale)
    {
        if (string.IsNullOrWhiteSpace(matriculeFiscale))
            return null;

        var cleaned = new string(matriculeFiscale.Where(c => char.IsLetterOrDigit(c)).ToArray());
        if (cleaned.Length == 0)
            return null;

        var digits = new string(cleaned.Where(char.IsDigit).Take(7).ToArray());
        var letterPart = new string(cleaned.Where(char.IsLetter).ToArray());
        if (letterPart.Length == 0)
            return null;

        var digitsPadded = digits.PadLeft(7, '0');
        var letter = letterPart[0];
        if (!char.IsLetter(letter))
            return null;

        return digitsPadded + char.ToUpperInvariant(letter);
    }
}

