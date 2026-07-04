using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.FunctionalTests.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.FunctionalTests.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteEndpointFunctionalTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CreateDeliveryNoteEndpointFunctionalTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostDeliveryNote_WithValidRequest_ReturnsCreated()
    {
        var request = new CreateDeliveryNoteRequest
        {
            Date = new DateTime(2024, 6, 15, 10, 0, 0),
            TotalExcludingTax = 100m,
            TotalVat = 19m,
            TotalAmount = 119m,
            DeliveryTime = new TimeOnly(14, 30),
            Items = new List<DeliveryNoteItemRequest>
            {
                new()
                {
                    ProductReference = "REF1",
                    Description = "Product 1",
                    Quantity = 2,
                    DeliveredQuantity = 2,
                    UnitPriceExcludingTax = 50m,
                    DiscountPercentage = 0,
                    TotalExcludingTax = 100m,
                    VatPercentage = 19,
                    TotalIncludingTax = 119m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/deliveryNote", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Matches("/deliveryNote/\\d+", response.Headers.Location.ToString());

        var body = await response.Content.ReadFromJsonAsync<CreateDeliveryNoteRequest>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task PostDeliveryNote_WithDefaultDate_ReturnsValidationProblem()
    {
        var request = new CreateDeliveryNoteRequest
        {
            Date = default,
            TotalExcludingTax = 100m,
            TotalVat = 19m,
            TotalAmount = 119m,
            DeliveryTime = new TimeOnly(14, 30),
            Items = new List<DeliveryNoteItemRequest>()
        };

        var response = await _client.PostAsJsonAsync("/deliveryNote", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var errors = doc.RootElement.GetProperty("errors").GetProperty("errors");
        var errorMessages = errors.EnumerateArray().Select(e => e.GetString()).ToList();
        Assert.Contains("date_is_required", errorMessages);
    }

    [Fact]
    public async Task PostDeliveryNote_WithNegativeTotalExcludingTax_ReturnsValidationProblem()
    {
        var request = new CreateDeliveryNoteRequest
        {
            Date = DateTime.Today,
            TotalExcludingTax = -1m,
            TotalVat = 19m,
            TotalAmount = 18m,
            DeliveryTime = new TimeOnly(14, 30),
            Items = new List<DeliveryNoteItemRequest>()
        };

        var response = await _client.PostAsJsonAsync("/deliveryNote", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var errors = doc.RootElement.GetProperty("errors").GetProperty("errors");
        var errorMessages = errors.EnumerateArray().Select(e => e.GetString()).ToList();
        Assert.Contains("tothtva_must_be_greater_than_or_equal_to_0", errorMessages);
    }

    [Fact]
    public async Task DeliveryNote_Sum_ShouldBeCorrect()
    {
        var request = new CreateDeliveryNoteRequest
        {
            Date = new DateTime(2025, 6, 15, 10, 0, 0),
            TotalExcludingTax = 255m,
            TotalVat = 48.45m,
            TotalAmount = 303.45m,
            DeliveryTime = new TimeOnly(14, 30),
            Items = new List<DeliveryNoteItemRequest>
                    {
                        new()
                        {
                            ProductReference = "REF1",
                            Description = "Product 1",
                            Quantity = 2,
                            DeliveredQuantity = 2,
                            UnitPriceExcludingTax = 50m,
                            DiscountPercentage = 0,
                            TotalExcludingTax = 100m,
                            VatPercentage = 19,
                            TotalIncludingTax = 119m
                        },
                        new()
                        {
                            ProductReference = "REF2",
                            Description = "Product 2",
                            Quantity = 3,
                            DeliveredQuantity = 3,
                            UnitPriceExcludingTax = 25m,
                            DiscountPercentage = 0,
                            TotalExcludingTax = 75m,
                            VatPercentage = 19,
                            TotalIncludingTax = 89.25m
                        },
                        new()
                        {
                            ProductReference = "REF3",
                            Description = "Product 3",
                            Quantity = 4,
                            DeliveredQuantity = 4,
                            UnitPriceExcludingTax = 20m,
                            DiscountPercentage = 0,
                            TotalExcludingTax = 80m,
                            VatPercentage = 19,
                            TotalIncludingTax = 95.20m
                        }
                    }
        };


        var postResponse = await _client.PostAsJsonAsync("/deliveryNote", request);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var location = postResponse.Headers.Location;
        Assert.NotNull(location);

        var getResponse = await _client.GetAsync(location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var deliveryNote = await getResponse.Content
            .ReadFromJsonAsync<DeliveryNoteResponse>();

        Assert.NotNull(deliveryNote);

        Assert.Equal(255m, deliveryNote.TotalExcludingTax);
        Assert.Equal(48.45m, deliveryNote.TotalVat);
        Assert.Equal(303.45m, deliveryNote.TotalAmount);

        Assert.Equal(3, deliveryNote.Items.Count);
        Assert.Equal(255m, deliveryNote.Items.Sum(i => i.TotalExcludingTax));
        Assert.Equal(303.45m, deliveryNote.Items.Sum(i => i.TotalIncludingTax));
    }
}
