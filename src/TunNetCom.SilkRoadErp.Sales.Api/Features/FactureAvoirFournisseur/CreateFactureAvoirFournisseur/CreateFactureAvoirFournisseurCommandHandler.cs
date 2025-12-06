using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.CreateFactureAvoirFournisseur;

public class CreateFactureAvoirFournisseurCommandHandler(
    SalesContext _context,
    ILogger<CreateFactureAvoirFournisseurCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService)
    : IRequestHandler<CreateFactureAvoirFournisseurCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateFactureAvoirFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateFactureAvoirFournisseurCommand called with IdFournisseur {IdFournisseur} and Date {Date}", command.IdFournisseur, command.Date);

        var fournisseurExists = await _context.Fournisseur.AnyAsync(f => f.Id == command.IdFournisseur, cancellationToken);
        if (!fournisseurExists)
        {
            return Result.Fail("fournisseur_not_found");
        }

        if (command.NumFactureFournisseur.HasValue)
        {
            var factureFournisseurExists = await _context.FactureFournisseur.AnyAsync(f => f.Num == command.NumFactureFournisseur.Value, cancellationToken);
            if (!factureFournisseurExists)
            {
                return Result.Fail("facture_fournisseur_not_found");
            }
        }

        // Validate avoir fournisseurs exist and are not already linked (only if provided)
        List<Domain.Entites.AvoirFournisseur> avoirFournisseurs = new();
        if (command.AvoirFournisseurIds != null && command.AvoirFournisseurIds.Any())
        {
            avoirFournisseurs = await _context.AvoirFournisseur
                .Where(a => command.AvoirFournisseurIds.Contains(a.Num))
                .ToListAsync(cancellationToken);

            if (avoirFournisseurs.Count != command.AvoirFournisseurIds.Count)
            {
                var foundIds = avoirFournisseurs.Select(a => a.Num).ToList();
                var missingIds = command.AvoirFournisseurIds.Except(foundIds).ToList();
                _logger.LogWarning("AvoirFournisseurs not found: {Ids}", string.Join(", ", missingIds));
                return Result.Fail($"avoir_fournisseurs_not_found: {string.Join(", ", missingIds)}");
            }

            // Check if any avoir fournisseur is already linked to another facture avoir
            var alreadyLinked = avoirFournisseurs.Where(a => a.FactureAvoirFournisseurId.HasValue).ToList();
            if (alreadyLinked.Any())
            {
                var linkedIds = alreadyLinked.Select(a => a.Num).ToList();
                _logger.LogWarning("AvoirFournisseurs already linked: {Ids}", string.Join(", ", linkedIds));
                return Result.Fail($"avoir_fournisseurs_already_linked: {string.Join(", ", linkedIds)}");
            }
        }

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var num = await _numberGeneratorService.GenerateFactureAvoirFournisseurNumberAsync(activeAccountingYear.Id, cancellationToken);

        var factureAvoirFournisseur = Domain.Entites.FactureAvoirFournisseur.CreateFactureAvoirFournisseur(
            num, // NumFactureAvoirFourSurPage
            command.IdFournisseur,
            command.Date,
            command.NumFactureFournisseur,
            activeAccountingYear.Id);
        factureAvoirFournisseur.Num = num;

        _context.FactureAvoirFournisseur.Add(factureAvoirFournisseur);
        
        // Save to get the Id
        await _context.SaveChangesAsync(cancellationToken);

        // Link avoir fournisseurs to this facture avoir (if any) - use Id, not Num
        if (avoirFournisseurs.Any())
        {
            foreach (var avoirFournisseur in avoirFournisseurs)
            {
                avoirFournisseur.FactureAvoirFournisseurId = factureAvoirFournisseur.Id;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("FactureAvoirFournisseur created successfully with Num {Num}", factureAvoirFournisseur.Num);
        return Result.Ok(factureAvoirFournisseur.Num);
    }
}

