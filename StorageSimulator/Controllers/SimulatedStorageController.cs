using Microsoft.AspNetCore.Mvc;

namespace LimitedStorage.Controllers;

[ApiController]
[Route("[controller]")]
public class SimulatedStorageController : ControllerBase
{
    private readonly LatencySimulator _simulator;

    public SimulatedStorageController(LatencySimulator simulator)
    {
        _simulator = simulator;
    }

    [HttpGet("GetData")]
    public async Task<ActionResult> GetData()
    {
        var latency = _simulator.GetAdjustedLatency();
        await Task.Delay(latency);
        return Ok("Here you go");
    }
}