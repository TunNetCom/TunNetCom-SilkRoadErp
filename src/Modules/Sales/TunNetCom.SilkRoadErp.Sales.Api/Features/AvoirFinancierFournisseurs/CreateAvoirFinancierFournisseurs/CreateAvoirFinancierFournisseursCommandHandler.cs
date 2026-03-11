using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.CreateAvoirFinancierFournisseurs;

public class CreateAvoirFinancierFournisseursCommandHandler(
    SalesContext _context,
    INumberGeneratorService _numberGeneratorService,
    ILogger<CreateAvoirFinancierFournisseursCommandHandler> _logger)
    : IRequestHandler<CreateAvoirFinancierFournisseursCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAvoirFinancierFournisseursCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateAvoirFinancierFournisseursCommand called with NumFactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);

        var factureFournisseur = await _context.FactureFournisseur
            .FirstOrDefaultAsync(f => f.Num == command.NumFactureFournisseur, cancellationToken);

        if (factureFournisseur == null)
        {
            _logger.LogWarning("FactureFournisseur with Num {Num} not found", command.NumFactureFournisseur);
            return Result.Fail("facture_fournisseur_not_found");
        }

        var num = await _numberGeneratorService.GenerateAvoirFinancierFournisseurNumAsync(factureFournisseur.AccountingYearId, cancellationToken);

        var avoirFinancier = new Domain.Entites.AvoirFinancierFournisseurs
        {
            Num = num,
            NumFactureFournisseur = command.NumFactureFournisseur,
            NumSurPage = command.NumSurPage,
            Date = command.Date,
            Description = command.Description,
            TotTtc = command.TotTtc
        };

        _context.AvoirFinancierFournisseurs.Add(avoirFinancier);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("AvoirFinancierFournisseurs created successfully with Num {Num}", avoirFinancier.Num);
        return Result.Ok(avoirFinancier.Num);
    }
}

