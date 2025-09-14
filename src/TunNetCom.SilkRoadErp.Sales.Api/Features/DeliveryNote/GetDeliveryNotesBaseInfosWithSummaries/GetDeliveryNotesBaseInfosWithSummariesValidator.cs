namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBaseInfosWithSummaries;

public class GetDeliveryNotesBaseInfosWithSummariesValidator : AbstractValidator<GetDeliveryNotesBaseInfosWithSummariesQuery>
{
    public GetDeliveryNotesBaseInfosWithSummariesValidator()
    {
        //RuleFor(x => x.CustomerId)
        //    .NotEmpty().WithMessage("customer_id_is_required");

        _ = RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("page_number_must_be_greater_than_0");

        _ = RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("page_size_must_be_greater_than_0")
            .LessThanOrEqualTo(50).WithMessage("page_size_cannot_exceed_50");

        //RuleFor(x => x.IsInvoiced)
        //    .Equal(true)
        //    .When(x => x.InvoiceId.HasValue)
        //    .WithMessage("isInvoiced_must_be_true_when_invoice_id_has_a_value");

        //RuleFor(x => x.IsInvoiced)
        //    .Equal(false)
        //    .When(x => !x.InvoiceId.HasValue)
        //    .WithMessage("isInvoiced_must_be_false_when_invoice_id_is_null");
    }
}