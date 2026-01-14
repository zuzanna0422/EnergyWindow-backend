public class EnergyMixService
{
    private readonly EnergyMixClient _client;

    public EnergyMixService(EnergyMixClient client)
    {
        _client = client;
    }

    public async Task<EnergyMix> GetThreeDaysRawAsync()
    {
        var todayUtc = DateTime.UtcNow.Date;
        var from = todayUtc;
        var to = todayUtc.AddDays(2);

        var data = await _client.GetEnergyMixAsync(from, to);
        if (data is null)
        {
            throw new InvalidOperationException("External API returned no data.");
        }

        return data;
    }

    public List<DailyMix> BuildDailyMix(EnergyMix energyMix)
    {
        var cleanFuels = new HashSet<string> { "biomass", "nuclear", "hydro", "wind", "solar" };

        return energyMix.Data
            .GroupBy(entry => entry.From.Date)
            .OrderBy(group => group.Key)
            .Select(group =>
            {
                var allEntries = group.SelectMany(entry => entry.GenerationMix);
                var fuelAverages = allEntries
                    .GroupBy(fuel => fuel.Fuel)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Average(fuel => fuel.Perc)
                    );

                var cleanAverage = allEntries
                    .Where(fuel => cleanFuels.Contains(fuel.Fuel))
                    .GroupBy(fuel => fuel.Fuel)
                    .Sum(group => group.Average(fuel => fuel.Perc));

                return new DailyMix
                {
                    Date = group.Key,
                    Fuel = fuelAverages,
                    CleanEnergyPercentage = Math.Round(cleanAverage, 2)
                };
            }).ToList();
    }
}