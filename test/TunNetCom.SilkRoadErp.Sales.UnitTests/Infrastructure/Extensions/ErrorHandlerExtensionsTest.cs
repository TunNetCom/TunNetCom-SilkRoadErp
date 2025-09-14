using System.Net;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;


public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task HandleExceptionAsync_Should_Return_InternalServerError_For_GenericException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        var handler = new GlobalExceptionHandler(loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/test";

        var exception = new InvalidOperationException("Test exception");

        // Act
        var result = await handler.HandleExceptionAsync(context, exception);

        // Assert
        _ = result.Should().BeTrue();
        _ = context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        _ = context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        _ = body.Should().Contain("Test exception");
    }
}
