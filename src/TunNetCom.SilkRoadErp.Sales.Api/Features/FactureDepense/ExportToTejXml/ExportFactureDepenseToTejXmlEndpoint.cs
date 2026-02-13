using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.ExportToTejXml;

public class ExportFactureDepenseToTejXmlEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/factures-depenses/{id:int}/export/tej-xml", HandleExportToTejXmlAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.FactureDepense)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, BadRequest<string>, NotFound<string>, StatusCodeHttpResult>> HandleExportToTejXmlAsync(
        [FromServices] SalesContext context,
        [FromServices] TejXmlExportService exportService,
        [FromServices] INumberGeneratorService numberGeneratorService,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<ExportFactureDepenseToTejXmlEndpoint> logger,
        [FromServices] IActiveAccountingYearService activeAccountingYearService,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("ExportFactureDepenseToTejXmlEndpoint called for FactureDepense Id {Id}", id);

            var appParamsResult = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            if (appParamsResult.IsFailed)
            {
                logger.LogError("Failed to retrieve app parameters");
                return TypedResults.BadRequest("Impossible de récupérer les paramètres de l'application.");
            }

            var factureDepense = await context.FactureDepense
                .Include(f => f.IdTiersDepenseFonctionnementNavigation)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            if (factureDepense == null)
            {
                logger.LogWarning("FactureDepense Id {Id} not found", id);
                return TypedResults.NotFound($"Facture dépense introuvable.");
            }

            var tiers = factureDepense.IdTiersDepenseFonctionnementNavigation;
            if (tiers == null)
            {
                logger.LogWarning("Tiers dépense not found for FactureDepense Id {Id}", id);
                return TypedResults.BadRequest("Tiers dépense introuvable pour cette facture.");
            }

            if (tiers.ExonereRetenueSource)
            {
                logger.LogWarning("TEJ export not applicable: tiers is exempt from withholding tax for FactureDepense Id {Id}", id);
                return TypedResults.BadRequest("Le tiers est exonéré de retenue à la source ; l'export TEJ n'est pas applicable.");
            }

            var seuilRetenueSource = await financialParametersService.GetSeuilRetenueSourceAsync(1000, cancellationToken);
            if (factureDepense.MontantTotal < seuilRetenueSource)
            {
                logger.LogWarning(
                    "Montant {MontantTotal} is below threshold {Seuil} for FactureDepense Id {Id}",
                    factureDepense.MontantTotal, seuilRetenueSource, id);
                return TypedResults.BadRequest(
                    $"Le montant ({factureDepense.MontantTotal:F2}) doit être supérieur ou égal au seuil ({seuilRetenueSource:F2}) pour pouvoir exporter en XML TEJ.");
            }

            var existingAttribution = await context.TejCertificatFactureDepense
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.FactureDepenseId == id, cancellationToken);

            string refCertifTej;
            if (existingAttribution != null)
            {
                refCertifTej = existingAttribution.RefCertif;
            }
            else
            {
                var year = factureDepense.Date.Year;
                var month = factureDepense.Date.Month;
                refCertifTej = await numberGeneratorService.GetNextTejCertificatRefAsync(year, month, cancellationToken);
                context.TejCertificatFactureDepense.Add(new TejCertificatFactureDepense
                {
                    FactureDepenseId = id,
                    RefCertif = refCertifTej
                });
                await context.SaveChangesAsync(cancellationToken);
            }

            _ = await activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);

            var systeme = await context.Systeme
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (systeme == null)
            {
                logger.LogError("Systeme parameters not found");
                return TypedResults.BadRequest("Paramètres système introuvables.");
            }

            var financialParams = await financialParametersService.GetAllFinancialParametersAsync(cancellationToken);
            if (financialParams == null)
            {
                logger.LogError("No active accounting year found");
                return TypedResults.BadRequest("Aucun exercice comptable actif trouvé.");
            }

            if (string.IsNullOrWhiteSpace(systeme.MatriculeFiscale))
            {
                logger.LogWarning("MatriculeFiscale is missing in systeme parameters");
                return TypedResults.BadRequest("Le matricule fiscal de l'entreprise n'est pas configuré dans les paramètres système.");
            }

            if (string.IsNullOrWhiteSpace(tiers.Matricule))
            {
                logger.LogWarning("Tiers Matricule is missing for FactureDepense Id {Id}", id);
                return TypedResults.BadRequest("Le matricule fiscal du tiers n'est pas configuré pour cette facture.");
            }

            if (string.IsNullOrWhiteSpace(tiers.Mail))
            {
                logger.LogWarning("Tiers Mail missing for FactureDepense Id {Id}, TEJ export blocked", id);
                return TypedResults.BadRequest(
                    "L'export TEJ exige l'adresse e-mail du tiers. Veuillez renseigner l'e-mail du tiers avant d'exporter.");
            }

            var telDigitsOnly = new string((tiers.Tel ?? "").Where(char.IsDigit).ToArray());
            if (telDigitsOnly.Length != 8)
            {
                logger.LogWarning("Tiers Tel not in format XXXXXXXX (8 digits) for FactureDepense Id {Id}, TEJ export blocked", id);
                return TypedResults.BadRequest(
                    "L'export TEJ exige le téléphone du tiers au format XXXXXXXX (8 chiffres). Veuillez corriger le numéro du tiers.");
            }

            var tiersMatriculeNormalise = TryNormalizeMatriculeFiscal(tiers.Matricule.Trim());
            if (tiersMatriculeNormalise is null)
            {
                logger.LogWarning("Tiers Matricule format invalid for FactureDepense Id {Id}: expected 7 digits + 1 letter", id);
                return TypedResults.BadRequest(
                    "Le matricule fiscal du tiers doit être au format 7 chiffres et une lettre clé (ex. 0001238L).");
            }

            var matriculeNormaliseResult = TryNormalizeMatriculeFiscal(systeme.MatriculeFiscale.Trim());
            if (matriculeNormaliseResult is null)
            {
                logger.LogWarning("MatriculeFiscale format invalid: expected 7 digits + 1 letter, got {Value}", systeme.MatriculeFiscale);
                return TypedResults.BadRequest(
                    "Le matricule fiscal de l'entreprise doit être au format 7 chiffres et une lettre clé (ex. 0001238L).");
            }

            var xmlBytes = exportService.ExportFactureDepenseToTejXml(
                factureDepense,
                tiers,
                systeme,
                appParamsResult.Value,
                financialParams,
                refCertifChezDeclarant: refCertifTej,
                normalizedDeclarantMatricule: matriculeNormaliseResult,
                normalizedBeneficiaireMatricule: tiersMatriculeNormalise,
                beneficiaireTel8Digits: telDigitsOnly);

            var validationErrors = TejXsdValidator.Validate(xmlBytes)
                .Where(e => !e.Contains("introuvable", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (validationErrors.Count > 0)
            {
                logger.LogWarning("TEJ XSD validation failed for FactureDepense Id {Id}: {Errors}", id, string.Join("; ", validationErrors));
                return TypedResults.BadRequest(
                    "Le fichier XML n'est pas conforme au schéma TEJ: " + string.Join("; ", validationErrors));
            }

            var exercice = factureDepense.Date.Year;
            var mois = factureDepense.Date.Month.ToString("D2");
            const string codeActe = "0";
            var filename = $"{matriculeNormaliseResult}-{exercice}-{mois}-{codeActe}.xml";

            logger.LogInformation("TEJ XML export completed successfully for FactureDepense Id {Id}", id);

            return TypedResults.File(
                xmlBytes,
                contentType: "application/xml; charset=utf-8",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting FactureDepense Id {Id} to TEJ XML format", id);
            return TypedResults.StatusCode(500);
        }
    }

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
