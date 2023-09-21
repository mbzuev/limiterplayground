using System.Collections.Concurrent;

public class LatencySimulator
{
    private const int BaseRPS = 100; // Base requests per second
    private const int BaseLatencyMs = 100; // Base latency in milliseconds for 100 RPS
    private const int MaxStoredRequests = 1000; // Maximum number of stored requests

    private ConcurrentQueue<DateTime> requestTimes;

    public LatencySimulator()
    {
        requestTimes = new ConcurrentQueue<DateTime>();
    }

    public TimeSpan GetAdjustedLatency()
    {
        // Remove requests older than 1 second
        var cutoffTime = DateTime.Now.AddSeconds(-1);
        while (requestTimes.TryPeek(out DateTime oldestRequest) && oldestRequest < cutoffTime)
        {
            requestTimes.TryDequeue(out _);
        }
        requestTimes.Enqueue(DateTime.Now);

        // Calculate the current requests per second (RPS)
        int currentRPS = requestTimes.Count;

        // Calculate the ratio of the current RPS to the base RPS
        double rpsRatio = (double)currentRPS / BaseRPS;

        // Adjust the latency proportionally based on the RPS ratio
        int adjustedLatencyMs = (int)(BaseLatencyMs * rpsRatio);

        // Create a TimeSpan representing the adjusted latency
        return TimeSpan.FromMilliseconds(adjustedLatencyMs);
    }

    public void AddRequest()
    {
        // Enforce a limit on the number of stored requests
        while (requestTimes.Count >= MaxStoredRequests)
        {
            requestTimes.TryDequeue(out _);
        }

        // Add the current time to the request times queue
        requestTimes.Enqueue(DateTime.Now);
    }
}