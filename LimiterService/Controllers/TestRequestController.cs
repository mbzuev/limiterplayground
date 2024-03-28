using Microsoft.AspNetCore.Mvc;
using LimiterService.LimiterStuff;

namespace LimiterService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestRequestController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConcurrencyLimiterFactory _factory;
    private readonly IStorageMetrics _metrics;

    private readonly string _storageUrl = "http://mystorage/SimulatedStorage/GetData";

    public TestRequestController(HttpClient httpClient,
        IConcurrencyLimiterFactory factory, IStorageMetrics metrics)
    {
        _httpClient = httpClient;
        _factory = factory;
        _metrics = metrics;
    }

    [HttpGet]
    [Route("Limited")]
    public async Task<ActionResult> Get()
    {
        try
        {
            var limiter = _factory.Create(RequestType.Bulk);
            var result = await limiter.LimitCallAsync(async () => await _httpClient.GetAsync(_storageUrl));
            return result.IsSuccessStatusCode ? Ok("Good, went through limiter") : new BadRequestResult();
        }
        catch (TooManyRequestsException)
        {
            return new StatusCodeResult(429);
        }
    }

    [HttpGet]
    [Route("Usual")]
    public async Task<ActionResult> GetUsual()
    {
        try
        {
            using var timer = _metrics.MeasureStorageCall();
            var result = await _httpClient.GetAsync(_storageUrl);
            return result.IsSuccessStatusCode ? Ok("Good, went through limiter") : new BadRequestResult();
        }
        catch (TooManyRequestsException)
        {
            return new StatusCodeResult(429);
        }
    }
}