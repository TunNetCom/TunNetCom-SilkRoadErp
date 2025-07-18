using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuote;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
using Xunit;

public class GetPriceQuoteEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    [Fact]
    public async Task GetPriceQuote_ReturnsOk_WithPagedListAndPaginationHeader()
    {
        // Arrange : paramètres de pagination et données factices
        var paginationParams = new QueryStringParameters
        {
            PageNumber = 1,
            PageSize = 2,
            SearchKeyword = null
        };

        var quotations = new List<QuotationResponse>
        {
            new QuotationResponse
            {
                Num = 1,
                IdClient = 10,
                Date = System.DateTime.UtcNow,
                TotHTva = 100m,
                TotTva = 20m,
                TotTtc = 120m
            },
            new QuotationResponse
            {
                Num = 2,
                IdClient = 20,
                Date = System.DateTime.UtcNow,
                TotHTva = 200m,
                TotTva = 40m,
                TotTtc = 240m
            }
        };

        var pagedList = new PagedList<QuotationResponse>(
            quotations,
            count: 5,
            pageNumber: 1,
            pageSize: 2);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetPriceQuoteQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedList);

        var endpoint = new GetPriceQuoteEndpoint();
        var httpContext = new DefaultHttpContext();

        // Act : on appelle la méthode d'invocation directe du endpoint
        var result = await endpoint.TestInvoke(
            paginationParams,
            _mediatorMock.Object,
            httpContext,
            CancellationToken.None);

        // Assert : on vérifie le type de retour
        result.Should().BeOfType<Ok<PagedList<QuotationResponse>>>();

        var okResult = result as Ok<PagedList<QuotationResponse>>;
        okResult!.Value.Should().BeEquivalentTo(pagedList);

        // Vérification de l'en-tête X-Pagination
        httpContext.Response.Headers.Should().ContainKey("X-Pagination");

        var paginationHeader = httpContext.Response.Headers["X-Pagination"].ToString();
        paginationHeader.Should().NotBeNullOrEmpty();

        var metadata = JsonConvert.DeserializeObject<dynamic>(paginationHeader);

        ((int)metadata.TotalCount).Should().Be(pagedList.TotalCount);
        ((int)metadata.PageSize).Should().Be(pagedList.PageSize);
        ((int)metadata.CurrentPage).Should().Be(pagedList.CurrentPage);
        ((int)metadata.TotalPages).Should().Be(pagedList.TotalPages);
        ((bool)metadata.HasNext).Should().Be(pagedList.HasNext);
        ((bool)metadata.HasPrevious).Should().Be(pagedList.HasPrevious);
    }
}

// Extension pour appeler la logique du endpoint sans route HTTP
public static class GetPriceQuoteEndpointExtensions
{
    public static Task<IResult> TestInvoke(
        this GetPriceQuoteEndpoint endpoint,
        QueryStringParameters paginationQueryParams,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        return endpoint.InvokeInternal(
            paginationQueryParams,
            mediator,
            httpContext,
            cancellationToken);
    }

    // Cette méthode doit être dans la classe GetPriceQuoteEndpoint, rendue 'internal' ou 'public' pour le test
    private static async Task<IResult> InvokeInternal(
        this GetPriceQuoteEndpoint endpoint,
        QueryStringParameters paginationQueryParams,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = new GetPriceQuoteQuery(
            paginationQueryParams.PageNumber,
            paginationQueryParams.PageSize,
            paginationQueryParams.SearchKeyword);

        var pagedCustomers = await mediator.Send(query, cancellationToken);

        var metadata = new
        {
            pagedCustomers.TotalCount,
            pagedCustomers.PageSize,
            pagedCustomers.CurrentPage,
            pagedCustomers.TotalPages,
            pagedCustomers.HasNext,
            pagedCustomers.HasPrevious
        };

        httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

        return Results.Ok(pagedCustomers);
    }
}
