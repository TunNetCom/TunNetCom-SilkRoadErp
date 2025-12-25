using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.UpdateAccountingYear;

public record UpdateAccountingYearCommand(
    int Id,
    int Year,
    bool IsActive
) : IRequest<Result<AccountingYearResponse>>;
