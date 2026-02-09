#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class BankTransaction
{
    private BankTransaction()
    {
    }

    public static BankTransaction CreateBankTransaction(
        int bankTransactionImportId,
        DateTime dateOperation,
        DateTime dateValeur,
        string operation,
        string reference,
        decimal debit,
        decimal credit)
    {
        return new BankTransaction
        {
            BankTransactionImportId = bankTransactionImportId,
            DateOperation = dateOperation,
            DateValeur = dateValeur,
            Operation = operation,
            Reference = reference,
            Debit = debit,
            Credit = credit
        };
    }

    public int Id { get; private set; }

    public int BankTransactionImportId { get; private set; }

    public DateTime DateOperation { get; private set; }

    public DateTime DateValeur { get; private set; }

    public string Operation { get; private set; } = null!;

    public string Reference { get; private set; } = null!;

    public decimal Debit { get; private set; }

    public decimal Credit { get; private set; }

    public string? SageCompteGeneral { get; private set; }

    public string? SageLibelle { get; private set; }

    public virtual BankTransactionImport BankTransactionImport { get; set; } = null!;
}
