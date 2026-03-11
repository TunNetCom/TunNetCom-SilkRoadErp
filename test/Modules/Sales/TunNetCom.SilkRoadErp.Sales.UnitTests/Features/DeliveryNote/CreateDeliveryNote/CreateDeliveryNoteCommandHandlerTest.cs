//using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;
//public class CreateDeliveryNoteCommandHandlerTest
//{
//    private readonly DbContextOptions<SalesContext> _dbOptions;
//    public CreateDeliveryNoteCommandHandlerTest()
//    {
//        _dbOptions = new DbContextOptionsBuilder<SalesContext>()
//            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//            .Options;
//    }
//    [Fact]
//    public async Task Handle_ShouldCreateDeliveryNoteAndReturnNum()
//    {
//        // Arrange
//        using var context = new SalesContext(_dbOptions);
//        var loggerMock = new Mock<ILogger<CreateDeliveryNoteCommandHandler>>();
//        var handler = new CreateDeliveryNoteCommandHandler(context, loggerMock.Object);
//        var command = new CreateDeliveryNoteCommand(
//            Date: DateTime.Today,
//            TotHTva: 100,
//            TotTva: 20,
//            NetPayer: 120,
//            TempBl: TimeOnly.FromDateTime(DateTime.Now),
//            NumFacture: null,
//            ClientId: 1,
//            DeliveryNoteDetails: new List<LigneBlSubCommand>
//            {
//                new() {
//                    RefProduit = "P001",
//                    DesignationLi = "Produit 1",
//                    QteLi = 2,
//                    PrixHt = 50,
//                    Remise = 0,
//                    TotHt = 100,
//                    Tva = 20,
//                    TotTtc = 120
//                }
//            });
//        // Act
//        var result = await handler.Handle(command, CancellationToken.None);
//        // Assert
//        _ = result.IsSuccess.Should().BeTrue();
//        _ = result.Value.Should().BeGreaterThan(0);
//        var saved = await context.BonDeLivraison.Include(b => b.LigneBl).FirstOrDefaultAsync();
//        _ = saved.Should().NotBeNull();
//        _ = saved!.LigneBl.Should().HaveCount(1);
        
//    }
//}
