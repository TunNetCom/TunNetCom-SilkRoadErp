using TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.DeletePaiementClient;
using PaiementClientEntity = TunNetCom.SilkRoadErp.Sales.Domain.Entites.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.PaiementClient.DeletePaiementClient;

public class DeletePaiementClientCommandHandlerTest
{
    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"DeletePaiementClientTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static DeletePaiementClientCommandHandler CreateHandler(SalesContext context)
    {
        return new DeletePaiementClientCommandHandler(
            context,
            new Mock<ILogger<DeletePaiementClientCommandHandler>>().Object);
    }

    private static PaiementClientEntity CreatePaiement(int clientId = 1, int accountingYearId = 1)
    {
        return PaiementClientEntity.CreatePaiementClient(
            numeroTransactionBancaire: "TRX-001",
            clientId: clientId,
            accountingYearId: accountingYearId,
            montant: 100m,
            datePaiement: DateTime.UtcNow,
            methodePaiement: MethodePaiement.Cheque,
            factureIds: null,
            bonDeLivraisonIds: null,
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: null,
            documentStoragePath: null);
    }

    [Fact]
    public async Task Handle_WhenPaiementNotFound_ReturnsFailure()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(new DeletePaiementClientCommand(999), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "paiement_client_not_found");
    }

    [Fact]
    public async Task Handle_WhenPaiementExists_DeletesAndReturnsSuccess()
    {
        using var context = CreateContext();
        var paiement = CreatePaiement();
        _ = context.PaiementClient.Add(paiement);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new DeletePaiementClientCommand(paiement.Id), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        _ = context.PaiementClient.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenMultiplePaiements_DeletesOnlyRequested()
    {
        using var context = CreateContext();
        var p1 = CreatePaiement(clientId: 1);
        var p2 = CreatePaiement(clientId: 2);
        _ = context.PaiementClient.Add(p1);
        _ = context.PaiementClient.Add(p2);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(new DeletePaiementClientCommand(p1.Id), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var remaining = await context.PaiementClient.ToListAsync();
        _ = remaining.Should().ContainSingle(r => r.Id == p2.Id);
    }
}
