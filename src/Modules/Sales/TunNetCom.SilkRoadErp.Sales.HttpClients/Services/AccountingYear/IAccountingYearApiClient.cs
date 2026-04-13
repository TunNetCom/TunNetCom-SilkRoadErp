using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AccountingYear;

public interface IAccountingYearApiClient
{
    Task<GetActiveAccountingYearResponse?> GetActiveAccountingYearAsync(CancellationToken cancellationToken = default);
    Task<List<GetAllAccountingYearsResponse>> GetAllAccountingYearsAsync(CancellationToken cancellationToken = default);
    Task<bool> SetActiveAccountingYearAsync(int accountingYearId, CancellationToken cancellationToken = default);
    Task<AccountingYearResponse?> GetAccountingYearByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAccountingYearAsync(CreateAccountingYearRequest request, CancellationToken cancellationToken = default);
    Task UpdateAccountingYearAsync(int id, UpdateAccountingYearRequest request, CancellationToken cancellationToken = default);
    Task DeleteAccountingYearAsync(int id, CancellationToken cancellationToken = default);
}

public record GetActiveAccountingYearResponse(
    int Id, 
    int Year, 
    bool IsActive,
    decimal? Timbre = null,
    decimal? PourcentageFodec = null,
    decimal? VatRate0 = null,
    decimal? VatRate7 = null,
    decimal? VatRate13 = null,
    decimal? VatRate19 = null,
    double? PourcentageRetenu = null);
public record GetAllAccountingYearsResponse(int Id, int Year, bool IsActive);

