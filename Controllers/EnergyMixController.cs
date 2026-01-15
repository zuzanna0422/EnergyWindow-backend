using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EnergyMixController : ControllerBase
{
    private readonly EnergyMixClient _client;
    private readonly EnergyMixService _service;

    public EnergyMixController(EnergyMixClient client, EnergyMixService service)
    {
        _client = client;
        _service = service;
    }

    [HttpGet("daily-mix")]
    public async Task<IActionResult> GetDailyMix()
    {
        var rawData = await _service.GetThreeDaysRawAsync();
        var dailyMix = _service.BuildDailyMix(rawData);
        return Ok(dailyMix);

    }

    [HttpGet("optimal-window")]
    public async Task<IActionResult> GetOptimalWindow(int hours)
    {
        var data = await _service.GetOptimalWindowAsync(hours);
        return Ok(data);
    }

    [HttpGet("generation")]
    public async Task<IActionResult> GetGeneration([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var data = await _client.GetEnergyMixAsync(from, to);
        if (data is null)
        {
            return StatusCode(502, "External API returned no data.");
        }

        return Ok(data);
    }
}