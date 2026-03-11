namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetUsers;

public class GetUsersValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must not exceed 100");
    }
}


