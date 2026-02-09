using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.ImportBankTransactions;

public record ImportBankTransactionsCommand(
    int CompteBancaireId,
    Stream FileStream,
    string FileName) : IRequest<Result<ImportBankTransactionsResponse>>;
