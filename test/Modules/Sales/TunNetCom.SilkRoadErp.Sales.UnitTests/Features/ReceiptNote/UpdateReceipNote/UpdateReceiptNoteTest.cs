using TunNetCom.SilkRoadErp.Sales.UnitTests.Tests;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Features.ReceiptNote.UpdateReceiptNote;

public class UpdateReceiptNoteTest
{
    private readonly SalesContext _context;
    private readonly TestLogger<UpdateReceiptNoteCommandHandler> _TestLogger;
    private readonly UpdateReceiptNoteCommandHandler _handler;

    public UpdateReceiptNoteTest()
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new SalesContext(options);
        _TestLogger = new TestLogger<UpdateReceiptNoteCommandHandler>();
        _handler = new UpdateReceiptNoteCommandHandler(_context, _TestLogger);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenNotFound()
    {
        // Arrange
        var command = new UpdateReceiptNoteCommand(
            Num: 1222,
            NumBonFournisseur: 1288,
            DateLivraison: DateTime.UtcNow,
            IdFournisseur: 5,
            Date: DateTime.UtcNow,
            NumFactureFournisseur: 66
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(_TestLogger.Logs,
            log => log.Contains($"{nameof(BonDeReception)}"));
        Assert.Contains(_TestLogger.Logs,
           log => log.Contains($"{command.Num}"));
    }

    [Theory]
    [InlineData(101, 123)]
    [InlineData(102, 234)]
    public async Task Handle_ShouldUpdateEntity_WhenFound(int num,int numFournisseur)
    {
        // Arrange
        var entity = new BonDeReception
        {
            Num = num,
            NumBonFournisseur = 44,
            NumFactureFournisseur = 55,
            IdFournisseur = 1,
            DateLivraison = DateTime.Today.AddDays(-1),
            Date = DateTime.Today.AddDays(-1)
        };

        _ = _context.BonDeReception.Add(entity);
        _ = await _context.SaveChangesAsync();

        var command = new UpdateReceiptNoteCommand(
            Num: num,
            NumBonFournisseur: 55,
            DateLivraison: DateTime.Today,
            IdFournisseur: 5,
            Date: DateTime.Today,
            NumFactureFournisseur: numFournisseur
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var updated = await _context.BonDeReception.FindAsync(entity.Num);
        Assert.NotNull(updated);
        Assert.Equal(55, updated.NumBonFournisseur);
        Assert.Equal(numFournisseur, updated.NumFactureFournisseur);
        Assert.Equal(5, updated.IdFournisseur);
    }
}

