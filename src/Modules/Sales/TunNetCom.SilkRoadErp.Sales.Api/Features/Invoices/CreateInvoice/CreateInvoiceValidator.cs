namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator()
    {
        _ = RuleFor(x => x.Date)
            .NotEmpty().WithMessage("date_is_required");

        _ = RuleFor(x => x.Date.Date)
            .GreaterThanOrEqualTo(_ => DateTime.Today)
            .WithMessage("invoice_date_must_be_greater_or_equal_to_today");

        _ = RuleFor(x => x.Date.Date)
            .LessThanOrEqualTo(_ => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1))
            .WithMessage("invoice_date_must_not_exceed_current_month");

        _ = RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("client_id_is_required");
    }
}
