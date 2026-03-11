namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;

public record BankTransactionSageLine(
    DateTime Date,
    string CompteGeneral,
    string NumPiece,
    string Libelle,
    decimal Debit,
    decimal Credit);
