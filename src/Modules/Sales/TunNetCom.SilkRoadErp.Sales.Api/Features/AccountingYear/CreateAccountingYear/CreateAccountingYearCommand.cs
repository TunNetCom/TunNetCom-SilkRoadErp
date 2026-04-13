using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AccountingYear.CreateAccountingYear;

public record CreateAccountingYearCommand(
    int Year,
    bool IsActive,
    decimal? Timbre = null,
    decimal? PourcentageFodec = null,
    decimal? VatRate0 = null,
    decimal? VatRate7 = null,
    decimal? VatRate13 = null,
    decimal? VatRate19 = null,
    double? PourcentageRetenu = null,
    decimal? VatAmount = null,
    decimal? SeuilRetenueSource = null
) : IRequest<Result<AccountingYearResponse>>;
