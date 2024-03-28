using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace SutService.LimiterStuff;

public interface IConcurrencyLimitCalculator
{
    int Calculate(TimeSpan roundTimeTrip, int currentLimit, int inFlightRequests);
}

public class AimdConcurrencyLimitCalculator : IConcurrencyLimitCalculator
{
    private readonly AimdConcurrencyLimitSettings _settings;
    private readonly TimeSpan _maxLatency;
    private readonly ConcurrentQueue<TimeSpan> _lastRequests = new();

    public AimdConcurrencyLimitCalculator(IOptions<AimdConcurrencyLimitSettings> options)
    {
        _settings = options.Value;
        _maxLatency = TimeSpan.FromMilliseconds(_settings.MaxLatencyMilliseconds);
    }

    public int Calculate(TimeSpan roundTimeTrip, int currentLimit, int inFlightRequests)
    {
        _lastRequests.Enqueue(roundTimeTrip);
        var requests = GetRequestTimes();
        if (requests.Length == 0)
            return currentLimit;

        var percentileLatency = CalculatePercentileLatency(requests, _settings.TargetPercentile);
        var newLimit = currentLimit;
        if (percentileLatency > _maxLatency)
            newLimit = (int)(1d * _settings.BackoffRatio * newLimit);
        else if (CorrectlyUtilized(currentLimit, inFlightRequests))
            newLimit++;
        Console.WriteLine($"Limit - {currentLimit} slowest {percentileLatency} newLimit {newLimit}");

        var result = Math.Min(_settings.MaxConcurrency, Math.Max(newLimit, _settings.MinConcurrency));
        return result;
    }

    private TimeSpan[] GetRequestTimes()
    {
        var requests = Array.Empty<TimeSpan>();
        if (_lastRequests.Count >= _settings.RecalculationRequestCount)
        {
            requests = _lastRequests.ToArray();
            _lastRequests.Clear();
        }

        return requests;
    }

    private static TimeSpan CalculatePercentileLatency(TimeSpan[] timeSpans, int percentile)
    {
        if (timeSpans == null || timeSpans.Length == 0)
            throw new ArgumentException("Input array cannot be null or empty.");

        Array.Sort(timeSpans);
        var index = (int)Math.Ceiling(percentile * timeSpans.Length / 100d) - 1;
        return timeSpans[index];
    }

    private static bool CorrectlyUtilized(int currentLimit, int inFlightRequests)
    {
        return inFlightRequests * 2 + 1 >= currentLimit;
    }
}