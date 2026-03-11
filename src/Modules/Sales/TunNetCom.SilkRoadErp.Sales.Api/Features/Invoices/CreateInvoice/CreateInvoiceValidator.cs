namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator()
    {
        _ = RuleFor(x => x.Date)
            .NotEmpty().WithMessage("date_is_required");

        _ = RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("client_id_is_required");
    }
}
