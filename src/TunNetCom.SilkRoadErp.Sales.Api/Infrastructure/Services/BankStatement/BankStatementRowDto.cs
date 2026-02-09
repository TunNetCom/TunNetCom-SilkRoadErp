namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;

public class BankStatementRowDto
{
    public DateTime DateOperation { get; set; }
    public DateTime DateValeur { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
