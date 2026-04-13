namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.TransferInvoiceToCustomer;

public class TransferInvoiceToCustomerValidator : AbstractValidator<TransferInvoiceToCustomerCommand>
{
    public TransferInvoiceToCustomerValidator()
    {
        _ = RuleFor(x => x.InvoiceNumber)
            .GreaterThan(0).WithMessage("invoice_number_must_be_greater_than_0");

        _ = RuleFor(x => x.TargetCustomerId)
            .GreaterThan(0).WithMessage("target_customer_id_must_be_greater_than_0");
    }
}

