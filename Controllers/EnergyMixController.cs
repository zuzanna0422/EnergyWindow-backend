using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EnergyMixController : ControllerBase
{
    private readonly EnergyMixService _service;

    public EnergyMixController(EnergyMixService service)
    {
        _service = service;
    }

    [HttpGet("daily-mix")]
    public async Task<IActionResult> GetDailyMix()
    {
        var rawData = await _service.GetDaysRawAsync();
        var dailyMix = _service.BuildDailyMix(rawData);
        return Ok(dailyMix);

    }

    [HttpGet("optimal-window")]
    public async Task<IActionResult> GetOptimalWindow([FromQuery, Range(1, 6)] int hours)
    {
        var data = await _service.GetOptimalWindowAsync(hours);
        return Ok(data);
    }

}