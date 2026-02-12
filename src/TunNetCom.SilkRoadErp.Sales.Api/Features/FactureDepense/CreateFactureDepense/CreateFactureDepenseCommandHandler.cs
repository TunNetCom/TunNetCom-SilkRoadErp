using Domain = TunNetCom.SilkRoadErp.Sales.Domain;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.CreateFactureDepense;

public class CreateFactureDepenseCommandHandler(
    SalesContext _context,
    ILogger<CreateFactureDepenseCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<CreateFactureDepenseCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        var tiersExists = await _context.TiersDepenseFonctionnement.AnyAsync(t => t.Id == command.IdTiersDepenseFonctionnement, cancellationToken);
        if (!tiersExists)
        {
            _logger.LogWarning("TiersDepenseFonctionnement not found: Id {Id}", command.IdTiersDepenseFonctionnement);
            return Result.Fail(EntityNotFound.Error());
        }

        var accountingYearId = command.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYear = await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);
            if (activeYear == null)
            {
                _logger.LogError("No active accounting year found");
                return Result.Fail("no_active_accounting_year");
            }
            accountingYearId = activeYear.Id;
        }

        string? documentStoragePath = null;
        if (!string.IsNullOrWhiteSpace(command.DocumentBase64))
        {
            try
            {
                var documentBytes = Convert.FromBase64String(command.DocumentBase64);
                var fileName = $"facture_depense_{DateTime.UtcNow:yyyyMMddHHmmss}";
                documentStoragePath = await _documentStorageService.SaveAsync(documentBytes, fileName, cancellationToken);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 format for document");
                return Result.Fail("invalid_document_format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing document");
                return Result.Fail("error_storing_document");
            }
        }

        var num = await _numberGeneratorService.GenerateFactureDepenseNumberAsync(accountingYearId.Value, cancellationToken);

        var entity = Domain.Entites.FactureDepense.Create(
            num,
            command.IdTiersDepenseFonctionnement,
            command.Date,
            command.Description ?? string.Empty,
            command.MontantTotal,
            accountingYearId.Value,
            documentStoragePath);

        _context.FactureDepense.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("FactureDepense created with Id {Id}, Num {Num}", entity.Id, entity.Num);
        return Result.Ok(entity.Id);
    }
}
