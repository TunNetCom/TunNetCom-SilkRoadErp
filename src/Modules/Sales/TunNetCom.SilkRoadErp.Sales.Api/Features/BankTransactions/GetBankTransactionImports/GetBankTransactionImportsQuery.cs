using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.GetBankTransactionImports;

public record GetBankTransactionImportsQuery(int? CompteBancaireId) : IRequest<Result<List<BankTransactionImportResponse>>>;
