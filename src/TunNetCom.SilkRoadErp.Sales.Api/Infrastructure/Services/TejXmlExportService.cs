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
    /// Exporte une facture fournisseur au format XML TEJ
    /// </summary>
    public byte[] ExportProviderInvoiceToTejXml(
        FactureFournisseur factureFournisseur,
        Fournisseur fournisseur,
        Systeme systeme,
        GetAppParametersResponse appParams)
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

            // Calculate retenue source
            var tauxRS = (decimal)appParams.PourcentageRetenu;
            var montantRS = montantTTC * (tauxRS / 100);
            var montantNetServi = montantTTC - montantRS;

            // Determine VAT rate (use the highest rate found, or default to 19)
            var tauxTVA = DetermineVatRate(factureFournisseur, appParams);

            // Extract TypeIdentifiant and CategorieContribuable from MatriculeFiscale
            var (declarantTypeIdentifiant, declarantIdentifiant, declarantCategorie) = 
                ExtractIdentifiantInfo(systeme.MatriculeFiscale, systeme.CodeCategorie);

            var (beneficiaireTypeIdentifiant, beneficiaireIdentifiant, beneficiaireCategorie) = 
                ExtractIdentifiantInfo(fournisseur.Matricule, fournisseur.CodeCat);

            // Create XML document
            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
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
                        new XElement("MoisDepot", factureFournisseur.Date.Month)
                    ),
                    // AjouterCertificats
                    new XElement("AjouterCertificats",
                        new XElement("Certificat",
                            // Beneficiaire
                            new XElement("Beneficiaire",
                                new XElement("IdTaxpayer",
                                    new XElement("MatriculeFiscal",
                                        new XElement("TypeIdentifiant", beneficiaireTypeIdentifiant),
                                        new XElement("Identifiant", beneficiaireIdentifiant ?? ""),
                                        new XElement("CategorieContribuable", beneficiaireCategorie ?? "PM")
                                    )
                                ),
                                new XElement("Resident", "1"),
                                new XElement("NometprenonOuRaisonsociale", fournisseur.Nom ?? ""),
                                new XElement("Adresse", fournisseur.Adresse ?? ""),
                                new XElement("Activite", ""), // Not available in current schema
                                new XElement("InfosContact",
                                    new XElement("AdresseMail", fournisseur.Mail ?? ""),
                                    new XElement("NumTel", fournisseur.Tel ?? "")
                                )
                            ),
                            // DatePayement
                            new XElement("DatePayement", factureFournisseur.Date.ToString("dd/MM/yyyy")),
                            // Ref_certif_chez_declarant
                            new XElement("Ref_certif_chez_declarant", factureFournisseur.Num.ToString()),
                            // ListeOperations
                            new XElement("ListeOperations",
                                new XElement("Operation",
                                    new XAttribute("IdTypeOperation", "RS7_000002"),
                                    new XElement("AnneeFacturation", factureFournisseur.Date.Year),
                                    new XElement("CNPC", "0"),
                                    new XElement("P_Charge", "0"),
                                    new XElement("MontantHT", ConvertToCentimes(montantHT)),
                                    new XElement("TauxRS", tauxRS.ToString("F2")),
                                    new XElement("TauxTVA", tauxTVA.ToString("F0")),
                                    new XElement("MontantTVA", ConvertToCentimes(montantTVA)),
                                    new XElement("MontantTTC", ConvertToCentimes(montantTTC)),
                                    new XElement("MontantRS", ConvertToCentimes(montantRS)),
                                    new XElement("MontantNetServi", ConvertToCentimes(montantNetServi))
                                )
                            ),
                            // TotalPayement
                            new XElement("TotalPayement",
                                new XElement("TotalMontantHT", ConvertToCentimes(montantHT)),
                                new XElement("TotalMontantTVA", ConvertToCentimes(montantTVA)),
                                new XElement("TotalMontantTTC", ConvertToCentimes(montantTTC)),
                                new XElement("TotalMontantRS", ConvertToCentimes(montantRS)),
                                new XElement("TotalMontantNetServi", ConvertToCentimes(montantNetServi))
                            )
                        )
                    )
                )
            );

            // Convert to UTF-8 bytes
            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
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
    private decimal DetermineVatRate(FactureFournisseur factureFournisseur, GetAppParametersResponse appParams)
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

        // Default to highest configured rate
        return Math.Max(Math.Max(appParams.VatRate19, appParams.VatRate13), appParams.VatRate7);
    }

    /// <summary>
    /// Converts decimal amount to centimes (integer without decimals)
    /// </summary>
    private string ConvertToCentimes(decimal amount)
    {
        // Round to 2 decimal places first, then convert to centimes
        var rounded = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        var centimes = (long)(rounded * 100);
        return centimes.ToString();
    }
}

