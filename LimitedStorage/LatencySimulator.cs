using System.Collections.Concurrent;

namespace LimitedStorage;

public class LatencySimulator
{
    private const int BaseRPS = 10; // Base requests per second
    private const int BaseLatencyMs = 100; // Base latency in milliseconds for 100 RPS
    private const int MaxStoredRequests = 1000; // Maximum number of stored requests

    private readonly ConcurrentQueue<DateTime> _requests = new();

    public TimeSpan GetAdjustedLatency()
    {
        RemoveOutdatedRequests();
        var currentRps = _requests.Count;
        var rpsRatio = (double)currentRps / BaseRPS;
        var adjustedLatencyMs = BaseLatencyMs * rpsRatio;
        return TimeSpan.FromMilliseconds(adjustedLatencyMs);
    }

    private void RemoveOutdatedRequests()
    {
        var cutoffTime = DateTime.Now.AddSeconds(-1);
        while (_requests.TryPeek(out DateTime oldestRequest) && oldestRequest < cutoffTime)
        {
            _requests.TryDequeue(out _);
        }

        _requests.Enqueue(DateTime.Now);
    }

    public void AddRequest()
    {
        // Enforce a limit on the number of stored requests
        while (_requests.Count >= MaxStoredRequests)
        {
            _requests.TryDequeue(out _);
        }

        // Add the current time to the request times queue
        _requests.Enqueue(DateTime.Now);
    }
}