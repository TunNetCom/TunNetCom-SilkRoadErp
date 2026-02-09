namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;

public interface IBankStatementFileParser
{
    /// <summary>
    /// Parses a bank statement file (Excel or TSV) and returns the data rows.
    /// Expects columns: Date Opération, Date valeur, Opération, Référence, Débit, Crédit.
    /// </summary>
    Task<IReadOnlyList<BankStatementRowDto>> ParseAsync(Stream stream, string? fileName, CancellationToken cancellationToken = default);
}
