using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class CreateReceiptNoteCommandHandlerTest
    {
        private SalesContext CreateTestContext()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SalesContext(options);
        }

        [Fact]
        public async Task Handle_ProviderNotFound_ReturnsFailResult()
        {
            // Arrange
            using var context = CreateTestContext();
            var logger = Mock.Of<ILogger<CreateReceiptNoteCommandHandler>>();
            var handler = new CreateReceiptNoteCommandHandler(context, logger);
            var command = new CreateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 123,
                DateLivraison: DateTime.Today,
                IdFournisseur: 999, // Not existing
                Date: DateTime.Today,
                NumFactureFournisseur: null
            );
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("not_found", result.Errors.First().Message);
        }
        [Fact]
        public async Task Handle_ReceiptNoteAlreadyExists_ReturnsFailResult()
        {
            // Arrange
            using var context = CreateTestContext();
            var logger = Mock.Of<ILogger<CreateReceiptNoteCommandHandler>>();
            var handler = new CreateReceiptNoteCommandHandler(context, logger);
            // Fournisseur avec tous les champs requis
            var fournisseur = new Fournisseur
            {
                Id = 1,
                Nom = "Test Fournisseur",
                Tel = "12345678",           // Champ requis
                Fax = "Fax Test",
                Matricule = "Mat123",
                Code = "Code001",
                CodeCat = "Cat01",
                EtbSec = "Sec01",
                Mail = "fournisseur@example.com"
            };
            context.Fournisseur.Add(fournisseur);
            var existingNote = BonDeReception.CreateReceiptNote(
                num: 1,
                numBonFournisseur: 123,
                dateLivraison: DateTime.Today,
                idFournisseur: 1,
                date: DateTime.Today,
                numFactureFournisseur: null
            );
            context.BonDeReception.Add(existingNote);
            await context.SaveChangesAsync();
            var command = new CreateReceiptNoteCommand(
                Num: 1,
                NumBonFournisseur: 123,
                DateLivraison: DateTime.Today,
                IdFournisseur: 1,
                Date: DateTime.Today,
                NumFactureFournisseur: null
            );
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
}
        [Fact]
        public async Task Handle_ValidReceiptNote_CreatesSuccessfully()
        {
            // Arrange
            using var context = CreateTestContext();
            var logger = Mock.Of<ILogger<CreateReceiptNoteCommandHandler>>();
            var handler = new CreateReceiptNoteCommandHandler(context, logger);
            var fournisseur = new Fournisseur
            {
                Id = 1,
                Nom = "Test Fournisseur",
                Tel = "123456789",
                Fax = "Test Fax",
                Matricule = "MF123456",
                Code = "F001",
                CodeCat = "CAT001",
                EtbSec = "E1",
                Mail = "test@provider.com"
            };
           context.Fournisseur.Add(fournisseur);
            await context.SaveChangesAsync();

            var command = new CreateReceiptNoteCommand(
                Num: 10,
                NumBonFournisseur: 456,
                DateLivraison: new DateTime(2024, 5, 10),
                IdFournisseur: 1,
                Date: new DateTime(2024, 5, 10),
                NumFactureFournisseur: null
            );
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10, result.Value);
        }

    }
}
