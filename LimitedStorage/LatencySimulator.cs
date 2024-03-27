using System.Collections.Concurrent;
namespace LimitedStorage;

public class LatencySimulator
{
    private readonly ConcurrentQueue<DateTime> _lastRequests = new();
    private const int BaseRps = 50; // Base requests per second
    private const int BaseLatencyMs = 180; // Base latency in milliseconds for BaseRps
    
    public TimeSpan GetAdjustedLatency()
    {
        _lastRequests.Enqueue(DateTime.Now);
        while(_lastRequests.TryPeek(out var request) && (DateTime.Now - request).TotalMilliseconds >=1000)
        {
            _lastRequests.TryDequeue(out _);
        }

        var rps = _lastRequests.Count(x => (DateTime.Now - x).TotalMilliseconds < 1000);
        var rpsRatio = 1d*rps / BaseRps;
        var adjustedLatencyMs = BaseLatencyMs * rpsRatio;
        Console.WriteLine($"RPS={rps} adjusted latency {adjustedLatencyMs}");
        return TimeSpan.FromMilliseconds(adjustedLatencyMs);
    }
}