using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.CreateAccountingYear;

public record CreateAccountingYearCommand(
    int Year,
    bool IsActive
) : IRequest<Result<AccountingYearResponse>>;
