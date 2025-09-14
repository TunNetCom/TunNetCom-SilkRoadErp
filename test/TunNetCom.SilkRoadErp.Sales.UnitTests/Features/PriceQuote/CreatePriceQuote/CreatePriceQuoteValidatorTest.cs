using TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;
public class CreatePriceQuoteValidatorTest
{
    private readonly CreatePriceQuoteValidator _validator;
    public CreatePriceQuoteValidatorTest()
    {
        _validator = new CreatePriceQuoteValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenValidCommand()
    {
        // Arrange
        var command = new CreatePriceQuoteCommand(
            Num: 123,
            IdClient: 10,
            Date: DateTime.UtcNow,
            TotHTva: 100m,
            TotTva: 19m,
            TotTtc: 119m
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        _ = result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 10, "num_is_required")]
    [InlineData(123, 0, "client_id_is_required")]
    public void Validate_ShouldFail_WhenRequiredFieldsMissing(int num, int idClient, string expectedErrorMessage)
    {
        // Arrange
        var command = new CreatePriceQuoteCommand(
            Num: num,
            IdClient: idClient,
            Date: DateTime.UtcNow,
            TotHTva: 100m,
            TotTva: 19m,
            TotTtc: 119m
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().ContainSingle(e => e.ErrorMessage == expectedErrorMessage);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDateIsDefault()
    {
        // Arrange
        var command = new CreatePriceQuoteCommand(
            Num: 123,
            IdClient: 10,
            Date: default,
            TotHTva: 100m,
            TotTva: 19m,
            TotTtc: 119m
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().ContainSingle(e => e.ErrorMessage == "invalid_date_format");
    }

    [Theory]
    [InlineData(-1, "total_ht_tva_must_be_greater_or_equal_to_zero")]
    [InlineData(0, null)] 
    public void Validate_ShouldCheck_TotHTva(decimal totHTva, string? expectedErrorMessage)
    {
        // Arrange
        var command = new CreatePriceQuoteCommand(
            Num: 123,
            IdClient: 10,
            Date: DateTime.UtcNow,
            TotHTva: totHTva,
            TotTva: 19m,
            TotTtc: 119m
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        if (expectedErrorMessage == null)
            _ = result.IsValid.Should().BeTrue();
        else
            _ = result.Errors.Should().ContainSingle(e => e.ErrorMessage == expectedErrorMessage);
    }

    [Theory]
    [InlineData(-1, "total_tva_must_be_greater_or_equal_to_zero")]
    [InlineData(0, null)]
    public void Validate_ShouldCheck_TotTva(decimal totTva, string? expectedErrorMessage)
    {
        // Arrange
        var command = new CreatePriceQuoteCommand(
            Num: 123,
            IdClient: 10,
            Date: DateTime.UtcNow,
            TotHTva: 100m,
            TotTva: totTva,
            TotTtc: 119m
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        if (expectedErrorMessage == null)
            _ = result.IsValid.Should().BeTrue();
        else
            _ = result.Errors.Should().ContainSingle(e => e.ErrorMessage == expectedErrorMessage);
    }

    [Theory]
    [InlineData(-1, "total_ttc_must_be_greater_or_equal_to_zero")]
    [InlineData(0, null)]
    public void Validate_ShouldCheck_TotTtc(decimal totTtc, string? expectedErrorMessage)
    {
        // Arrange
        var command = new CreatePriceQuoteCommand(
            Num: 123,
            IdClient: 10,
            Date: DateTime.UtcNow,
            TotHTva: 100m,
            TotTva: 19m,
            TotTtc: totTtc
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        if (expectedErrorMessage == null)
            _ = result.IsValid.Should().BeTrue();
        else
            _ = result.Errors.Should().ContainSingle(e => e.ErrorMessage == expectedErrorMessage);
    }
}
