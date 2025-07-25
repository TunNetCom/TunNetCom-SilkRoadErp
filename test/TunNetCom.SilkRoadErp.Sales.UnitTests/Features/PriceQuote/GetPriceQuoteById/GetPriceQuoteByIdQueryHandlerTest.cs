﻿using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Handlers.PriceQuote
{
    public class GetPriceQuoteByIdQueryHandlerTest
    {
        private readonly DbContextOptions<SalesContext> _dbOptions;
        public GetPriceQuoteByIdQueryHandlerTest()
        {
            _dbOptions = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "SalesTestDb_GetById")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnQuotation_WhenQuotationExists()
        {
            // Arrange
            var num = 1;
            using (var context = new SalesContext(_dbOptions))
            {
                context.Devis.Add(new Devis
                {
                    Num = num,
                    IdClient = 101,
                    Date = DateTime.Today,
                    TotHTva = 100,
                    TotTva = 19,
                    TotTtc = 119
                });
                context.SaveChanges();
            }
            using (var context = new SalesContext(_dbOptions))
            {
                var loggerMock = new Mock<ILogger<GetPriceQuoteByIdQueryHandler>>();
                var handler = new GetPriceQuoteByIdQueryHandler(context, loggerMock.Object);
                var query = new GetPriceQuoteByIdQuery(num);
                // Act
                var result = await handler.Handle(query, CancellationToken.None);
                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Num.Should().Be(num);
                result.Value.IdClient.Should().Be(101);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenQuotationNotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<GetPriceQuoteByIdQueryHandler>>();
            using var context = new SalesContext(_dbOptions);
            var handler = new GetPriceQuoteByIdQueryHandler(context, logger.Object);
            var query = new GetPriceQuoteByIdQuery(999); // Num inexistant
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Message.Should().Be("not_found"); 
        }
    }
}
