using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class GetReceiptNoteEndpointTests
    {
        [Fact]
        public async Task GetReceiptNotes_ReturnsOk_WithPaginationHeader()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var queryParams = new QueryStringParameters
            {
                PageNumber = 1,
                PageSize = 10,
                SearchKeyword = "fournisseur"
            };
            var expectedResult = new PagedList<ReceiptNoteResponse>(
                items: new List<ReceiptNoteResponse>
                {
                    new ReceiptNoteResponse
                    {
                        Num = 1,
                        NumBonFournisseur = 123456789,
                        DateLivraison = DateTime.Today,
                        IdFournisseur = 101,
                        Date = DateTime.Today.AddDays(-1),
                        NumFactureFournisseur = null
                    }
                },
                count: 1,
                pageNumber: 1,
                pageSize: 10);
            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetReceiptNoteQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);
            var context = new DefaultHttpContext();
            var handler = async (
                QueryStringParameters paginationQueryParams,
                IMediator mediator,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var query = new GetReceiptNoteQuery(
                    paginationQueryParams.PageNumber,
                    paginationQueryParams.PageSize,
                    paginationQueryParams.SearchKeyword);
                var pagedReceipts = await mediator.Send(query, cancellationToken);
                var metadata = new
                {
                    pagedReceipts.TotalCount,
                    pagedReceipts.PageSize,
                    pagedReceipts.CurrentPage,
                    pagedReceipts.TotalPages,
                    pagedReceipts.HasNext,
                    pagedReceipts.HasPrevious
                };
                httpContext.Response.Headers["X-Pagination"] = JsonSerializer.Serialize(metadata);
                return Results.Ok(pagedReceipts);
            };

            // Act
            var result = await handler(queryParams, mediatorMock.Object, context, CancellationToken.None);
            // Assert
            var okResult = Assert.IsType<Ok<PagedList<ReceiptNoteResponse>>>(result);
            // Vérifie la liste retournée
            Assert.Single(okResult.Value.Items);
            Assert.Equal(1, okResult.Value.Items[0].Num);
            Assert.Equal(123456789, okResult.Value.Items[0].NumBonFournisseur);
            Assert.Equal(101, okResult.Value.Items[0].IdFournisseur);
            // Vérifie l'entête de pagination
            Assert.True(context.Response.Headers.ContainsKey("X-Pagination"));
            var paginationHeader = context.Response.Headers["X-Pagination"].ToString();
            Assert.Contains("\"TotalCount\":1", paginationHeader);
            Assert.Contains("\"PageSize\":10", paginationHeader);
            Assert.Contains("\"CurrentPage\":1", paginationHeader);
        }
    }
}
