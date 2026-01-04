using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;

public class UpdateDeliveryNoteValidator : AbstractValidator<UpdateDeliveryNoteCommand>
{
    public UpdateDeliveryNoteValidator(IServiceProvider serviceProvider) 
    {
        _ = RuleFor(x => x.Date)
               .NotEmpty().WithMessage("date_is_required");

        _ = RuleFor(x => x.TotHTva)
               .GreaterThanOrEqualTo(0).WithMessage("tothtva_must_be_greater_than_or_equal_to_0");

        _ = RuleFor(x => x.TotTva)
               .GreaterThanOrEqualTo(0).WithMessage("tottva_must_be_greater_than_or_equal_to_0");

        _ = RuleFor(x => x.NetPayer)
                .GreaterThanOrEqualTo(0).WithMessage("netpayer_must_be_greater_than_or_equal_to_0");

        _ = RuleFor(x => x.TempBl)
               .NotEmpty().WithMessage("tempbl_is_required");

        // Valider NumFacture uniquement si le paramètre système le requiert
        _ = RuleFor(x => x.NumFacture)
               .NotNull().WithMessage("invoice_number_is_required")
               .GreaterThan(0).WithMessage("invoice_number_must_be_greater_than_0")
               .WhenAsync(async (command, cancellationToken) =>
               {
                   using var scope = serviceProvider.CreateScope();
                   var context = scope.ServiceProvider.GetRequiredService<SalesContext>();
                   var systeme = await context.Systeme.FirstOrDefaultAsync(cancellationToken);
                   return systeme?.BloquerBlSansFacture ?? false;
               });

        _ = RuleFor(x => x.ClientId)
                .GreaterThanOrEqualTo(0).WithMessage("clientid_must_be_greater_than_or_equal_to_0")
                .When(x => x.ClientId.HasValue);
    }
}

