//using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;
//using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;
//public class UpdateAppParametersEndpoinTest
//{
//    [Fact]
//    public async Task UpdateAppParameters_ShouldReturnNoContent_WhenUpdateSucceeds()
//    {
//        // Arrange: contexte EF InMemory avec un Systeme existant
//        var options = new DbContextOptionsBuilder<SalesContext>()
//            .UseInMemoryDatabase(databaseName: "UpdateAppParamsLogicDb")
//            .Options;
//        var context = new SalesContext(options);
//        _ = context.Systeme.Add(new Systeme
//        {
//            NomSociete = "Société Initiale",
//            Adresse = "Adresse Initiale",
//            CodeTva = "TVA000",
//            Tel = "00000000",
//        });
//        _ = await context.SaveChangesAsync();
//        var loggerMock = new Mock<ILogger<UpdateAppParametersCommandHandler>>();
//        var handler = new UpdateAppParametersCommandHandler(context, loggerMock.Object);
//        var mediatorMock = new Mock<IMediator>();
//        _ = mediatorMock.Setup(m => m.Send(It.IsAny<UpdateAppParametersCommand>(), It.IsAny<CancellationToken>()))
//                    .Returns<UpdateAppParametersCommand, CancellationToken>((cmd, token) => handler.Handle(cmd, token));
//        var request = new UpdateAppParametersRequest
//        {
//            NomSociete = "Société Modifiée",
//            Timbre = 1.0m,
//            Adresse = "Nouvelle Adresse",
//            Tel = "11111111",
//            Fax = "22222222",
//            Email = "modifie@example.com",
//            MatriculeFiscale = "MF999",
//            CodeTva = "TVA999",
//            CodeCategorie = "CAT999",
//            EtbSecondaire = "true",
//            PourcentageFodec = 2.5m,
//            AdresseRetenu = "Adresse Retenue Modifiée",
//            PourcentageRetenu = 5.5
//        };
//        var command = new UpdateAppParametersCommand(
//            request.NomSociete,
//            request.Timbre,
//            request.Adresse,
//            request.Tel,
//            request.Fax,
//            request.Email,
//            request.MatriculeFiscale,
//            request.CodeTva,
//            request.CodeCategorie,
//            request.EtbSecondaire,
//            request.PourcentageFodec,
//            request.AdresseRetenu,
//            request.PourcentageRetenu,
//            request.VatAmount,
//            request.DiscountPercentage
//        );
//        // Act
//        var result = await mediatorMock.Object.Send(command, CancellationToken.None);
//        // Assert
//        _ = result.IsSuccess.Should().BeTrue();
//        var updated = await context.Systeme.FirstOrDefaultAsync();
//        _ = updated.NomSociete.Should().Be(request.NomSociete);
//        _ = updated.Email.Should().Be(request.Email);
//        _ = updated.EtbSecondaire.Should().Be(request.EtbSecondaire);
//        _ = updated.PourcentageRetenu.Should().Be(request.PourcentageRetenu);
//    }
//}
