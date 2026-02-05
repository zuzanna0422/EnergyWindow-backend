public class EnergyMixService
{
    private readonly EnergyMixClient _client;

    public EnergyMixService(EnergyMixClient client)
    {
        _client = client;
    }

    public async Task<EnergyMix> GetDaysRawAsync(int days = 3)
    {
        var (from, to) = GetUtcRangeForNextDays(days);
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

    public async Task<OptimalWindow> GetOptimalWindowAsync(int hours)
    {
        if (hours < 1 || hours > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be between 1 and 6.");
        }

        var (from, to) = GetUtcRangeForNextDays(2);
        var data = await _client.GetEnergyMixAsync(from, to);
        if (data is null)
        {
            throw new InvalidOperationException("External API returned no data.");
        }

        var cleanFuels = new HashSet<string> { "biomass", "nuclear", "hydro", "wind", "solar" };

        var intervals = data.Data
            .Where(intervals => intervals.To > DateTime.UtcNow)
            .OrderBy(intervals => intervals.From)
            .Select(intervals => new
            {
                intervals.From,
                intervals.To,
                Clean = intervals.GenerationMix
                    .Where(fuel => cleanFuels.Contains(fuel.Fuel))
                    .Sum(fuel => fuel.Perc)
            }).ToList();

        var windowSize = hours * 2;
        if (intervals.Count < windowSize)
        {
            throw new InvalidOperationException("Not enough data to determine optimal window.");
        }

        double bestAvg = -1;
        DateTime bestFrom = default;
        DateTime bestTo = default;

        for (int i = 0; i <= intervals.Count - windowSize; i++)
        {
            var avg = intervals
                .Skip(i)
                .Take(windowSize)
                .Average(x => x.Clean);
            if (avg > bestAvg)
            {
                bestAvg = avg;
                bestFrom = intervals[i].From;
                bestTo = intervals[i + windowSize - 1].To;
            }
        }

        return new OptimalWindow
        {
            From = bestFrom,
            To = bestTo,
            CleanEnergyPercentage = Math.Round(bestAvg, 2)
        };

    }

    private (DateTime fromUtc, DateTime toUtc) GetUtcRangeForNextDays(int days)
    {
        var ukTz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        var todayUk = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ukTz).Date;

        var fromUtc = TimeZoneInfo.ConvertTimeToUtc(todayUk, ukTz);
        var toUtc = TimeZoneInfo.ConvertTimeToUtc(todayUk.AddDays(days), ukTz);

        return (fromUtc, toUtc);
    }
}