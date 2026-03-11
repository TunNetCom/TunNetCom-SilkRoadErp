using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.Infrastructure
{
    public class ResultExtensionTests
    {
        [Fact]
        public void ToValidationProblem_ShouldThrow_WhenResultIsSuccess()
        {
            var successResult = Result.Ok();
            Action act1 = () => successResult.ToValidationProblem();
            _ = act1.Should().Throw<InvalidOperationException>();
            var successResultInt = Result.Ok(5);
            Action act2 = () => successResultInt.ToValidationProblem();
            _ = act2.Should().Throw<InvalidOperationException>();
            var successResultString = Result.Ok("ok");
            Action act3 = () => successResultString.ToValidationProblem();
            _ = act3.Should().Throw<InvalidOperationException>();
        }
        [Fact]
        public void ToValidationProblem_ShouldReturnValidationProblemResult_WhenResultIsFailure()
        {
            var errorMessages = new[] { "error1", "error2" };
            var failureResult = Result.Fail(errorMessages);
            var validationProblemResult = failureResult.ToValidationProblem();
            _ = validationProblemResult.Should().BeOfType<ValidationProblem>();
        }
        [Fact]
        public void IsEntityNotFound_ShouldReturnFalse_WhenEntityNotFoundErrorNotPresent()
        {
            var failureResult = Result.Fail("some_other_error");
            _ = failureResult.IsEntityNotFound().Should().BeFalse();
            var failureResultInt = Result.Fail<int>("some_other_error");
            _ = failureResultInt.IsEntityNotFound().Should().BeFalse();
            var successResult = Result.Ok();
            _ = successResult.IsEntityNotFound().Should().BeFalse();
        }
    }
    public class EntityNotFound { }
}
