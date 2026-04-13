namespace TunNetCom.SilkRoadErp.Sales.Api.Features.BankTransactions.ImportBankTransactions;

public class ImportBankTransactionsValidator : AbstractValidator<ImportBankTransactionsCommand>
{
    public ImportBankTransactionsValidator()
    {
        _ = RuleFor(x => x.CompteBancaireId).GreaterThan(0).WithMessage("compte_bancaire_is_required");
        _ = RuleFor(x => x.FileStream).NotNull().WithMessage("file_is_required");
        _ = RuleFor(x => x.FileName).NotEmpty().WithMessage("file_name_is_required").MaximumLength(500).WithMessage("file_name_max_500");
    }
}
