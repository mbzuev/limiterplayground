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
		TimeSpan[] requests = Array.Empty<TimeSpan>();
		if (_lastRequests.Count >= 100)
		{
			requests = _lastRequests.ToArray();
			_lastRequests.Clear();
		}
		if (requests.Length == 0)
			return currentLimit;

		var percentile = CalculatePercentile(requests, 99);
		
		var newLimit = currentLimit;
		if (percentile > _maxLatency)
		{
			newLimit = (int)(_settings.BackoffRatio * newLimit);
		}
		else if (CorrectlyUtilized(currentLimit, inFlightRequests))
			newLimit++;

		var result =  Math.Min(_settings.MaxConcurrency, Math.Max(newLimit, _settings.MinConcurrency));
		return result;
	}

	private static TimeSpan CalculatePercentile(TimeSpan[] timeSpans, int percentile)
	{
		if (timeSpans == null || timeSpans.Length == 0)
			throw new ArgumentException("Input array cannot be null or empty.");

		Array.Sort(timeSpans);
		var index = (int)Math.Ceiling(percentile/100d * timeSpans.Length) - 1;
		return timeSpans[index];
	}

	private static bool CorrectlyUtilized(int currentLimit, int inFlightRequests)
	{
		return inFlightRequests * 2 >= currentLimit;
	}
}