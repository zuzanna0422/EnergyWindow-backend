using System.Net.Http.Json;

public class EnergyMixClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public EnergyMixClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<EnergyMix> GetEnergyMixAsync(DateTime fromUrl, DateTime toUrl)
    {
        var from = fromUrl.ToString("yyyy-MM-ddTHH:mmZ");
        var to = toUrl.ToString("yyyy-MM-ddTHH:mmZ");
        var baseUrl = _configuration["EnergyApi:BaseUrl"] ?? throw new InvalidOperationException("Energy API base URL is not configured.");
        var url = $"{baseUrl}/generation/{from}/{to}";
        return await _httpClient.GetFromJsonAsync<EnergyMix>(url) ?? throw new InvalidOperationException("Energy response was empty.");
    }
}