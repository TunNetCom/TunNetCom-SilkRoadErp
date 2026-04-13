using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Recap;

public class RecapVentesAchatsService : IRecapVentesAchatsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecapVentesAchatsService> _logger;

    public RecapVentesAchatsService(
        HttpClient httpClient,
        ILogger<RecapVentesAchatsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RecapVentesAchatsViewModel> GetRecapAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var vm = new RecapVentesAchatsViewModel
        {
            StartDate = startDate,
            EndDate = endDate
        };

        try
        {
            var url = $"/api/dashboard/recap-ventes-achats?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                vm.HasError = true;
                vm.ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                return vm;
            }

            var apiRecap = await response.Content.ReadFromJsonAsync<RecapVentesAchatsResponse>(cancellationToken);
            if (apiRecap == null)
            {
                vm.HasError = true;
                vm.ErrorMessage = "Empty response from recap API.";
                return vm;
            }

            vm.VentesFactures = MapSection(apiRecap.VentesFactures);
            vm.VentesAvoirs = MapSection(apiRecap.VentesAvoirs);
            vm.VentesNettes = MapSection(apiRecap.VentesNettes);
            vm.AchatsFacturesFournisseurs = MapSection(apiRecap.AchatsFacturesFournisseurs);
            vm.AchatsAvoirsFournisseur = MapSection(apiRecap.AchatsAvoirsFournisseur);
            vm.AchatsFacturesDepensesTotal = apiRecap.AchatsFacturesDepensesTotal;
            vm.AchatsNets = MapSection(apiRecap.AchatsNets);
            vm.PaiementsClientsTotal = apiRecap.PaiementsClientsTotal;
            vm.PaiementsFournisseursTotal = apiRecap.PaiementsFournisseursTotal;
            vm.PaiementsTiersDepensesTotal = apiRecap.PaiementsTiersDepensesTotal;
            vm.Resultat = MapSection(apiRecap.Resultat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading recap ventes/achats");
            vm.HasError = true;
            vm.ErrorMessage = ex.Message;
        }

        return vm;
    }

    private static RecapTotalsSection MapSection(RecapTotalsSectionDto dto) => new()
    {
        TotalHT = dto.TotalHT,
        TotalTTC = dto.TotalTTC,
        TotalTVA = dto.TotalTVA,
        TotalBase7 = dto.TotalBase7,
        TotalBase13 = dto.TotalBase13,
        TotalBase19 = dto.TotalBase19,
        TotalVat7 = dto.TotalVat7,
        TotalVat13 = dto.TotalVat13,
        TotalVat19 = dto.TotalVat19
    };
}
