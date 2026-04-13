using Newtonsoft.Json;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNote;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DeliveryNotes.GetDeliveryNote
{
    public class GetDeliveryNoteEndpointTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        public GetDeliveryNoteEndpointTest()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetDeliveryNote_ShouldReturnOkResult_WithPaginationHeader()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var queryParams = new QueryStringParameters
            {
                PageNumber = 1,
                PageSize = 2,
                SearchKeyword = "test"
            };
            bool? isFactured = false;
            var cancellationToken = CancellationToken.None;
            var deliveryNotes = new List<DeliveryNoteResponse>
            {
                new() { DeliveryNoteNumber = 2 },
                new() { DeliveryNoteNumber = 3}
            };
            var pagedList = new PagedList<DeliveryNoteResponse>(deliveryNotes, 5, 1, 2);
            _ = _mediatorMock
                .Setup(m => m.Send(It.Is<GetDeliveryNoteQuery>(q =>
                    q.PageNumber == queryParams.PageNumber &&
                    q.PageSize == queryParams.PageSize &&
                    q.SearchKeyword == queryParams.SearchKeyword &&
                    q.IsFactured == isFactured),
                    cancellationToken))
                .ReturnsAsync(pagedList);
            // Act
            var result = await GetDelegate()(queryParams, isFactured, _mediatorMock.Object, httpContext, cancellationToken);
            // Assert
            _ = result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<PagedList<DeliveryNoteResponse>>>();
            _ = httpContext.Response.Headers.Should().ContainKey("X-Pagination");
            var expectedMetadata = new
            {
                pagedList.TotalCount,
                pagedList.PageSize,
                pagedList.CurrentPage,
                pagedList.TotalPages,
                pagedList.HasNext,
                pagedList.HasPrevious
            };
            var expectedHeader = JsonConvert.SerializeObject(expectedMetadata);
            _ = httpContext.Response.Headers["X-Pagination"].ToString().Should().Be(expectedHeader);
        }
        private static Func<QueryStringParameters, bool?, IMediator, HttpContext, CancellationToken, Task<IResult>> GetDelegate()
        {
            return async (queryParams, isFactured, mediator, context, token) =>
            {
                var query = new GetDeliveryNoteQuery(
                    queryParams.PageNumber,
                    queryParams.PageSize,
                    queryParams.SearchKeyword,
                    isFactured);

                var pagedDeliveryNote = await mediator.Send(query, token);
                var metadata = new
                {
                    pagedDeliveryNote.TotalCount,
                    pagedDeliveryNote.PageSize,
                    pagedDeliveryNote.CurrentPage,
                    pagedDeliveryNote.TotalPages,
                    pagedDeliveryNote.HasNext,
                    pagedDeliveryNote.HasPrevious
                };
                context.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);
                return Results.Ok(pagedDeliveryNote);
            };
        }
    }
}
