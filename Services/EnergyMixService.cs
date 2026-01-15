public class EnergyMixService
{
    private readonly EnergyMixClient _client;

    public EnergyMixService(EnergyMixClient client)
    {
        _client = client;
    }

    public async Task<EnergyMix> GetThreeDaysRawAsync()
    {
        var ukTz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        var todayUk = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ukTz).Date;
        var from = TimeZoneInfo.ConvertTimeToUtc(todayUk, ukTz);
        var to = TimeZoneInfo.ConvertTimeToUtc(todayUk.AddDays(3), ukTz);

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
        var ukTz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        return energyMix.Data
            .GroupBy(entry => TimeZoneInfo.ConvertTimeFromUtc(entry.From, ukTz).Date)
            .OrderBy(group => group.Key)
            .Skip(1)
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