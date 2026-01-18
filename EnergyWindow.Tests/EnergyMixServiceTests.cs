using Xunit;

namespace EnergyWindow.Tests;

public class EnergyMixServiceTests
{
    [Fact]
    public async Task GetOptimalWindowAsync_InvalidHours_Throws()
    {
        var service = new EnergyMixService(null!);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetOptimalWindowAsync(0));
    }

    [Fact]
    public void BuildDailyMix_ReturnsDailySummary()
    {
        var service = new EnergyMixService(null!);

        var day1 = new DateTime(2024, 01, 10, 0, 0, 0, DateTimeKind.Utc);
        var day2 = day1.AddDays(1);

        var mix = new EnergyMix
        {
            Data =
            {
                new EnergyMix.Interval
                {
                    From = day1,
                    To = day1.AddMinutes(30),
                    GenerationMix =
                    {
                        new EnergyMix.GenerationMix { Fuel = "wind", Perc = 40 },
                        new EnergyMix.GenerationMix { Fuel = "gas", Perc = 60 }
                    }
                },
                new EnergyMix.Interval
                {
                    From = day2,
                    To = day2.AddMinutes(30),
                    GenerationMix =
                    {
                        new EnergyMix.GenerationMix { Fuel = "solar", Perc = 50 },
                        new EnergyMix.GenerationMix { Fuel = "gas", Perc = 50 }
                    }
                },
                new EnergyMix.Interval
                {
                    From = day2.AddMinutes(30),
                    To = day2.AddMinutes(60),
                    GenerationMix =
                    {
                        new EnergyMix.GenerationMix { Fuel = "wind", Perc = 30 },
                        new EnergyMix.GenerationMix { Fuel = "gas", Perc = 70 }
                    }
                }
            }
        };

        var result = service.BuildDailyMix(mix);

        Assert.Single(result);
        Assert.Equal(day2.Date, result[0].Date);
        Assert.Equal(80, result[0].CleanEnergyPercentage);
    }

    [Fact]
    public async Task GetOptimalWindowAsync_TooLargeHours_Throws()
    {
        var service = new EnergyMixService(null!);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetOptimalWindowAsync(7));
    }

    [Fact]
    public void BuildDailyMix_WhenOnlyOneDay_ReturnsEmpty()
    {
        var service = new EnergyMixService(null!);

        var day1 = new DateTime(2024, 01, 10, 0, 0, 0, DateTimeKind.Utc);

        var mix = new EnergyMix
        {
            Data =
            {
                new EnergyMix.Interval
                {
                    From = day1,
                    To = day1.AddMinutes(30),
                    GenerationMix =
                    {
                        new EnergyMix.GenerationMix { Fuel = "wind", Perc = 40 },
                        new EnergyMix.GenerationMix { Fuel = "gas", Perc = 60 }
                    }
                }
            }
        };

        var result = service.BuildDailyMix(mix);

        Assert.Empty(result);
    }
}