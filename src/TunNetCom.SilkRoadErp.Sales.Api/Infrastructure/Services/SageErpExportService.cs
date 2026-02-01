namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class SageErpExportService
{
    private const string CompteHtVentes = "70700019";
    private const string CompteTva = "43670019";
    private const string CompteDroitsTimbre = "43710000";
    private const string CompteTtcClients = "41100000";

    private readonly ILogger<SageErpExportService> _logger;

    public SageErpExportService(ILogger<SageErpExportService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Exporte une liste de factures au format Sage ERP (fichier texte à largeur fixe)
    /// </summary>
    public byte[] ExportInvoicesToSageFormat(IEnumerable<InvoiceBaseInfo> invoices, decimal timbre, string journalCode = "VTE")
    {
        var sb = new StringBuilder();
        
        // Ligne d'en-tête avec espacement exact
        sb.AppendLine("Code jDate dN° compte génN° pièce     Numéro facture   N° compte tiers  Libellé écriture                   Montant débit Montant crédit");

        // Lignes de données - 4 lignes par facture
        foreach (var invoice in invoices)
        {
            var lines = FormatInvoiceAccountingLines(invoice, timbre, journalCode);
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
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
        
        // Ligne d'en-tête avec espacement exact
        sb.AppendLine("Code jDate dN° compte génN° pièce     Numéro facture   N° compte tiers  Libellé écriture                   Montant débit Montant crédit");

        // Lignes de données - 4 lignes par facture
        foreach (var invoice in invoices)
        {
            var lines = FormatProviderInvoiceAccountingLines(invoice, journalCode);
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
        }

        // Encodage Windows-1252 pour compatibilité avec Sage ERP
        var encoding = Encoding.GetEncoding("Windows-1252");
        return encoding.GetBytes(sb.ToString());
    }

    /// <summary>
    /// Génère les 4 lignes d'écriture comptable pour une facture client
    /// </summary>
    private List<string> FormatInvoiceAccountingLines(InvoiceBaseInfo invoice, decimal timbre, string journalCode)
    {
        var lines = new List<string>();
        
        // Calcul des montants
        var ht = invoice.NetAmount - timbre; // HT = NetAmount - timbre
        var tva = invoice.VatAmount;
        var ttc = ht + tva + timbre; // TTC = HT + TVA + timbre

        // Ligne 1: HT en crédit (compte 70700019)
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.CustomerId,
            invoice.CustomerName ?? "",
            0,
            ht,
            CompteHtVentes));

        // Ligne 2: TVA en crédit (compte 43670019)
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.CustomerId,
            invoice.CustomerName ?? "",
            0,
            tva,
            CompteTva));

        // Ligne 3: DT (droits de timbre) en crédit (compte 43710000)
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.CustomerId,
            invoice.CustomerName ?? "",
            0,
            timbre,
            CompteDroitsTimbre));

        // Ligne 4: TTC en débit (compte 41100000)
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.CustomerId,
            invoice.CustomerName ?? "",
            ttc,
            0,
            CompteTtcClients));

        return lines;
    }

    /// <summary>
    /// Génère les 4 lignes d'écriture comptable pour une facture fournisseur
    /// </summary>
    private List<string> FormatProviderInvoiceAccountingLines(ProviderInvoiceBaseInfo invoice, string journalCode)
    {
        var lines = new List<string>();
        
        // Calcul des montants
        var ht = invoice.NetAmount;
        var tva = invoice.VatAmount;
        var ttc = ht + tva; // TTC = HT + TVA
        var timbre = 0m; // Pas de timbre pour les factures fournisseurs

        // Ligne 1: HT en débit
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.ProviderId,
            invoice.ProviderName ?? "",
            ht,
            0));

        // Ligne 2: TVA en débit
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.ProviderId,
            invoice.ProviderName ?? "",
            tva,
            0));

        // Ligne 3: TTC en crédit
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.ProviderId,
            invoice.ProviderName ?? "",
            0,
            ttc));

        // Ligne 4: Timbre (0) en débit
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            invoice.ProviderId,
            invoice.ProviderName ?? "",
            timbre,
            0));

        return lines;
    }

    /// <summary>
    /// Formate une ligne d'écriture comptable avec les montants débit/crédit spécifiés
    /// </summary>
    private string FormatAccountingLine(
        string journalCode,
        DateTimeOffset date,
        int invoiceNumber,
        int accountId,
        string label,
        decimal debitAmount,
        decimal creditAmount,
        string? generalAccountCode = null)
    {
        var sb = new StringBuilder();

        // Code: 6 caractères (code journal - VTE pour ventes, ACH pour achats)
        var code = journalCode.PadRight(6).Substring(0, 6);
        sb.Append(code);

        // jDate: 6 caractères (format jjmmaa = ddMMyy)
        var dateStr = date.ToString("ddMMyy");
        if (dateStr.Length > 6) dateStr = dateStr.Substring(0, 6);
        sb.Append(dateStr.PadLeft(6));

        // dN: 3 caractères (jour du mois)
        var dayOfMonth = date.Day.ToString().PadLeft(3, '0');
        sb.Append(dayOfMonth);

        // compte gnN: 10 caractères (compte général)
        var compteGen = (generalAccountCode ?? "").PadRight(10).Substring(0, 10);
        sb.Append(compteGen);

        // pièce: 16 caractères (référence document - on utilise le numéro de facture)
        var piece = invoiceNumber.ToString().PadRight(16).Substring(0, 16);
        sb.Append(piece);

        // Numéro facture: 10 caractères
        var numFacture = invoiceNumber.ToString().PadLeft(10, '0');
        if (numFacture.Length > 10) numFacture = numFacture.Substring(0, 10);
        sb.Append(numFacture);

        // N° compte tiers: 16 caractères (ID formaté)
        var compteTiers = accountId.ToString().PadLeft(16, '0');
        if (compteTiers.Length > 16) compteTiers = compteTiers.Substring(0, 16);
        sb.Append(compteTiers);

        // Libellé écriture: 40 caractères
        var libelle = label.PadRight(40);
        if (libelle.Length > 40) libelle = libelle.Substring(0, 40);
        sb.Append(libelle);

        // Montant débit: 28 caractères (aligné à droite, rempli de zéros)
        var montantDebit = FormatAmount(debitAmount, 28);
        sb.Append(montantDebit);

        // Montant crédit: 28 caractères (aligné à droite, rempli de zéros)
        var montantCredit = FormatAmount(creditAmount, 28);
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

