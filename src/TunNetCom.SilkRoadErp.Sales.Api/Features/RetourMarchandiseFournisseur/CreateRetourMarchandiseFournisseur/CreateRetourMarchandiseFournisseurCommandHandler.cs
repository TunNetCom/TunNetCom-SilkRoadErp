using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur;

internal class CreateRetourMarchandiseFournisseurCommandHandler(
    SalesContext salesContext,
    ILogger<CreateRetourMarchandiseFournisseurCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService,
    NotificationService _notificationService)
    : IRequestHandler<CreateRetourMarchandiseFournisseurCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateRetourMarchandiseFournisseurCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await salesContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Get the active accounting year
            var activeAccountingYear = await salesContext.AccountingYear
                .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

            if (activeAccountingYear == null)
            {
                _logger.LogError("No active accounting year found");
                return Result.Fail("no_active_accounting_year");
            }

            // Verify provider exists
            var providerExists = await salesContext.Fournisseur
                .AnyAsync(f => f.Id == request.IdFournisseur, cancellationToken);
            if (!providerExists)
            {
                return Result.Fail("provider_not_found");
            }

            var num = await _numberGeneratorService.GenerateRetourMarchandiseFournisseurNumberAsync(activeAccountingYear.Id, cancellationToken);

            var retour = Domain.Entites.RetourMarchandiseFournisseur.CreateRetourMarchandiseFournisseur(
                num: num,
                date: request.Date,
                idFournisseur: request.IdFournisseur,
                accountingYearId: activeAccountingYear.Id,
                totHTva: 0m,
                totTva: 0m,
                netPayer: 0m
            );

            await salesContext.RetourMarchandiseFournisseur.AddAsync(retour, cancellationToken);
            
            // Save the retour first to get its Id
            await salesContext.SaveChangesAsync(cancellationToken);

            // Create the lines
            var retourLines = request.Lines.Select(x =>
                LigneRetourMarchandiseFournisseur.CreateRetourLine(
                    retourMarchandiseFournisseurId: retour.Id,
                    productRef: x.ProductRef,
                    designationLigne: x.Description,
                    quantity: x.Quantity,
                    unitPrice: x.UnitPrice,
                    discount: x.Discount,
                    tax: x.Tax)
            ).ToList();

            await salesContext.LigneRetourMarchandiseFournisseur.AddRangeAsync(retourLines, cancellationToken);

            // Calculate totals from lines
            var totHTva = DecimalHelper.RoundAmount(retourLines.Sum(l => l.TotHt));
            var totTva = DecimalHelper.RoundAmount(retourLines.Sum(l => l.TotTtc - l.TotHt));
            var netPayer = DecimalHelper.RoundAmount(retourLines.Sum(l => l.TotTtc));

            // Update retour with calculated totals
            retour.UpdateRetourMarchandiseFournisseur(
                num: num,
                date: request.Date,
                idFournisseur: request.IdFournisseur,
                accountingYearId: activeAccountingYear.Id,
                totHTva: totHTva,
                totTva: totTva,
                netPayer: netPayer
            );

            // Save the lines and updated totals
            await salesContext.SaveChangesAsync(cancellationToken);

            // Create notification
            var provider = await salesContext.Fournisseur
                .FirstOrDefaultAsync(f => f.Id == request.IdFournisseur, cancellationToken);
            
            var notificationData = new NotificationData
            {
                Type = Domain.Entites.NotificationType.SupplierReturn,
                Title = "Nouveau retour marchandise fournisseur",
                Message = $"Un retour marchandise a été créé pour le fournisseur {provider?.Nom ?? "Inconnu"} (Numéro: {num})",
                RelatedEntityId = retour.Id,
                RelatedEntityType = "RetourMarchandiseFournisseur"
            };

            await _notificationService.CreateNotificationsAsync(new List<NotificationData> { notificationData }, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Created RetourMarchandiseFournisseur with Id {Id} and Num {Num}", retour.Id, retour.Num);
            return Result.Ok(retour.Num);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error creating RetourMarchandiseFournisseur: {Message}. StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
            
            // Return more specific error messages
            if (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
            {
                return Result.Fail("database_tables_not_found: Please run the migration to create RetourMarchandiseFournisseur tables");
            }
            
            return Result.Fail($"error_creating_retour_marchandise_fournisseur: {ex.Message}");
        }
    }
}

