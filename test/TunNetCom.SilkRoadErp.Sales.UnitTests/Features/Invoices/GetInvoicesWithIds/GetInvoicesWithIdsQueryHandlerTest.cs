using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithIds;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Invoices
{
    public class GetInvoicesWithIdsQueryHandlerTest
    {
        private readonly Mock<ILogger<GetInvoicesWithIdsQueryHandler>> _mockLogger;
        public GetInvoicesWithIdsQueryHandlerTest()
        {
            _mockLogger = new Mock<ILogger<GetInvoicesWithIdsQueryHandler>>();
        }
        private SalesContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
            return new SalesContext(options);
        }

        [Fact]
        public async Task Handle_ShouldReturnInvoices_WhenIdsExist()
        {
            // Arrange
            using var context = CreateContext();
            var invoice = new Facture
            {
                Num = 1,
                IdClient = 101,
                BonDeLivraison = new List<BonDeLivraison>
                {
                    new BonDeLivraison { TotHTva = 100, TotTva = 19, NetPayer = 119 },
                    new BonDeLivraison { TotHTva = 50, TotTva = 9.5m, NetPayer = 59.5m }
                }
            };
            context.Facture.Add(invoice);
            await context.SaveChangesAsync();
            var handler = new GetInvoicesWithIdsQueryHandler(context, _mockLogger.Object);
            var query = new GetInvoicesWithIdsQuery(new List<int> { 1 });
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            var response = result.Value.First();
            response.Number.Should().Be(1);
            response.CustomerId.Should().Be(101);
            response.TotalExcludingTaxAmount.Should().Be(150);
            response.TotalVATAmount.Should().Be(28.5m);
            response.TotalIncludingTaxAmount.Should().Be(178.5m);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenIdsDoNotMatch()
        {
            // Arrange
            using var context = CreateContext();
            var handler = new GetInvoicesWithIdsQueryHandler(context, _mockLogger.Object);
            var query = new GetInvoicesWithIdsQuery(new List<int> { 99 });
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}
