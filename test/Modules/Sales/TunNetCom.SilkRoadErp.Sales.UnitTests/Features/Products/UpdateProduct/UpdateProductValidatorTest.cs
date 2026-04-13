//public class UpdateProductValidatorTest
//{
//    private readonly UpdateProductValidator _validator;
//    public UpdateProductValidatorTest()
//    {
//        _validator = new UpdateProductValidator();
//    }
//    [Fact]
//    public void Should_Have_Error_When_Refe_Is_Null_Or_Empty()
//    {
//        var model = new UpdateProductCommand(
//            Refe: null, Nom: "Nom", Qte: 1, QteLimite: 1,
//            Remise: 0, RemiseAchat: 0, Tva: 0,
//            Prix: 1, PrixAchat: 1, Visibilite: true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Refe)
//              .WithErrorMessage("reference_required");
//        model = model with { Refe = "" };
//        result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Refe);
//    }

//    [Fact]
//    public void Should_Have_Error_When_Nom_Is_Null_Or_Empty()
//    {
//        var model = new UpdateProductCommand(
//            Refe: "REF123", Nom: null, Qte: 1, QteLimite: 1,
//            Remise: 0, RemiseAchat: 0, Tva: 0,
//            Prix: 1, PrixAchat: 1, Visibilite: true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Nom)
//              .WithErrorMessage("nom_required");
//        model = model with { Nom = "" };
//        result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Nom);
//    }

//    [Fact]
//    public void Should_Have_Error_When_Qte_Is_Negative()
//    {
//        var model = new UpdateProductCommand(
//            Refe: "REF123", Nom: "Nom", Qte: -1, QteLimite: 1,
//            Remise: 0, RemiseAchat: 0, Tva: 0,
//            Prix: 1, PrixAchat: 1, Visibilite: true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Qte)
//              .WithErrorMessage("quantite_must_be_non_negative");
//    }

//    [Fact]
//    public void Should_Have_Error_When_Remise_Out_Of_Range()
//    {
//        var model = new UpdateProductCommand(
//            Refe: "REF123", Nom: "Nom", Qte: 1, QteLimite: 1,
//            Remise: -1, RemiseAchat: 0, Tva: 0,
//            Prix: 1, PrixAchat: 1, Visibilite: true);
//        var result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Remise)
//              .WithErrorMessage("remise_must_be_between_0_and_100");
//        model = model with { Remise = 101 };
//        result = _validator.TestValidate(model);
//        _ = result.ShouldHaveValidationErrorFor(x => x.Remise);
//    }

//    [Fact]
//    public void Should_Not_Have_Errors_When_Model_Is_Valid()
//    {
//        var model = new UpdateProductCommand(
//            Refe: "REF123",
//            Nom: "Nom valide",
//            Qte: 10,
//            QteLimite: 5,
//            Remise: 10,
//            RemiseAchat: 5,
//            Tva: 19,
//            Prix: 100m,
//            PrixAchat: 80m,
//            Visibilite: true);
//        var result = _validator.TestValidate(model);
//        result.ShouldNotHaveAnyValidationErrors();
//    }
//}
