using Microsoft.AspNetCore.Mvc;

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

    public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpGet(Name = "Doggie")]
    public IEnumerable<WeatherForecast> Get()
    {
        _httpClient.PostAsync("http://localhost:")
        
    }
}