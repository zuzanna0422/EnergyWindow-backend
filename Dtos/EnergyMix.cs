using System.Text.Json.Serialization;

public class EnergyMix
{
    [JsonPropertyName("data")]
    public List<Interval> Data { get; set; }

    public class Interval
    {
        [JsonPropertyName("from")]
        public DateTime From { get; set; }

        [JsonPropertyName("to")]
        public DateTime To { get; set; }

        [JsonPropertyName("generationmix")]
        public List<GenerationMix> GenerationMix { get; set; }
    }
    public class GenerationMix
    {
        [JsonPropertyName("fuel")]
        public string Fuel { get; set; }

        [JsonPropertyName("perc")]
        public double Perc { get; set; }
    }

}