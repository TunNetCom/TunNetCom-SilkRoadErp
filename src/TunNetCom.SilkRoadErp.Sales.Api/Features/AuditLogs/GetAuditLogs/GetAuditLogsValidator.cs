namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AuditLogs.GetAuditLogs;

public class GetAuditLogsValidator : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must not exceed 100");

        RuleFor(x => x.DateFrom)
            .LessThanOrEqualTo(x => x.DateTo)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateFrom must be before or equal to DateTo");

        RuleFor(x => x.Action)
            .Must(BeValidAction)
            .When(x => !string.IsNullOrEmpty(x.Action))
            .WithMessage("Action must be one of: Created, Updated, Deleted");
    }

    private bool BeValidAction(string? action)
    {
        if (string.IsNullOrEmpty(action))
            return true;

        return Enum.TryParse<AuditAction>(action, true, out _);
    }
}

