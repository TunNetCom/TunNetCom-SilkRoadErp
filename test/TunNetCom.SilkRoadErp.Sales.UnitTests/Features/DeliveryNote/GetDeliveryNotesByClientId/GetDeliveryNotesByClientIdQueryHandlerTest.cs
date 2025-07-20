using Mapster;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByClientId;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNotes
{
    public class GetDeliveryNotesByClientIdQueryHandlerTest
    {
        private readonly DbContextOptions<SalesContext> _dbContextOptions;
        private readonly Mock<ILogger<GetDeliveryNotesByClientIdQueryHandler>> _mockLogger;
        public GetDeliveryNotesByClientIdQueryHandlerTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockLogger = new Mock<ILogger<GetDeliveryNotesByClientIdQueryHandler>>();         
            TypeAdapterConfig<BonDeLivraison, DeliveryNoteResponse>.NewConfig()
                .Map(dest => dest.CustomerId, src => src.ClientId)
                .Map(dest => dest.DeliveryNoteNumber, src => src.Num)
                .Map(dest => dest.CreationTime, src => src.TempBl)
                .Map(dest => dest.InvoiceNumber, src => src.NumFacture)
                .Map(dest => dest.TotalExcludingTax, src => src.TotHTva)
                .Map(dest => dest.TotalVat, src => src.TotTva)
                .Map(dest => dest.TotalAmount, src => src.NetPayer);
        }

        [Fact]
        public async Task Handle_ShouldReturnDeliveryNotes_WhenClientIdExists()
        {
            // Arrange
            using var context = new SalesContext(_dbContextOptions);
            context.BonDeLivraison.Add(new BonDeLivraison
            {
                Num = 1,
                Date = DateTime.Today,
                ClientId = 10,
                TotHTva = 100,
                TotTva = 19,
                NetPayer = 119,
                TempBl = new TimeOnly(10, 0),
                NumFacture = 5
            });
            await context.SaveChangesAsync();
            var handler = new GetDeliveryNotesByClientIdQueryHandler(context, _mockLogger.Object);
            var query = new GetDeliveryNoteByClientIdQuery(10);
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value.First().CustomerId.Should().Be(10);
        }
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNoDeliveryNotesFound()
        {
            // Arrange
            using var context = new SalesContext(_dbContextOptions);
            var handler = new GetDeliveryNotesByClientIdQueryHandler(context, _mockLogger.Object);
            var query = new GetDeliveryNoteByClientIdQuery(999); // ClientId non existant
           // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.First().Message.Should().Be("not_found");
        }
    }
}
