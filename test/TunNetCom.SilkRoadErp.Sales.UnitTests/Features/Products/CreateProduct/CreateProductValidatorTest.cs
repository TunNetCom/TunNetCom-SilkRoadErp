//public class CreateProductValidatorTest
//{
//    private readonly CreateProductValidator _validator = new();

//    [Fact]
//    public void Should_Have_Error_When_Refe_Is_Null_Or_Empty()
//    {
//        var model = new CreateProductCommand(null, "Nom", 1, 1, 10, 10, 19, 100, 80, true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Refe)
//            .WithErrorMessage("reference_required");
//    }
//    [Fact]
//    public void Should_Have_Error_When_Nom_Is_Null_Or_Empty()
//    {
//        var model = new CreateProductCommand("REF001", "", 1, 1, 10, 10, 19, 100, 80, true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
//            .WithErrorMessage("nom_required");
//    }
//    [Fact]
//    public void Should_Have_Error_When_Qte_Is_Negative()
//    {
//        var model = new CreateProductCommand("REF", "Nom", -1, 0, 0, 0, 0, 1, 1, true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Qte)
//            .WithErrorMessage("quantite_must_be_non_negative");
//    }
//    [Fact]
//    public void Should_Have_Error_When_Remise_Outside_ValidRange()
//    {
//        var model = new CreateProductCommand("REF", "Nom", 1, 1, 200, 0, 0, 1, 1, true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Remise)
//            .WithErrorMessage("remise_must_be_between_0_and_100");
//    }
//    [Fact]
//    public void Should_Have_Error_When_Prix_Is_Zero()
//    {
//        var model = new CreateProductCommand("REF", "Nom", 1, 1, 10, 10, 20, 0, 100, true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Prix)
//            .WithErrorMessage("prix_must_be_greater_than_0");
//    }
//    [Fact]
//    public void Should_Not_Have_Error_For_Valid_Input()
//    {
//        var model = new CreateProductCommand("REF01", "Produit Test", 10, 2, 10, 5, 19, 150, 100, true);
//        var result = _validator.TestValidate(model);
//        result.ShouldNotHaveAnyValidationErrors();
//    }
//}
