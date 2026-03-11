#nullable enable
using System;
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class BankTransactionImport : ITenantEntity
{
    private BankTransactionImport()
    {
    }

    public static BankTransactionImport CreateBankTransactionImport(int compteBancaireId, string fileName)
    {
        return new BankTransactionImport
        {
            CompteBancaireId = compteBancaireId,
            FileName = fileName,
            ImportedAt = DateTime.UtcNow
        };
    }

    public int Id { get; private set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public int CompteBancaireId { get; private set; }

    public string FileName { get; private set; } = null!;

    public DateTime ImportedAt { get; private set; }

    public virtual CompteBancaire CompteBancaire { get; set; } = null!;

    public virtual ICollection<BankTransaction> BankTransaction { get; set; } = new List<BankTransaction>();
}
