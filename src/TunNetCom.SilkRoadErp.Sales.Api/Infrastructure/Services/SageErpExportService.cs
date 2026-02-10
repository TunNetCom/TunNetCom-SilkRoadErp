using System.Globalization;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;

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
    public byte[] ExportInvoicesToSageFormat(IEnumerable<InvoiceBaseInfo> invoices, decimal timbre, string journalCode = "VT")
    {
        var sb = new StringBuilder();
        
        // Ligne d'en-tête avec espacement exact
        sb.AppendLine("Code jDate dN° compte génN° pièce     Numéro facture   N° compte tiers  Libellé écriture                   Montant débit Montant crédit");

        var invoiceList = invoices.ToList();
        // Numéro de pièce par mois : M001, M002, ... (ex. mars → 3001, 3002)
        var monthCounters = new Dictionary<(int year, int month), int>();
        var pieceNumbers = new List<int>();
        foreach (var inv in invoiceList)
        {
            var key = (inv.Date.Year, inv.Date.Month);
            if (!monthCounters.TryGetValue(key, out var seq)) seq = 0;
            monthCounters[key] = seq + 1;
            pieceNumbers.Add(inv.Date.Month * 1000 + seq + 1);
        }

        for (var i = 0; i < invoiceList.Count; i++)
        {
            var lines = FormatInvoiceAccountingLines(invoiceList[i], timbre, journalCode, pieceNumbers[i]);
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
        }

        // Encodage UTF-8 pour l'export factures vente
        return Encoding.UTF8.GetBytes(sb.ToString());
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
    private List<string> FormatInvoiceAccountingLines(InvoiceBaseInfo invoice, decimal timbre, string journalCode, int pieceNumber)
    {
        var lines = new List<string>();
        var libelle = "FRE " + invoice.Number + " " + (invoice.CustomerName ?? "");
        var customerCode = invoice.CustomerCode ?? "";

        // Calcul des montants
        var ht = invoice.NetAmount - timbre; // HT = NetAmount - timbre
        var tva = invoice.VatAmount;
        var ttc = ht + tva + timbre; // TTC = HT + TVA + timbre

        // Ligne 1: HT en crédit (compte 70700019) — N° compte tiers vide
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            pieceNumber,
            invoice.CustomerId,
            libelle,
            0,
            ht,
            CompteHtVentes,
            tiersCode: null,
            amountWithDecimals: true));

        // Ligne 2: TVA en crédit (compte 43670019) — N° compte tiers vide
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            pieceNumber,
            invoice.CustomerId,
            libelle,
            0,
            tva,
            CompteTva,
            tiersCode: null,
            amountWithDecimals: true));

        // Ligne 3: DT (droits de timbre) en crédit (compte 43710000) — N° compte tiers vide
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            pieceNumber,
            invoice.CustomerId,
            libelle,
            0,
            timbre,
            CompteDroitsTimbre,
            tiersCode: null,
            amountWithDecimals: true));

        // Ligne 4: TTC en débit (compte 41100000) — N° compte tiers = code client
        lines.Add(FormatAccountingLine(
            journalCode,
            invoice.Date,
            invoice.Number,
            pieceNumber,
            invoice.CustomerId,
            libelle,
            ttc,
            0,
            CompteTtcClients,
            tiersCode: customerCode,
            amountWithDecimals: true));

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
        int pieceNumber,
        int accountId,
        string label,
        decimal debitAmount,
        decimal creditAmount,
        string? generalAccountCode = null,
        string? tiersCode = null,
        bool amountWithDecimals = false,
        string? libelleOverride = null)
    {
        var sb = new StringBuilder();

        // Code: 6 caractères (code journal - VT pour ventes, ACH pour achats)
        var code = journalCode.PadRight(6).Substring(0, 6);
        sb.Append(code);

        // jDate: 6 caractères (format jjmmaa = ddMMyy)
        var dateStr = date.ToString("ddMMyy");
        if (dateStr.Length > 6) dateStr = dateStr.Substring(0, 6);
        sb.Append(dateStr.PadLeft(6));

        // compte gnN: 10 caractères (compte général)
        var compteGen = (generalAccountCode ?? "").PadRight(10).Substring(0, 10);
        sb.Append(compteGen);

        // pièce: 16 caractères (numéro par mois pour ventes)
        var piece = pieceNumber.ToString().PadRight(16).Substring(0, 16);
        sb.Append(piece);

        // Numéro facture: 10 caractères (en caractères, reste rempli d'espaces)
        var numFacture = invoiceNumber.ToString().PadRight(10).Substring(0, 10);
        sb.Append(numFacture);

        // N° compte tiers: 16 caractères (vide sauf ligne 4 = code client pour ventes)
        var compteTiers = tiersCode != null
            ? (tiersCode ?? "").PadRight(16).Substring(0, 16)
            : accountId.ToString().PadLeft(16, '0');
        if (compteTiers.Length > 16) compteTiers = compteTiers.Substring(0, 16);
        sb.Append(compteTiers);

        // Libellé écriture: 40 caractères
        var libelle = (libelleOverride ?? label).PadRight(40);
        if (libelle.Length > 40) libelle = libelle.Substring(0, 40);
        sb.Append(libelle);

        // Montant débit / crédit
        var montantDebit = amountWithDecimals ? FormatAmountForInvoice(debitAmount) : FormatAmount(debitAmount, 28);
        var montantCredit = amountWithDecimals ? FormatAmountForInvoice(creditAmount) : FormatAmount(creditAmount, 28);
        sb.Append(montantDebit);
        sb.Append(montantCredit);

        return sb.ToString();
    }

    private const int InvoiceAmountWidth = 12;

    private static string FormatAmountForInvoice(decimal amount)
    {
        // 3 décimales, virgule, pas de zéros inutiles, aligné à droite avec espaces
        var formatted = amount.ToString("0.000", CultureInfo.GetCultureInfo("fr-FR")).Replace('.', ',');
        return formatted.PadLeft(InvoiceAmountWidth);
    }

    private string FormatAmount(decimal amount, int width)
    {
        // Convertir en centimes et formater sans séparateur de milliers
        var amountInCents = (long)(amount * 100);
        var amountStr = amountInCents.ToString();
        return amountStr.PadLeft(width, '0');
    }

    /// <summary>
    /// Exporte des écritures bancaires au format Sage ERP (journal BQ).
    /// Chaque ligne métier est doublée d'une contrepartie 53200000.
    /// </summary>
    public byte[] ExportBankTransactionsToSageFormat(IEnumerable<BankTransactionSageLine> lines, string journalCode = "BQ")
    {
        var sb = new StringBuilder();
        sb.AppendLine("Code jDate dN° compte génN° pièce     Numéro facture   N° compte tiers  Libellé écriture                   Montant débit Montant crédit");

        foreach (var line in lines)
        {
            sb.AppendLine(FormatBankAccountingLine(journalCode, line));
        }

        var encoding = Encoding.GetEncoding("Windows-1252");
        return encoding.GetBytes(sb.ToString());
    }

    private string FormatBankAccountingLine(string journalCode, BankTransactionSageLine line)
    {
        var date = new DateTimeOffset(line.Date, TimeSpan.Zero);
        var code = journalCode.PadRight(6).Substring(0, 6);
        var dateStr = date.ToString("ddMMyy");
        if (dateStr.Length > 6) dateStr = dateStr.Substring(0, 6);
        var dayOfMonth = date.Day.ToString().PadLeft(3, '0');
        var compteGen = (line.CompteGeneral ?? "").PadRight(10).Substring(0, 10);
        var piece = (line.NumPiece ?? "").PadRight(16).Substring(0, 16);
        var numFacture = "0".PadLeft(10, '0');
        var compteTiers = "0".PadLeft(16, '0');
        var libelle = (line.Libelle ?? "").PadRight(40);
        if (libelle.Length > 40) libelle = libelle.Substring(0, 40);
        var montantDebit = FormatAmount(line.Debit, 28);
        var montantCredit = FormatAmount(line.Credit, 28);
        return code + dateStr.PadLeft(6) + dayOfMonth + compteGen + piece + numFacture + compteTiers + libelle + montantDebit + montantCredit;
    }
}

