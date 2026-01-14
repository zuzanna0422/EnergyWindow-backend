public class DailyMix
{
    public DateTime Date { get; set; }
    public Dictionary<string, double> Fuel { get; set; } = new();
    public double CleanEnergyPercentage { get; set; }
}