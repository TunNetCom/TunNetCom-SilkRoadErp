using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByClientId;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using Xunit;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNotes
{
    public class GetDeliveryNotesByClientIdQueryHandlerTest
    {
        private Mock<ILogger<GetDeliveryNotesByClientIdQueryHandler>> _mockLogger;

        public GetDeliveryNotesByClientIdQueryHandlerTest()
        {
            _mockLogger = new Mock<ILogger<GetDeliveryNotesByClientIdQueryHandler>>();
        }

        private SalesContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SalesContext(options);
        }

        [Fact]
        public async Task Handle_ShouldReturnDeliveryNotes_WhenClientIdExists()
        {
            using var context = CreateContext();

            _ = context.BonDeLivraison.Add(new BonDeLivraison
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
            _ = await context.SaveChangesAsync();

            var handler = new GetDeliveryNotesByClientIdQueryHandler(context, _mockLogger.Object);
            var query = new GetDeliveryNoteByClientIdQuery(10);

            var result = await handler.Handle(query, CancellationToken.None);

            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().HaveCount(1);
            _ = result.Value.First().CustomerId.Should().Be(10);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNoDeliveryNotesFound()
        {
            using var context = CreateContext();

            var handler = new GetDeliveryNotesByClientIdQueryHandler(context, _mockLogger.Object);
            var query = new GetDeliveryNoteByClientIdQuery(999);

            var result = await handler.Handle(query, CancellationToken.None);

            _ = result.IsFailed.Should().BeTrue();
            _ = result.Errors.First().Message.Should().Be("not_found");
        }
    }
}