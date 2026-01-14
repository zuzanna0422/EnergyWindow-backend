using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EnergyMixController : ControllerBase
{
    private readonly EnergyMixClient _client;

    public EnergyMixController(EnergyMixClient client)
    {
        _client = client;
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