namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.ExportBankTransactionsToSage;

public record ExportBankTransactionsToSageQuery(
    int? ImportId,
    int? CompteBancaireId,
    DateTime? DateDebut,
    DateTime? DateFin) : IRequest<Result<byte[]>>;
