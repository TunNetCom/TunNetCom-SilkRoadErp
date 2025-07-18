using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.AppParameters
{
    public class GetAppParametersQueryHandlerTest
    {
        [Fact]
        public async Task Handle_ShouldReturnAppParameters_WhenOneRowExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: "GetAppParamsTest")
                .Options;
            using var context = new SalesContext(options);
           var systeme = new Systeme
            {
                NomSociete = "MySociety",
                Timbre = 0.6m,
                Adresse = "123 Street",
                Tel = "12345678",
                Fax = "98765432",
                Email = "contact@societe.tn",
                MatriculeFiscale = "MF123",
                CodeTva = "TVA20",
                CodeCategorie = "Cat1",
                EtbSecondaire = "EtbX",
                PourcentageFodec = 1.5m,
                AdresseRetenu = "Adresse Retenu",
                PourcentageRetenu = 5.0,
                DiscountPercentage = 10,
                VatAmount = 19
            };
            await context.Systeme.AddAsync(systeme);
            await context.SaveChangesAsync();
            var loggerMock = new Mock<ILogger<CreateCustomerCommandHandler>>();
            var handler = new GetAppParametersQueryHandler(context, loggerMock.Object);
            var request = new GetAppParametersQuery();
            // Act
            var result = await handler.Handle(request, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(systeme.NomSociete, result.Value.NomSociete);
            Assert.Equal(systeme.Tel, result.Value.Tel);
            Assert.Equal(systeme.CodeTva, result.Value.CodeTva);
        }
    }
}
