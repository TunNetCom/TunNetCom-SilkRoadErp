using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.UpdateAppParameters;


public class UpdateAppParametersCommandHandlerTest
{
    [Fact]
    public async Task Handle_ShouldUpdateAppParameters_WhenSystemeExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SalesContext>()
            .UseInMemoryDatabase(databaseName: "UpdateAppParametersDb")
            .Options;
        var context = new SalesContext(options);
        var existingSysteme = new Systeme
        {
            NomSociete = "Société Initiale",
            Adresse = "Adresse Initiale",
            CodeTva = "TVA000",
            Tel = "00000000",
        };
        context.Systeme.Add(existingSysteme);
        await context.SaveChangesAsync();
        var loggerMock = new Mock<ILogger<UpdateAppParametersCommandHandler>>();
        var handler = new UpdateAppParametersCommandHandler(context, loggerMock.Object);
        var command = new UpdateAppParametersCommand(
          NomSociete: "Ma Société",
          Timbre: 0.6m,
          Adresse: "Rue Exemple",
          Tel: "12345678",
          Fax: "87654321",
          Email: "email@example.com",
          MatriculeFiscale: "MF123",
          CodeTva: "TVA456",
          CodeCategorie: "CAT789",
          EtbSecondaire: "true",
          PourcentageFodec: 1.5m,
          AdresseRetenu: "Autre Adresse",
          PourcentageRetenu: 3.25
        );
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        result.IsSuccess.Should().BeTrue();
        var updated = await context.Systeme.FirstOrDefaultAsync();
        updated.NomSociete.Should().Be(command.NomSociete);
        updated.Email.Should().Be(command.Email);
        updated.EtbSecondaire.Should().Be("true");
        updated.PourcentageRetenu.Should().Be(command.PourcentageRetenu);
    }
}
