using Microsoft.AspNetCore.Mvc;
using SutService.LimiterStuff;

namespace SutService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConcurrencyLimiterFactory _factory;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient,
        IConcurrencyLimiterFactory factory)
    {
        _logger = logger;
        _httpClient = httpClient;
        _factory = factory;
    }

    [HttpGet(Name = "Doggie")]
    public async Task<ActionResult> Get()
    {
        try
        {
            var limiter = _factory.Create(RecoSlaType.Bulk);
            // var storageUrl = "http://localhost:1234/SimulatedStorage/GetData";
            // var result = await limiter.LimitReadAsync(async () => await _httpClient.GetAsync(storageUrl));
            var storageUrl = "http://mystorage/SimulatedStorage/GetData";
            var result = await _httpClient.GetAsync(storageUrl);
            return result.IsSuccessStatusCode ? Ok("Good") : new BadRequestResult();
        }
        catch (TooManyRequestsException)
        {
            // Console.WriteLine("AAAA STATUS 429");
            return new StatusCodeResult(429);
        }
    }
}