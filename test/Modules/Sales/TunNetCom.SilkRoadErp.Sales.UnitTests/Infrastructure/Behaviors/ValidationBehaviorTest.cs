using FluentValidation;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Behaviors;
public class ValidationBehaviorTests
{
    private class TestCommand : IRequest<string>
    {
        public string Name { get; set; }
    }
    private class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    [Fact]
    public async Task Handle_Should_ThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var validators = new List<IValidator<TestCommand>> { new TestCommandValidator() };
        var behavior = new ValidationBehavior<TestCommand, string>(validators);
        var request = new TestCommand { Name = "" }; // Invalid
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(request, nextMock.Object, CancellationToken.None));
        _ = exception.Errors.Should().Contain(e => e.ErrorMessage == "Name is required");
        nextMock.Verify(n => n(), Times.Never); // next should not be called
    }

    [Fact]
    public async Task Handle_Should_CallNext_WhenValidationPasses()
    {
        // Arrange
        var validators = new List<IValidator<TestCommand>> { new TestCommandValidator() };
        var behavior = new ValidationBehavior<TestCommand, string>(validators);
        var request = new TestCommand { Name = "Valid Name" };
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        _ = nextMock.Setup(n => n()).ReturnsAsync("Success");
        // Act
        var result = await behavior.Handle(request, nextMock.Object, CancellationToken.None);
        // Assert
        _ = result.Should().Be("Success");
        nextMock.Verify(n => n(), Times.Once);
    }
}
