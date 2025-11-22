namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AccountingYear;

public interface IAccountingYearApiClient
{
    Task<GetActiveAccountingYearResponse?> GetActiveAccountingYearAsync(CancellationToken cancellationToken = default);
    Task<List<GetAllAccountingYearsResponse>> GetAllAccountingYearsAsync(CancellationToken cancellationToken = default);
    Task<bool> SetActiveAccountingYearAsync(int accountingYearId, CancellationToken cancellationToken = default);
}

public record GetActiveAccountingYearResponse(int Id, int Year, bool IsActive);
public record GetAllAccountingYearsResponse(int Id, int Year, bool IsActive);

