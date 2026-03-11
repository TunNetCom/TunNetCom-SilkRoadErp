using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.BankTransactions;

public interface IBankTransactionsApiClient
{
    Task<List<BankTransactionImportResponse>> GetImportsAsync(int? compteBancaireId, CancellationToken cancellationToken = default);
    Task<OneOf<ImportBankTransactionsResponse, BadRequestResponse>> ImportAsync(Stream fileStream, string fileName, int compteBancaireId, CancellationToken cancellationToken = default);
    Task<OneOf<byte[], bool>> ExportToSageAsync(int? importId, int? compteBancaireId, DateTime? dateDebut, DateTime? dateFin, CancellationToken cancellationToken = default);
}
