using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.GetAccountingYearById;

public record GetAccountingYearByIdQuery(int Id) : IRequest<Result<AccountingYearResponse>>;
