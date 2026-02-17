using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tej;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tej.ExportAllToTejXml;

public class ExportAllToTejXmlEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/tej/export-all", HandleExportAllToTejXmlAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.Tej)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<FileContentHttpResult, BadRequest<string>, StatusCodeHttpResult>> HandleExportAllToTejXmlAsync(
        [FromServices] SalesContext context,
        [FromServices] TejXmlExportService exportService,
        [FromServices] INumberGeneratorService numberGeneratorService,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<ExportAllToTejXmlEndpoint> logger,
        [FromServices] IAccountingYearFinancialParametersService financialParametersService,
        [FromBody] ExportAllToTejXmlRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "ExportAllToTejXmlEndpoint called with {ProviderCount} provider invoices and {DepenseCount} factures dépense",
                request.ProviderInvoiceNumbers?.Count ?? 0,
                request.FactureDepenseIds?.Count ?? 0);

            var acteDepot = string.Equals(request.ActeDepot, "1", StringComparison.Ordinal) ? "1" : "0";
            var anneeDepot = request.AnneeDepot;
            var moisDepot = Math.Clamp(request.MoisDepot, 1, 12);

            if (anneeDepot < 2000 || anneeDepot > 2100)
            {
                anneeDepot = DateTime.Today.Year;
            }

            var appParamsResult = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
            if (appParamsResult.IsFailed)
            {
                return TypedResults.BadRequest("Impossible de récupérer les paramètres de l'application.");
            }

            var systeme = await context.Systeme.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (systeme == null)
            {
                return TypedResults.BadRequest("Paramètres système introuvables.");
            }

            if (string.IsNullOrWhiteSpace(systeme.MatriculeFiscale))
            {
                return TypedResults.BadRequest("Le matricule fiscal de l'entreprise n'est pas configuré.");
            }

            var matriculeNormalise = TryNormalizeMatriculeFiscal(systeme.MatriculeFiscale.Trim());
            if (matriculeNormalise == null)
            {
                return TypedResults.BadRequest(
                    "Le matricule fiscal de l'entreprise doit être au format 7 chiffres et une lettre clé (ex. 0001238L).");
            }

            var financialParams = await financialParametersService.GetAllFinancialParametersAsync(cancellationToken);
            if (financialParams == null)
            {
                return TypedResults.BadRequest("Aucun exercice comptable actif trouvé.");
            }

            var seuilRetenueSource = await financialParametersService.GetSeuilRetenueSourceAsync(1000, cancellationToken);

            var providerItems = new List<ProviderInvoiceTejItem>();
            var providerNums = request.ProviderInvoiceNumbers ?? new List<int>();
            if (providerNums.Count > 0)
            {
                var factures = await context.FactureFournisseur
                    .Include(f => f.IdFournisseurNavigation)
                    .Include(f => f.BonDeReception).ThenInclude(br => br.LigneBonReception)
                    .Include(f => f.FactureAvoirFournisseur).ThenInclude(fav => fav.AvoirFournisseur).ThenInclude(a => a.LigneAvoirFournisseur)
                    .Include(f => f.AvoirFinancierFournisseurs)
                    .Where(f => providerNums.Contains(f.Num))
                    .ToListAsync(cancellationToken);

                foreach (var ff in factures)
                {
                    var fournisseur = ff.IdFournisseurNavigation;
                    if (fournisseur == null) continue;
                    if (fournisseur.ExonereRetenueSource) continue;

                    var montantTTCBrut = ff.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc);
                    var totalAvoirsFinanciers = ff.AvoirFinancierFournisseurs?.Sum(a => a.TotTtc) ?? 0;
                    var totalFacturesAvoir = ff.FactureAvoirFournisseur?
                        .SelectMany(fav => fav.AvoirFournisseur)
                        .SelectMany(a => a.LigneAvoirFournisseur)
                        .Sum(l => l.TotTtc) ?? 0;
                    var montantAvantRetenue = montantTTCBrut - totalAvoirsFinanciers - totalFacturesAvoir;

                    if (montantAvantRetenue < seuilRetenueSource) continue;

                    if (string.IsNullOrWhiteSpace(fournisseur.Matricule) || string.IsNullOrWhiteSpace(fournisseur.Mail))
                        continue;

                    var telDigitsOnly = new string((fournisseur.Tel ?? "").Where(char.IsDigit).ToArray());
                    if (telDigitsOnly.Length != 8) continue;

                    var providerMatriculeNorm = TryNormalizeMatriculeFiscal(fournisseur.Matricule.Trim());
                    if (providerMatriculeNorm == null) continue;

                    var existingAttribution = await context.TejCertificatFacture
                        .FirstOrDefaultAsync(t => t.FactureFournisseurId == ff.Id, cancellationToken);

                    string refCertif;
                    if (existingAttribution != null)
                    {
                        refCertif = existingAttribution.RefCertif;
                    }
                    else
                    {
                        refCertif = await numberGeneratorService.GetNextTejCertificatRefAsync(ff.Date.Year, ff.Date.Month, cancellationToken);
                        context.TejCertificatFacture.Add(new TejCertificatFacture
                        {
                            FactureFournisseurId = ff.Id,
                            RefCertif = refCertif
                        });
                    }

                    providerItems.Add(new ProviderInvoiceTejItem(
                        ff, fournisseur,
                        montantAvantRetenue,
                        refCertif,
                        providerMatriculeNorm,
                        telDigitsOnly));
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            var depenseItems = new List<FactureDepenseTejItem>();
            var depenseIds = request.FactureDepenseIds ?? new List<int>();
            if (depenseIds.Count > 0)
            {
                var facturesDep = await context.FactureDepense
                    .Include(f => f.IdTiersDepenseFonctionnementNavigation)
                    .Where(f => depenseIds.Contains(f.Id))
                    .ToListAsync(cancellationToken);

                foreach (var fd in facturesDep)
                {
                    var tiers = fd.IdTiersDepenseFonctionnementNavigation;
                    if (tiers == null) continue;
                    if (tiers.ExonereRetenueSource) continue;
                    if (fd.MontantTotal < seuilRetenueSource) continue;

                    if (string.IsNullOrWhiteSpace(tiers.Matricule) || string.IsNullOrWhiteSpace(tiers.Mail))
                        continue;

                    var telDigitsOnly = new string((tiers.Tel ?? "").Where(char.IsDigit).ToArray());
                    if (telDigitsOnly.Length != 8) continue;

                    var tiersMatriculeNorm = TryNormalizeMatriculeFiscal(tiers.Matricule.Trim());
                    if (tiersMatriculeNorm == null) continue;

                    var existingAttribution = await context.TejCertificatFactureDepense
                        .FirstOrDefaultAsync(t => t.FactureDepenseId == fd.Id, cancellationToken);

                    string refCertif;
                    if (existingAttribution != null)
                    {
                        refCertif = existingAttribution.RefCertif;
                    }
                    else
                    {
                        refCertif = await numberGeneratorService.GetNextTejCertificatRefAsync(fd.Date.Year, fd.Date.Month, cancellationToken);
                        context.TejCertificatFactureDepense.Add(new TejCertificatFactureDepense
                        {
                            FactureDepenseId = fd.Id,
                            RefCertif = refCertif
                        });
                    }

                    depenseItems.Add(new FactureDepenseTejItem(
                        fd, tiers,
                        refCertif,
                        tiersMatriculeNorm,
                        telDigitsOnly));
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            if (providerItems.Count == 0 && depenseItems.Count == 0)
            {
                return TypedResults.BadRequest(
                    "Aucune facture éligible à l'export TEJ. Vérifiez les seuils, les exonérations et les données des fournisseurs/tiers (matricule, email, téléphone 8 chiffres).");
            }

            var xmlBytes = exportService.ExportMultipleToTejXml(
                providerItems,
                depenseItems,
                systeme,
                appParamsResult.Value,
                financialParams,
                matriculeNormalise,
                acteDepot,
                anneeDepot,
                moisDepot);

            var validationErrors = TejXsdValidator.Validate(xmlBytes)
                .Where(e => !e.Contains("introuvable", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (validationErrors.Count > 0)
            {
                logger.LogWarning("TEJ XSD validation failed: {Errors}", string.Join("; ", validationErrors));
                return TypedResults.BadRequest(
                    "Le fichier XML n'est pas conforme au schéma TEJ: " + string.Join("; ", validationErrors));
            }

            var filename = $"{matriculeNormalise}-{anneeDepot}-{moisDepot:D2}-{acteDepot}.xml";

            return TypedResults.File(
                xmlBytes,
                contentType: "application/xml; charset=utf-8",
                fileDownloadName: filename);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ExportAllToTejXmlEndpoint");
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
