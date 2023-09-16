using Microsoft.AspNetCore.Mvc;

namespace LimitedStorage.Controllers;

[ApiController]
[Route("[controller]")]
public class SimulatedStorageController : ControllerBase
{

    private readonly ILogger<SimulatedStorageController> _logger;
    private readonly LatencySimulator _simulator;

    public SimulatedStorageController(ILogger<SimulatedStorageController> logger)
    {
        _logger = logger;
        _simulator = new LatencySimulator(3,2);
    }

    [HttpGet("GetData")]
    public async Task<ActionResult> GetData()
    {
        var latency = _simulator.GetLatency()*0.1;
        Console.WriteLine($"Latency simulated={latency}");
        await Task.Delay(TimeSpan.FromSeconds(latency));
        return Ok();
    }
}