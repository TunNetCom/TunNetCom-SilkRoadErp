using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;
using FluentValidation;
using FluentValidation.Results;
public class GlobalExceptionHandlerTest
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private readonly GlobalExceptionHandler _handler;
    public GlobalExceptionHandlerTest()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_loggerMock.Object);
    }
    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        context.Request.Path = "/test";
        return context;
    }
    private static string GetResponseBody(HttpContext context)
    {
        _ = context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return reader.ReadToEnd();
    }

    [Fact]
    public async Task HandleExceptionAsync_ShouldReturnValidationProblemDetails_ForValidationException()
    {
        // Arrange
        var context = CreateHttpContext();
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Email", "Email is invalid")
        };
        var validationException = new ValidationException(failures);
        // Act
        var result = await _handler.HandleExceptionAsync(context, validationException);
        // Assert
        _ = result.Should().BeTrue();
        _ = context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _ = context.Response.ContentType.Should().Be("application/json");
        var json = GetResponseBody(context);
        var problemDetails = JsonSerializer.Deserialize<GlobalExceptionHandler.ErrorsProblemDetails>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        _ = problemDetails.Should().NotBeNull();
        _ = problemDetails.Status.Should().Be((int)HttpStatusCode.BadRequest);
        _ = problemDetails.Title.Should().Be("Validation Error");
        _ = problemDetails.Detail.Should().Be("One or more validation errors occurred.");
        _ = problemDetails.Instance.Should().Be("/test");
        _ = problemDetails.errors.Should().ContainKey("errors");
        _ = problemDetails.errors["errors"].Should().BeEquivalentTo("Name is required", "Email is invalid");
    }

    [Fact]
    public async Task HandleExceptionAsync_ShouldReturnProblemDetails_ForGenericException()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new Exception("Something went wrong");
        // Act
        var result = await _handler.HandleExceptionAsync(context, exception);
        // Assert
        _ = result.Should().BeTrue();
        _ = context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _ = context.Response.ContentType.Should().Be("application/json");
        var json = GetResponseBody(context);
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        _ = problemDetails.Should().NotBeNull();
        _ = problemDetails.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        _ = problemDetails.Title.Should().Be("An error occurred while processing your request.");
        _ = problemDetails.Detail.Should().Be("Something went wrong");
        _ = problemDetails.Instance.Should().Be("/test");
    }
}
