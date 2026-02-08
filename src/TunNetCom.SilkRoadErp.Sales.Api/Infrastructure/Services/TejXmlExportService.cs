using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class TejXmlExportService
{
    private readonly ILogger<TejXmlExportService> _logger;

    public TejXmlExportService(ILogger<TejXmlExportService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Exporte une facture fournisseur au format XML TEJ.
    /// </summary>
    /// <param name="normalizedBeneficiaireMatricule">Matricule fiscal bénéficiaire normalisé (7 chiffres + 1 lettre). Si fourni, utilisé pour IdTaxpayer/Identifiant.</param>
    public byte[] ExportProviderInvoiceToTejXml(
        FactureFournisseur factureFournisseur,
        Fournisseur fournisseur,
        Systeme systeme,
        GetAppParametersResponse appParams,
        AccountingYearFinancialParameters financialParams,
        string? normalizedBeneficiaireMatricule = null)
    {
        try
        {
            // Calculate totals from receipt notes
            var montantHT = factureFournisseur.BonDeReception
                .SelectMany(br => br.LigneBonReception)
                .Sum(l => l.TotHt);

            var montantTVA = factureFournisseur.BonDeReception
                .SelectMany(br => br.LigneBonReception)
                .Sum(l => l.TotTtc - l.TotHt);

            var montantTTC = factureFournisseur.BonDeReception
                .SelectMany(br => br.LigneBonReception)
                .Sum(l => l.TotTtc);

            // Get retenue source rate from financial parameters
            var tauxRS = financialParams.PourcentageRetenu;
            var montantRS = montantTTC * ((decimal)tauxRS / 100);
            var montantNetServi = montantTTC - montantRS;

            // Determine VAT rate (use the highest rate found, or default to 19)
            var tauxTVA = DetermineVatRate(factureFournisseur, financialParams);

            // Extract TypeIdentifiant and CategorieContribuable from MatriculeFiscale
            var (declarantTypeIdentifiant, declarantIdentifiant, declarantCategorie) = 
                ExtractIdentifiantInfo(systeme.MatriculeFiscale, systeme.CodeCategorie);

            var (beneficiaireTypeIdentifiant, _, beneficiaireCategorie) = 
                ExtractIdentifiantInfo(fournisseur.Matricule, fournisseur.CodeCat);
            var beneficiaireIdentifiant = normalizedBeneficiaireMatricule ?? fournisseur.Matricule?.Trim() ?? "";

            // Create XML document (CCT: prologue <?xml version="1.0" encoding="UTF-8"?> without standalone)
            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("DeclarationsRS",
                    new XAttribute("VersionSchema", "1.0"),
                    // Declarant
                    new XElement("Declarant",
                        new XElement("TypeIdentifiant", declarantTypeIdentifiant),
                        new XElement("Identifiant", declarantIdentifiant ?? ""),
                        new XElement("CategorieContribuable", declarantCategorie ?? "PM")
                    ),
                    // ReferenceDeclaration
                    new XElement("ReferenceDeclaration",
                        new XElement("ActeDepot", "0"),
                        new XElement("AnneeDepot", factureFournisseur.Date.Year),
                        new XElement("MoisDepot", factureFournisseur.Date.Month.ToString("D2"))
                    ),
                    // AjouterCertificats
                    new XElement("AjouterCertificats",
                        new XElement("Certificat",
                            // Beneficiaire
                            new XElement("Beneficiaire",
                                new XElement("IdTaxpayer",
                                    new XElement("MatriculeFiscal",
                                        new XElement("TypeIdentifiant", beneficiaireTypeIdentifiant),
                                        new XElement("Identifiant", beneficiaireIdentifiant),
                                        new XElement("CategorieContribuable", beneficiaireCategorie ?? "PM")
                                    )
                                ),
                                new XElement("Resident", "1"),
                                new XElement("NometprenonOuRaisonsociale", SanitizeTejTextField(fournisseur.Nom)),
                                new XElement("Adresse", SanitizeTejTextField(fournisseur.Adresse)),
                                new XElement("Activite", ""),
                                new XElement("InfosContact",
                                    new XElement("AdresseMail", SanitizeTejTextField(fournisseur.Mail)),
                                    new XElement("NumTel", SanitizeTejTextField(fournisseur.Tel))
                                )
                            ),
                            // DatePayement
                            new XElement("DatePayement", factureFournisseur.Date.ToString("dd/MM/yyyy")),
                            // Ref_certif_chez_declarant (numeric id; sanitize in case of custom ref in future)
                            new XElement("Ref_certif_chez_declarant", SanitizeTejTextField(factureFournisseur.Num.ToString())),
                            // ListeOperations
                            new XElement("ListeOperations",
                                new XElement("Operation",
                                    new XAttribute("IdTypeOperation", "RS7_000002"),
                                    new XElement("AnneeFacturation", factureFournisseur.Date.Year),
                                    new XElement("CNPC", "0"),
                                    new XElement("P_Charge", "0"),
                                    new XElement("MontantHT", ConvertToMillimes(montantHT)),
                                    new XElement("TauxRS", tauxRS.ToString("F2")),
                                    new XElement("TauxTVA", tauxTVA.ToString("F2")),
                                    new XElement("MontantTVA", ConvertToMillimes(montantTVA)),
                                    new XElement("MontantTTC", ConvertToMillimes(montantTTC)),
                                    new XElement("MontantRS", ConvertToMillimes(montantRS)),
                                    new XElement("MontantNetServi", ConvertToMillimes(montantNetServi))
                                )
                            ),
                            // TotalPayement
                            new XElement("TotalPayement",
                                new XElement("TotalMontantHT", ConvertToMillimes(montantHT)),
                                new XElement("TotalMontantTVA", ConvertToMillimes(montantTVA)),
                                new XElement("TotalMontantTTC", ConvertToMillimes(montantTTC)),
                                new XElement("TotalMontantRS", ConvertToMillimes(montantRS)),
                                new XElement("TotalMontantNetServi", ConvertToMillimes(montantNetServi))
                            )
                        )
                    )
                )
            );

            // Convert to UTF-8 bytes; ensure prologue is exactly <?xml version="1.0" encoding="UTF-8"?> (no standalone) per CCT
            using var ms = new MemoryStream();
            doc.Save(ms);
            var bytes = ms.ToArray();
            return RemoveStandaloneFromPrologue(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating TEJ XML for invoice {InvoiceNumber}", factureFournisseur.Num);
            throw;
        }
    }

    /// <summary>
    /// Extracts TypeIdentifiant, Identifiant, and CategorieContribuable from a matricule string
    /// </summary>
    private (int TypeIdentifiant, string? Identifiant, string? CategorieContribuable) ExtractIdentifiantInfo(
        string? matricule, 
        string? codeCategorie)
    {
        if (string.IsNullOrWhiteSpace(matricule))
        {
            return (1, null, codeCategorie ?? "PM");
        }

        // Default TypeIdentifiant is 1 (could be enhanced based on business rules)
        var typeIdentifiant = 1;
        
        // Use CodeCategorie if available, otherwise default to PM
        var categorie = codeCategorie ?? "PM";

        return (typeIdentifiant, matricule.Trim(), categorie);
    }

    /// <summary>
    /// Determines the VAT rate from the invoice lines
    /// Uses the highest VAT rate found, or defaults to 19%
    /// </summary>
    private decimal DetermineVatRate(FactureFournisseur factureFournisseur, AccountingYearFinancialParameters financialParams)
    {
        var vatRates = factureFournisseur.BonDeReception
            .SelectMany(br => br.LigneBonReception)
            .Where(l => l.Tva > 0)
            .Select(l => (decimal)l.Tva)
            .Distinct()
            .ToList();

        if (vatRates.Any())
        {
            return vatRates.Max();
        }

        // Default to highest configured rate from financial parameters
        return Math.Max(Math.Max(financialParams.VatRate19, financialParams.VatRate13), financialParams.VatRate7);
    }

    /// <summary>
    /// Removes standalone attribute from XML declaration so output matches CCT: <?xml version="1.0" encoding="UTF-8"?>
    /// </summary>
    private static byte[] RemoveStandaloneFromPrologue(byte[] utf8Bytes)
    {
        var declaration = Encoding.UTF8.GetString(utf8Bytes);
        if (declaration.StartsWith("<?xml ", StringComparison.Ordinal) &&
            (declaration.Contains(" standalone=\"yes\"", StringComparison.Ordinal) || declaration.Contains(" standalone='yes'", StringComparison.Ordinal)))
        {
            declaration = declaration
                .Replace(" standalone=\"yes\"", string.Empty, StringComparison.Ordinal)
                .Replace(" standalone='yes'", string.Empty, StringComparison.Ordinal);
            return Encoding.UTF8.GetBytes(declaration);
        }
        return utf8Bytes;
    }

    /// <summary>
    /// Sanitizes text for TEJ XML per CCT section 6: avoid special chars (é, è, à, ;, *, &),
    /// forbidden sequences (--, /*, &#, 'OR, 'AND). Replaces accented chars with ASCII equivalents.
    /// </summary>
    private static string SanitizeTejTextField(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var s = value.Trim();

        // Remove forbidden sequences (case-insensitive for 'OR and 'AND)
        s = Regex.Replace(s, @"'OR\b", string.Empty, RegexOptions.IgnoreCase);
        s = Regex.Replace(s, @"'AND\b", string.Empty, RegexOptions.IgnoreCase);
        s = s.Replace("--", "-");
        s = s.Replace("/*", string.Empty);
        s = s.Replace("&#", string.Empty);

        // Replace or remove forbidden characters
        s = s.Replace("&", " et ");
        s = s.Replace(";", ",");
        s = s.Replace("*", string.Empty);

        // Normalize accented characters to ASCII (CCT: avoid é, è, à, etc.)
        s = NormalizeToAscii(s);

        // Collapse multiple spaces
        s = Regex.Replace(s, @"\s+", " ");
        return s.Trim();
    }

    /// <summary>
    /// Replaces common accented characters with ASCII equivalents for TEJ CCT compliance.
    /// </summary>
    private static string NormalizeToAscii(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category == UnicodeCategory.NonSpacingMark)
                continue;
            if (c == 'é' || c == 'è' || c == 'ê' || c == 'ë') { sb.Append('e'); continue; }
            if (c == 'à' || c == 'â' || c == 'ä') { sb.Append('a'); continue; }
            if (c == 'ù' || c == 'û' || c == 'ü') { sb.Append('u'); continue; }
            if (c == 'ô' || c == 'ö') { sb.Append('o'); continue; }
            if (c == 'î' || c == 'ï') { sb.Append('i'); continue; }
            if (c == 'ç') { sb.Append('c'); continue; }
            if (c == 'ÿ') { sb.Append('y'); continue; }
            sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Converts decimal amount (dinars) to millimes (1 dinar = 1000 millimes) per CCT.
    /// </summary>
    private static string ConvertToMillimes(decimal amount)
    {
        var rounded = Math.Round(amount, 3, MidpointRounding.AwayFromZero);
        var millimes = (long)(rounded * 1000);
        return millimes.ToString();
    }
}

