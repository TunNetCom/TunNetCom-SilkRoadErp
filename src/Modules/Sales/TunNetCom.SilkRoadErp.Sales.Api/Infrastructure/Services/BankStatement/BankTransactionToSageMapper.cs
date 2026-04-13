using System.Text.RegularExpressions;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;

/// <summary>
/// Maps bank transactions to Sage accounting lines per accountant rules:
/// Encaissements -> 41110000 (ENC CHQ, ENC EFFET, etc.)
/// Décaissements chèque -> 40110000 CHQ ** (N°CHQ)
/// Décaissements effet -> 40110000 DOMICIL EFFET ** (5 last ref)
/// Cartes/autres -> 40110000 libellé en majuscules
/// Commissions -> 62720000 COMM/CHQ, COMM/EFFET, FRAIS PDL
/// Contrepartie -> 53200000 for each line
/// </summary>
public static class BankTransactionToSageMapper
{
    private const string CompteEncaissement = "41110000";
    private const string CompteDecaissement = "40110000";
    private const string CompteCommissions = "62720000";
    private const string CompteContrepartie = "53200000";

    public static List<BankTransactionSageLine> MapToSageLines(IEnumerable<BankTransaction> transactions)
    {
        var result = new List<BankTransactionSageLine>();
        foreach (var tx in transactions)
        {
            var (compte, libelle) = Classify(tx);
            var date = new DateTimeOffset(tx.DateOperation, TimeSpan.Zero);
            var numPiece = (tx.Reference ?? "").PadRight(16).Substring(0, 16);
            var libelleUpper = (libelle ?? "").ToUpperInvariant().PadRight(40).Substring(0, 40);

            if (tx.Credit > 0)
            {
                result.Add(new BankTransactionSageLine(tx.DateOperation, compte, numPiece, libelleUpper, 0, tx.Credit));
                result.Add(new BankTransactionSageLine(tx.DateOperation, CompteContrepartie, numPiece, libelleUpper, tx.Credit, 0));
            }
            else if (tx.Debit > 0)
            {
                result.Add(new BankTransactionSageLine(tx.DateOperation, compte, numPiece, libelleUpper, tx.Debit, 0));
                result.Add(new BankTransactionSageLine(tx.DateOperation, CompteContrepartie, numPiece, libelleUpper, 0, tx.Debit));
            }
        }
        return result;
    }

    private static (string Compte, string Libelle) Classify(BankTransaction tx)
    {
        var op = (tx.Operation ?? "").ToUpperInvariant();
        var refStr = (tx.Reference ?? "").Trim();

        if (tx.Credit > 0)
        {
            if (op.Contains("ENC CHEQ") || op.Contains("ENC CHEQUE"))
            {
                return (CompteEncaissement, "ENC CHQ");
            }
            if (op.Contains("EFFET") && op.Contains("ENC"))
            {
                return (CompteEncaissement, "ENC EFFET");
            }
            if (op.Contains("VERSEMENT ESPECES"))
            {
                return (CompteEncaissement, "VERSEMENT ESPECES");
            }
            return (CompteEncaissement, op.Length > 40 ? op.Substring(0, 40) : op);
        }

        if (tx.Debit > 0)
        {
            if (op.Contains("COMM ENC CHEQUE") || op.Contains("COM ENC CHEQUE"))
            {
                return (CompteCommissions, "COMM/CHQ");
            }
            if (op.Contains("COMM REGLEMENT EFFET") || op.Contains("COMMISSION") && op.Contains("EFFET"))
            {
                return (CompteCommissions, "COMM/EFFET");
            }
            if (op.Contains("FRAIS PDL"))
            {
                return (CompteCommissions, "FRAIS PDL");
            }
            if (op.Contains("COMMISSION TAWASSOL") || op.Contains("TVA") && refStr.StartsWith("CHG"))
            {
                return (CompteCommissions, op.Length > 40 ? op.Substring(0, 40) : op);
            }
            if (op.Contains("REGLEMENT CHEQUE") || op.Contains("CHEQUE"))
            {
                var numChq = ExtractChequeNumber(op, refStr);
                return (CompteDecaissement, $"CHQ ** ({numChq})");
            }
            if (op.Contains("PAIEMENT EFFET"))
            {
                var fiveLast = refStr.Length >= 5 ? refStr.Substring(refStr.Length - 5) : refStr;
                return (CompteDecaissement, $"DOMICIL EFFET ** {fiveLast}");
            }
            return (CompteDecaissement, op.Length > 40 ? op.Substring(0, 40) : op);
        }

        return (CompteContrepartie, op);
    }

    private static string ExtractChequeNumber(string operation, string reference)
    {
        var match = Regex.Match(operation, @"\d{6,}");
        if (match.Success)
        {
            return match.Value;
        }
        if (!string.IsNullOrEmpty(reference))
        {
            return reference.Length > 10 ? reference.Substring(0, 10) : reference;
        }
        return "N°CHQ";
    }
}
