namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.ReceiptNotes
{
    public class GetReceiptNoteQueryHandlerTest
    {
        private readonly SalesContext _context;
        private readonly TestLogger<GetReceiptNoteQueryHandler> _testlogger;
        private readonly GetReceiptNoteQueryHandler _handler;
        public GetReceiptNoteQueryHandlerTest()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SalesContext(options);
            _testlogger = new TestLogger<GetReceiptNoteQueryHandler>();
            _handler = new GetReceiptNoteQueryHandler(_context, _testlogger);
        }

        [Fact]
        public async Task Handle_PaginationRequest_LogsPagination()
        {
            // Arrange
            var query = new GetReceiptNoteQuery(
                PageNumber: 1,
                PageSize: 10,
                SearchKeyword: null
            );
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.Contains(_testlogger.Logs,
                log => log.Contains($"Fetching {nameof(BonDeReception)} with pageIndex: {query.PageNumber} and pageSize: {query.PageSize}"));
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_EmptySearchKeyword_ReturnsAllReceiptNotes()
        {
            // Arrange
            var receiptnote1 = BonDeReception.CreateReceiptNote(
                num: 123456662,
                numBonFournisseur: 123456662,
                dateLivraison: new DateTime(2020, 1, 20),
                idFournisseur: 1,
                date: new DateTime(2020, 1, 20),
                numFactureFournisseur: 12345);
            var receiptnote2 = BonDeReception.CreateReceiptNote(
                num: 123456696,
                numBonFournisseur: 123456696,
                dateLivraison: new DateTime(2020, 1, 20),
                idFournisseur: 1,
                date: new DateTime(2020, 1, 20),
                numFactureFournisseur: 12345);
            _context.BonDeReception.Add(receiptnote1);
            _context.BonDeReception.Add(receiptnote2);
            await _context.SaveChangesAsync();
            var query = new GetReceiptNoteQuery(
               PageNumber: 1,
               PageSize: 10,
               SearchKeyword: ""
           );
           // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.True(result.Items.Count >= 2);
        }

        [Fact]
        public async Task Handle_SearchKeyword_FiltersResults()
        {
            // Arrange
            var receiptnote1 = BonDeReception.CreateReceiptNote(
                num: 111,
                numBonFournisseur: 222,
                dateLivraison: new DateTime(2023, 6, 1),
                idFournisseur: 5,
                date: new DateTime(2023, 6, 1),
                numFactureFournisseur: 333);
            var receiptnote2 = BonDeReception.CreateReceiptNote(
                num: 444,
                numBonFournisseur: 555,
                dateLivraison: new DateTime(2023, 7, 1),
                idFournisseur: 10,
                date: new DateTime(2023, 7, 1),
                numFactureFournisseur: 666);
            _context.BonDeReception.AddRange(receiptnote1, receiptnote2);
            await _context.SaveChangesAsync();

            var query = new GetReceiptNoteQuery(
                PageNumber: 1,
                PageSize: 10,
                SearchKeyword: "111" 
            );
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
           // Assert
            Assert.Single(result.Items);
            Assert.Equal(111, result.Items.First().Num);

        }
    }
}
