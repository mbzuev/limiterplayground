using System.Collections.Concurrent;
using Polly;
using Polly.Bulkhead;
using Polly.Contrib.MutableBulkheadPolicy;

namespace SutService.LimiterStuff;

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

	public override async Task<T> LimitReadAsync<T>(Func<Task<T>> reader)
	{
		var wrappedPolicy = GetFallbackToExceptionPolicy<ReadResult<T>>().WrapAsync(_policy);
		var result = await wrappedPolicy.ExecuteAsync(() => ReadAndMeasureAsync(reader)).ConfigureAwait(false);

		UpdateMaxParallelization(result);
		return result.Value;
	}

	public override async Task LimitWriteAsync(Func<Task> writer)
	{
		var wrappedPolicy = GetFallbackToExceptionPolicy<OperationResult>().WrapAsync(_policy);
		var result = await wrappedPolicy.ExecuteAsync(() => WriteAndMeasureAsync(writer)).ConfigureAwait(false);
		UpdateMaxParallelization(result);
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
			_ => Task.FromException<T>(new TooManyRequestsException($"Too many requests for Reco service")));
}

public class TooManyRequestsException : Exception
{
	public TooManyRequestsException(string tooManyRequestsForRecoService)
	{
		
	}
}