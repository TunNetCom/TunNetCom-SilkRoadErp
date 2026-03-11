using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;
public class GetDeliveryNoteByNumQueryHandlerTest
{
    private readonly Mock<ILogger<GetDeliveryNoteByNumQueryHandler>> _loggerMock = new();
    private readonly CancellationToken _defaultCancellationToken = CancellationToken.None;

    private SalesContext CreateContextWithData(IEnumerable<BonDeLivraison> data)
    {
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new SalesContext(options);
        context.BonDeLivraison.AddRange(data);
        _ = context.SaveChanges();
        return context;
    }
    public GetDeliveryNoteByNumQueryHandlerTest()
    {
        _ = _loggerMock.Setup(l => l.IsEnabled(LogLevel.Information)).Returns(true);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDeliveryNoteExists()
    {
        // Arrange
        int deliveryNoteNum = 1;
        var bonLivraison = new BonDeLivraison
        {
            Num = deliveryNoteNum,
            Date = DateTime.Today,
            ClientId = 10,
            TotHTva = 100,
            TotTva = 19,
            NetPayer = 119,
            NumFacture = 5,
            TempBl = new TimeOnly(14, 30),
            LigneBl = new List<LigneBl>
            {
                new() {
                    RefProduit = "REF001",
                    DesignationLi = "Produit Test",
                    QteLi = 2,
                    PrixHt = 50,
                    Remise = 0,
                    Tva = 19,
                    TotHt = 100,
                    TotTtc = 119
                }
            }
        };
        using var context = CreateContextWithData(new[] { bonLivraison });
        var handler = new GetDeliveryNoteByNumQueryHandler(context, _loggerMock.Object);
        var query = new GetDeliveryNoteByNumQuery(deliveryNoteNum);
        // Act
         var result = await handler.Handle(query, _defaultCancellationToken);
        // Assert
        _ = result.IsSuccess.Should().BeTrue();
        var value = result.Value;
        _ = value.DeliveryNoteNumber.Should().Be(deliveryNoteNum);
        _ = value.Items.Should().HaveCount(1);      
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("BonDeLivraison")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenDeliveryNoteNotFound()
    {
        // Arrange
        using var context = CreateContextWithData(Array.Empty<BonDeLivraison>());
        var handler = new GetDeliveryNoteByNumQueryHandler(context, _loggerMock.Object);
        var query = new GetDeliveryNoteByNumQuery(999);
        // Act
        var result = await handler.Handle(query, _defaultCancellationToken);
        // Assert
        _ = result.IsFailed.Should().BeTrue();
        _ = result.Errors.Should().ContainSingle(e => e.Message == "not_found");     
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("BonDeLivraison")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenCancelled()
    {
        // Arrange
        using var context = CreateContextWithData(Array.Empty<BonDeLivraison>());
        var handler = new GetDeliveryNoteByNumQueryHandler(context, _loggerMock.Object);
        var query = new GetDeliveryNoteByNumQuery(1);
        var cts = new CancellationTokenSource();
        cts.Cancel();
        // Act & Assert
        _ = await Assert.ThrowsAsync<OperationCanceledException>(() => handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        var mockContext = new Mock<SalesContext>(new DbContextOptions<SalesContext>());
        _ = mockContext.Setup(c => c.BonDeLivraison)
            .Throws(new Exception("Unexpected failure"));
        var handler = new GetDeliveryNoteByNumQueryHandler(mockContext.Object, _loggerMock.Object);
        var query = new GetDeliveryNoteByNumQuery(1);
        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, _defaultCancellationToken));
        _ = ex.Message.Should().Be("Unexpected failure");
    }
}
