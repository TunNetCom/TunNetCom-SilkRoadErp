using FluentAssertions;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Products.UpdateProduct;
using Xunit;
public class UpdateProductValidatorTests
{
    private readonly UpdateProductValidator _validator;
    public UpdateProductValidatorTests()
    {
        _validator = new UpdateProductValidator();
    }

    [Fact]
    public void Validator_Should_Pass_For_Valid_Command()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Refe: "Ref123",
            Nom: "ProduitTest",
            Qte: 10,
            QteLimite: 5,
            Remise: 10,
            RemiseAchat: 5,
            Tva: 19,
            Prix: 100,
            PrixAchat: 80,
            Visibilite: true);
        // Act
        var result = _validator.Validate(command);
        // Assert
        _ = result.IsValid.Should().BeTrue();
        _ = result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "reference_required")]      
    [InlineData(null, "reference_required")]        
    [InlineData("ThisReferenceIsWayTooLongAndExceedsFiftyCharactersLimitSoShouldFail", "reference_must_be_less_than_50_characters")] 
    public void Validator_Should_Fail_For_Invalid_Reference(string refe, string expectedErrorMessage)
    {
        // Arrange
        var command = new UpdateProductCommand(
            Refe: refe,
            Nom: "ValidName",
            Qte: 10,
            QteLimite: 5,
            Remise: 10,
            RemiseAchat: 5,
            Tva: 19,
            Prix: 100,
            PrixAchat: 80,
            Visibilite: true);
        // Act
        var result = _validator.Validate(command);
        // Assert
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().ContainSingle(e => e.ErrorMessage == expectedErrorMessage);
    }

    [Fact]
    public void Validator_Should_Fail_For_Negative_Quantities()
    {
        var command = new UpdateProductCommand(
            Refe: "Ref123",
            Nom: "ProduitTest",
            Qte: -1,
            QteLimite: -5,
            Remise: 10,
            RemiseAchat: 5,
            Tva: 19,
            Prix: 100,
            PrixAchat: 80,
            Visibilite: true);
        var result = _validator.Validate(command);
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == "quantite_must_be_non_negative");
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == "quantite_limit_must_be_non_negative");
    }
    [Theory]
    [InlineData(-1, "remise_must_be_between_0_and_100")]
    [InlineData(150, "remise_must_be_between_0_and_100")]
    public void Validator_Should_Fail_For_Invalid_Remise(double remise, string expectedErrorMessage)
    {
        var command = new UpdateProductCommand(
            Refe: "Ref123",
            Nom: "ProduitTest",
            Qte: 10,
            QteLimite: 5,
            Remise: remise,
            RemiseAchat: 5,
            Tva: 19,
            Prix: 100,
            PrixAchat: 80,
            Visibilite: true);
        var result = _validator.Validate(command);
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == expectedErrorMessage);
    }

    [Fact]
    public void Validator_Should_Fail_For_Prix_And_PrixAchat_Less_Than_Or_Equal_Zero()
    {
        var command = new UpdateProductCommand(
            Refe: "Ref123",
            Nom: "ProduitTest",
            Qte: 10,
            QteLimite: 5,
            Remise: 10,
            RemiseAchat: 5,
            Tva: 19,
            Prix: 0,
            PrixAchat: 0,
            Visibilite: true);
        var result = _validator.Validate(command);
        _ = result.IsValid.Should().BeFalse();
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == "prix_must_be_greater_than_0");
        _ = result.Errors.Should().Contain(e => e.ErrorMessage == "prix_achat_must_be_greater_than_0");
    }

    [Fact]
    public void Validator_Should_Fail_If_Visibilite_Is_Not_Specified()
    {
        var command = new UpdateProductCommand(
            Refe: "Ref123",
            Nom: "ProduitTest",
            Qte: 10,
            QteLimite: 5,
            Remise: 10,
            RemiseAchat: 5,
            Tva: 19,
            Prix: 100,
            PrixAchat: 80,
            Visibilite: false);
    }
}