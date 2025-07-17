using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProduct;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using Xunit;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Products
{
    public class GetProductEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetProductEndpoint _endpoint;

        public GetProductEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = new GetProductEndpoint();
        }

        [Fact]
        public async Task Endpoint_ReturnsPagedProducts_WithPaginationHeaders()
        {
            // Arrange
            var queryParams = new QueryStringParameters
            {
                PageNumber = 1,
                PageSize = 2,
                SearchKeyword = "Test",
                SortProprety = "Reference",
                SortOrder = "asc"
            };

            var products = new List<ProductResponse>
            {
                new ProductResponse
                {
                    Reference = "Ref1",
                    Name = "Product 1",
                    Qte = 10,
                    QteLimit = 5,
                    DiscountPourcentage = 0.1,
                    DiscountPourcentageOfPurchasing = 0.05,
                    VatRate = 0.2,
                    Price = 100m,
                    PurchasingPrice = 80m,
                    Visibility = true
                },
                new ProductResponse
                {
                    Reference = "Ref2",
                    Name = "Product 2",
                    Qte = 20,
                    QteLimit = 10,
                    DiscountPourcentage = 0.15,
                    DiscountPourcentageOfPurchasing = 0.1,
                    VatRate = 0.2,
                    Price = 200m,
                    PurchasingPrice = 150m,
                    Visibility = false
                }
            };

            var pagedList = new PagedList<ProductResponse>(
                items: products,
                count: 10,          // total count
                pageNumber: 1,
                pageSize: 2);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProductQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedList);

            var httpContext = new DefaultHttpContext();

            // Act
            var result = await _endpoint.Handle(queryParams, _mediatorMock.Object, httpContext, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<Ok<PagedList<ProductResponse>>>(result);
            okResult.Value.Should().BeEquivalentTo(pagedList);

            httpContext.Response.Headers.Should().ContainKey("X-Pagination");
            var paginationHeader = httpContext.Response.Headers["X-Pagination"];
            var metadata = JsonConvert.DeserializeObject<dynamic>(paginationHeader);

            Assert.Equal(pagedList.TotalCount, (int)metadata.TotalCount);
            Assert.Equal(pagedList.PageSize, (int)metadata.PageSize);
            Assert.Equal(pagedList.CurrentPage, (int)metadata.CurrentPage);
            Assert.Equal(pagedList.TotalPages, (int)metadata.TotalPages);
            Assert.Equal(pagedList.HasNext, (bool)metadata.HasNext);
            Assert.Equal(pagedList.HasPrevious, (bool)metadata.HasPrevious);
        }
    }
}
