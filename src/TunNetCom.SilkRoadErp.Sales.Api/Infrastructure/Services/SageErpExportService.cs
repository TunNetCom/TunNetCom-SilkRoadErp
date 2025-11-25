using System.Text;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class SageErpExportService
{
    private readonly ILogger<SageErpExportService> _logger;

    public SageErpExportService(ILogger<SageErpExportService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Exporte une liste de factures au format Sage ERP (fichier texte à largeur fixe)
    /// </summary>
    public byte[] ExportInvoicesToSageFormat(IEnumerable<InvoiceBaseInfo> invoices, string journalCode = "VTE")
    {
        var sb = new StringBuilder();
        
        // Ligne d'en-tête
        sb.AppendLine("Code jDate dN compte gnN pice     Numro facture   N compte tiers  Libellcriture                   Montant dbit Montant crdit");

        // Lignes de données
        foreach (var invoice in invoices)
        {
            var line = FormatInvoiceLine(invoice, journalCode);
            sb.AppendLine(line);
        }

        // Encodage Windows-1252 pour compatibilité avec Sage ERP
        var encoding = Encoding.GetEncoding("Windows-1252");
        return encoding.GetBytes(sb.ToString());
    }

    /// <summary>
    /// Exporte une liste de factures fournisseurs au format Sage ERP (fichier texte à largeur fixe)
    /// </summary>
    public byte[] ExportProviderInvoicesToSageFormat(IEnumerable<ProviderInvoiceBaseInfo> invoices, string journalCode = "ACH")
    {
        var sb = new StringBuilder();
        
        // Ligne d'en-tête
        sb.AppendLine("Code jDate dN compte gnN pice     Numro facture   N compte tiers  Libellcriture                   Montant dbit Montant crdit");

        // Lignes de données
        foreach (var invoice in invoices)
        {
            var line = FormatProviderInvoiceLine(invoice, journalCode);
            sb.AppendLine(line);
        }

        // Encodage Windows-1252 pour compatibilité avec Sage ERP
        var encoding = Encoding.GetEncoding("Windows-1252");
        return encoding.GetBytes(sb.ToString());
    }

    private string FormatInvoiceLine(InvoiceBaseInfo invoice, string journalCode)
    {
        var sb = new StringBuilder();

        // Code: 6 caractères (code journal - VTE pour ventes, ACH pour achats)
        var code = journalCode.PadRight(6).Substring(0, 6);
        sb.Append(code);

        // jDate: 8 caractères (format DDMMYYYY)
        var dateStr = invoice.Date.ToString("ddMMyyyy");
        if (dateStr.Length > 8) dateStr = dateStr.Substring(0, 8);
        sb.Append(dateStr.PadLeft(8));

        // dN: 3 caractères (jour du mois)
        var dayOfMonth = invoice.Date.Day.ToString().PadLeft(3, '0');
        sb.Append(dayOfMonth);

        // compte gnN: 10 caractères (compte général - à configurer, par défaut vide)
        var compteGen = "".PadRight(10).Substring(0, 10);
        sb.Append(compteGen);

        // pièce: 16 caractères (référence document - on utilise le numéro de facture)
        var piece = invoice.Number.ToString().PadRight(16).Substring(0, 16);
        sb.Append(piece);

        // Numéro facture: 10 caractères
        var numFacture = invoice.Number.ToString().PadLeft(10, '0');
        if (numFacture.Length > 10) numFacture = numFacture.Substring(0, 10);
        sb.Append(numFacture);

        // N° compte tiers: 16 caractères (ID client formaté)
        var compteTiers = invoice.CustomerId.ToString().PadLeft(16, '0');
        if (compteTiers.Length > 16) compteTiers = compteTiers.Substring(0, 16);
        sb.Append(compteTiers);

        // Libellé écriture: 40 caractères (nom du client)
        var libelle = (invoice.CustomerName ?? "").PadRight(40);
        if (libelle.Length > 40) libelle = libelle.Substring(0, 40);
        sb.Append(libelle);

        // Montant débit: 28 caractères (montant HT aligné à droite, rempli de zéros)
        var montantDebit = FormatAmount(invoice.NetAmount, 28);
        sb.Append(montantDebit);

        // Montant crédit: 28 caractères (TVA alignée à droite, remplie de zéros)
        var montantCredit = FormatAmount(invoice.VatAmount, 28);
        sb.Append(montantCredit);

        return sb.ToString();
    }

    private string FormatProviderInvoiceLine(ProviderInvoiceBaseInfo invoice, string journalCode)
    {
        var sb = new StringBuilder();

        // Code: 6 caractères (code journal - ACH pour achats)
        var code = journalCode.PadRight(6).Substring(0, 6);
        sb.Append(code);

        // jDate: 8 caractères (format DDMMYYYY)
        var dateStr = invoice.Date.ToString("ddMMyyyy");
        if (dateStr.Length > 8) dateStr = dateStr.Substring(0, 8);
        sb.Append(dateStr.PadLeft(8));

        // dN: 3 caractères (jour du mois)
        var dayOfMonth = invoice.Date.Day.ToString().PadLeft(3, '0');
        sb.Append(dayOfMonth);

        // compte gnN: 10 caractères (compte général - à configurer, par défaut vide)
        var compteGen = "".PadRight(10).Substring(0, 10);
        sb.Append(compteGen);

        // pièce: 16 caractères (référence document - on utilise le numéro de facture)
        var piece = invoice.Number.ToString().PadRight(16).Substring(0, 16);
        sb.Append(piece);

        // Numéro facture: 10 caractères
        var numFacture = invoice.Number.ToString().PadLeft(10, '0');
        if (numFacture.Length > 10) numFacture = numFacture.Substring(0, 10);
        sb.Append(numFacture);

        // N° compte tiers: 16 caractères (ID fournisseur formaté)
        var compteTiers = invoice.ProviderId.ToString().PadLeft(16, '0');
        if (compteTiers.Length > 16) compteTiers = compteTiers.Substring(0, 16);
        sb.Append(compteTiers);

        // Libellé écriture: 40 caractères (nom du fournisseur - sera rempli par l'endpoint)
        var libelle = (invoice.ProviderName ?? "").PadRight(40);
        if (libelle.Length > 40) libelle = libelle.Substring(0, 40);
        sb.Append(libelle);

        // Montant débit: 28 caractères (montant HT aligné à droite, rempli de zéros)
        var montantDebit = FormatAmount(invoice.NetAmount, 28);
        sb.Append(montantDebit);

        // Montant crédit: 28 caractères (TVA alignée à droite, remplie de zéros)
        var montantCredit = FormatAmount(invoice.VatAmount, 28);
        sb.Append(montantCredit);

        return sb.ToString();
    }

    private string FormatAmount(decimal amount, int width)
    {
        // Convertir en centimes et formater sans séparateur de milliers
        var amountInCents = (long)(amount * 100);
        var amountStr = amountInCents.ToString();
        return amountStr.PadLeft(width, '0');
    }
}

