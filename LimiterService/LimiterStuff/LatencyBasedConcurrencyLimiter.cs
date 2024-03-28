using Polly;
using Polly.Bulkhead;
using Polly.Contrib.MutableBulkheadPolicy;

namespace LimiterService.LimiterStuff;

public class LatencyBasedConcurrencyLimiter : ConcurrencyLimiterBase
{
	private readonly AsyncMutableBulkheadPolicy _policy;
	private readonly IConcurrencyLimitCalculator _concurrencyLimitCalculator;

	public LatencyBasedConcurrencyLimiter(
		IStorageMetrics metrics,
		AsyncMutableBulkheadPolicy policy,
		IConcurrencyLimitCalculator concurrencyLimitCalculator) : base(metrics)
	{
		_policy = policy;
		_concurrencyLimitCalculator = concurrencyLimitCalculator;
	}

	public override async Task<T> LimitCallAsync<T>(Func<Task<T>> reader)
	{
		var wrappedPolicy = GetFallbackToExceptionPolicy<ReadResult<T>>().WrapAsync(_policy);
		var result = await wrappedPolicy.ExecuteAsync(() => ReadAndMeasureAsync(reader)).ConfigureAwait(false);
		UpdateMaxParallelization(result);
		return result.Value;
	}

	private void UpdateMaxParallelization(OperationResult result)
	{           
		var currentMaxParallelization = _policy.MaxParallelization;
		var newLimit = _concurrencyLimitCalculator.Calculate(
			result.Duration,
			currentMaxParallelization,
			Metrics.GetStorageRequests());
		_policy.MaxParallelization = newLimit;
		Metrics.UpdateQueueSize(_policy.MaxQueueingActions - _policy.QueueAvailableCount);
		Metrics.UpdateConcurrentRequestLimit(newLimit);
	}

	private AsyncPolicy<T> GetFallbackToExceptionPolicy<T>() => Policy<T>
		.Handle<BulkheadRejectedException>()
		.FallbackAsync(
			_ => Task.FromException<T>(new TooManyRequestsException()));
}

public class TooManyRequestsException : Exception
{
}