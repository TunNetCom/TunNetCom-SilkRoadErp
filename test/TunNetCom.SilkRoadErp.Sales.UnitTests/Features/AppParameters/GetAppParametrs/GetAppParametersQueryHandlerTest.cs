using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
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
                Adresse = "123 Street",
                Tel = "12345678",
                Fax = "98765432",
                Email = "contact@societe.tn",
                MatriculeFiscale = "MF123",
                CodeTva = "TVA20",
                CodeCategorie = "Cat1",
                EtbSecondaire = "EtbX",
                AdresseRetenu = "Adresse Retenu",
                DiscountPercentage = 10
            };
            _ = await context.Systeme.AddAsync(systeme);
            _ = await context.SaveChangesAsync();
            var loggerMock = new Mock<ILogger<CreateCustomerCommandHandler>>();
            var financialParametersServiceMock = new Mock<IAccountingYearFinancialParametersService>();
            financialParametersServiceMock
                .Setup(x => x.GetAllFinancialParametersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AccountingYearFinancialParameters
                {
                    Timbre = 0.6m,
                    PourcentageFodec = 1.5m,
                    PourcentageRetenu = 5.0,
                    VatRate0 = 0,
                    VatRate7 = 7,
                    VatRate13 = 13,
                    VatRate19 = 19,
                    VatAmount = 19,
                    SeuilRetenueSource = 1000,
                    DecimalPlaces = 3
                });
            var handler = new GetAppParametersQueryHandler(context, loggerMock.Object, financialParametersServiceMock.Object);
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
