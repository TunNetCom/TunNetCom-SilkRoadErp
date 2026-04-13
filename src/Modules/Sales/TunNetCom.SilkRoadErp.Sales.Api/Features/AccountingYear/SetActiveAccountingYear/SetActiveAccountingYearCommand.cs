namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.SetActiveAccountingYear;

public record SetActiveAccountingYearCommand(int AccountingYearId) : IRequest<Result>;

