using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.UpdateFactureDepense;

public class UpdateFactureDepenseCommandHandler(
    SalesContext _context,
    ILogger<UpdateFactureDepenseCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<UpdateFactureDepenseCommand, Result>
{
    public async Task<Result> Handle(UpdateFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.FactureDepense.FindAsync(new object[] { command.Id }, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("FactureDepense not found: Id {Id}", command.Id);
            return Result.Fail(EntityNotFound.Error());
        }
        if (entity.Statut != DocumentStatus.Draft)
        {
            _logger.LogWarning("FactureDepense cannot be updated when not Draft: Id {Id}", command.Id);
            return Result.Fail("facture_depense_not_draft");
        }

        string? documentStoragePath = entity.DocumentStoragePath;
        if (!string.IsNullOrWhiteSpace(command.DocumentBase64))
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(entity.DocumentStoragePath))
                {
                    try
                    {
                        await _documentStorageService.DeleteAsync(entity.DocumentStoragePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old document, continuing with new document");
                    }
                }
                var documentBytes = Convert.FromBase64String(command.DocumentBase64);
                var fileName = $"facture_depense_{entity.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}";
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

        var (baseHT0, montantTVA0, baseHT7, montantTVA7, baseHT13, montantTVA13, baseHT19, montantTVA19) = GetLignesFromDto(command.LignesTVA);
        entity.Update(command.Date, command.Description ?? string.Empty, command.MontantTotal, documentStoragePath, baseHT0, montantTVA0, baseHT7, montantTVA7, baseHT13, montantTVA13, baseHT19, montantTVA19);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("FactureDepense updated: Id {Id}", command.Id);
        return Result.Ok();
    }

    private static (decimal baseHT0, decimal montantTVA0, decimal baseHT7, decimal montantTVA7, decimal baseHT13, decimal montantTVA13, decimal baseHT19, decimal montantTVA19) GetLignesFromDto(List<FactureDepenseLigneTvaDto>? lignes)
    {
        decimal b0 = 0, t0 = 0, b7 = 0, t7 = 0, b13 = 0, t13 = 0, b19 = 0, t19 = 0;
        if (lignes != null)
        {
            foreach (var l in lignes)
            {
                if (l.TauxTVA == 0) { b0 = l.BaseHT; t0 = l.MontantTVA; }
                else if (l.TauxTVA == 7) { b7 = l.BaseHT; t7 = l.MontantTVA; }
                else if (l.TauxTVA == 13) { b13 = l.BaseHT; t13 = l.MontantTVA; }
                else if (l.TauxTVA == 19) { b19 = l.BaseHT; t19 = l.MontantTVA; }
            }
        }
        return (b0, t0, b7, t7, b13, t13, b19, t19);
    }
}
