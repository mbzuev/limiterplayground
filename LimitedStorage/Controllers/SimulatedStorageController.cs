using Microsoft.AspNetCore.Mvc;

namespace LimitedStorage.Controllers;

[ApiController]
[Route("[controller]")]
public class SimulatedStorageController : ControllerBase
{

    private readonly ILogger<SimulatedStorageController> _logger;
    private readonly LatencySimulator _simulator;

    public SimulatedStorageController(ILogger<SimulatedStorageController> logger, LatencySimulator simulator)
    {
        _logger = logger;
        _simulator = simulator;
    }

    [HttpGet("GetData")]
    public async Task<ActionResult> GetData()
    {
        var latency = _simulator.GetAdjustedLatency();
        Console.WriteLine($"Latency simulated={latency}");
        await Task.Delay(latency);
        return Ok("Here you go");
    }
}