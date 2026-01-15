using System.Net.Http.Json;

public class EnergyMixClient
{
    private readonly HttpClient _httpClient;

    public EnergyMixClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EnergyMix> GetEnergyMixAsync(DateTime fromUrl, DateTime toUrl)
    {
        var from = fromUrl.ToString("yyyy-MM-ddTHH:mmZ");
        var to = toUrl.ToString("yyyy-MM-ddTHH:mmZ");
        var url = $"https://api.carbonintensity.org.uk/generation/{from}/{to}";
        return await _httpClient.GetFromJsonAsync<EnergyMix>(url) ?? throw new InvalidOperationException("Energy response was empty.");
    }
}