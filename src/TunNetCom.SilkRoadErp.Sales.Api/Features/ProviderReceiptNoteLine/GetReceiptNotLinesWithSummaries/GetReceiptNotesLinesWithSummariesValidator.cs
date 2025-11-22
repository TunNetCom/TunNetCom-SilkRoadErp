using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.GetReceiptNotLinesWithSummaries;

public class GetReceiptNotesLinesWithSummariesValidator : AbstractValidator<GetReceiptNoteLinesWithSummariesQueryParams>
{
    public GetReceiptNotesLinesWithSummariesValidator()
    {
        _ = RuleFor(x => x.PageNumber)
            .NotEmpty()
            .WithMessage("Page number is required");

        _ = RuleFor(x => x.PageSize).NotEmpty()
            .WithMessage("Page size is required")
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("Page size must be less than or equal to 50");
    }
}
