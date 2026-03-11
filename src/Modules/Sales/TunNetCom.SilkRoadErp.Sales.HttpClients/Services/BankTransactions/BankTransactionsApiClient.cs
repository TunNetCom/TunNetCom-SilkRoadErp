using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.BankTransactions;

public class BankTransactionsApiClient : IBankTransactionsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankTransactionsApiClient> _logger;

    public BankTransactionsApiClient(HttpClient httpClient, ILogger<BankTransactionsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<BankTransactionImportResponse>> GetImportsAsync(int? compteBancaireId, CancellationToken cancellationToken = default)
    {
        var url = "/api/bank-transactions/imports";
        if (compteBancaireId.HasValue)
        {
            url += $"?compteBancaireId={compteBancaireId.Value}";
        }
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<BankTransactionImportResponse>>(cancellationToken);
        return list ?? new List<BankTransactionImportResponse>();
    }

    public async Task<OneOf<ImportBankTransactionsResponse, BadRequestResponse>> ImportAsync(Stream fileStream, string fileName, int compteBancaireId, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(fileStream), "file", fileName);
        var response = await _httpClient.PostAsync($"/api/bank-transactions/import?compteBancaireId={compteBancaireId}", content, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ImportBankTransactionsResponse>(cancellationToken);
            return result ?? new ImportBankTransactionsResponse();
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        response.EnsureSuccessStatusCode();
        return new ImportBankTransactionsResponse();
    }

    public async Task<OneOf<byte[], bool>> ExportToSageAsync(int? importId, int? compteBancaireId, DateTime? dateDebut, DateTime? dateFin, CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (importId.HasValue) query.Add($"importId={importId.Value}");
        if (compteBancaireId.HasValue) query.Add($"compteBancaireId={compteBancaireId.Value}");
        if (dateDebut.HasValue) query.Add($"dateDebut={dateDebut.Value:yyyy-MM-dd}");
        if (dateFin.HasValue) query.Add($"dateFin={dateFin.Value:yyyy-MM-dd}");
        var url = "/api/bank-transactions/export/sage";
        if (query.Count > 0)
        {
            url += "?" + string.Join("&", query);
        }
        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return bytes;
    }
}
