using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.ValidateDeliveryNotes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.DeliveryNote.ValidateDeliveryNotes;

public class ValidateDeliveryNotesCommandHandlerTest
{
    private static SalesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: $"ValidateDeliveryNotesTest_{Guid.NewGuid()}")
            .Options;
        return new SalesContext(options);
    }

    private static ValidateDeliveryNotesCommandHandler CreateHandler(SalesContext context)
    {
        return new ValidateDeliveryNotesCommandHandler(
            context,
            new Mock<ILogger<ValidateDeliveryNotesCommandHandler>>().Object);
    }

    private static BonDeLivraison CreateDeliveryNote(int num, int clientId = 1, int accountingYearId = 1)
    {
        return new BonDeLivraison
        {
            Num = num,
            Date = new DateTime(2024, 6, 1),
            NetPayer = 119m,
            TempBl = new TimeOnly(10, 0),
            ClientId = clientId,
            AccountingYearId = accountingYearId
        };
    }

    [Fact]
    public async Task Handle_WhenNoIdsProvided_ReturnsFailure()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(new ValidateDeliveryNotesCommand(new List<int>()), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "No ids provided");
    }

    [Fact]
    public async Task Handle_WhenNullIdsProvided_ReturnsFailure()
    {
        using var context = CreateContext();
        var handler = CreateHandler(context);

        var result = await handler.Handle(new ValidateDeliveryNotesCommand(null!), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "No ids provided");
    }

    [Fact]
    public async Task Handle_WhenSomeDeliveryNotesMissing_ReturnsFailure()
    {
        using var context = CreateContext();
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new ValidateDeliveryNotesCommand(new List<int> { 1, 2 }), CancellationToken.None);

        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "delivery_notes_not_found: 2");
    }

    [Fact]
    public async Task Handle_WhenAllDraft_ValidatesAndReturnsSuccess()
    {
        using var context = CreateContext();
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(1));
        _ = context.BonDeLivraison.Add(CreateDeliveryNote(2));
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new ValidateDeliveryNotesCommand(new List<int> { 1, 2 }), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var notes = await context.BonDeLivraison.OrderBy(b => b.Num).ToListAsync();
        _ = notes.Should().OnlyContain(b => b.Statut == DocumentStatus.Valid);
    }

    [Fact]
    public async Task Handle_WhenAlreadyValid_SkipsAndReturnsSuccess()
    {
        using var context = CreateContext();
        var note = CreateDeliveryNote(1);
        note.Valider();
        _ = context.BonDeLivraison.Add(note);
        _ = context.SaveChanges();

        var handler = CreateHandler(context);
        var result = await handler.Handle(
            new ValidateDeliveryNotesCommand(new List<int> { 1 }), CancellationToken.None);

        _ = result.IsSuccess.Should().BeTrue();
        var stored = await context.BonDeLivraison.FirstAsync(b => b.Num == 1);
        _ = stored.Statut.Should().Be(DocumentStatus.Valid);
    }
}
