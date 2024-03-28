namespace LimiterService.LimiterStuff;

public abstract class ConcurrencyLimiterBase : IStorageConcurrencyLimiter
{
    protected readonly IStorageMetrics Metrics;

    protected ConcurrencyLimiterBase(IStorageMetrics metrics)
    {
        Metrics = metrics;
    }

    public abstract Task<T> LimitCallAsync<T>(Func<Task<T>> reader);

    protected async Task<ReadResult<T>> ReadAndMeasureAsync<T>(Func<Task<T>> reader)
    {
        using var timer = Metrics.MeasureStorageCall();
        using var request = Metrics.RecordStorageRequest();

        var result = await reader().ConfigureAwait(false);
        var duration = timer.ObserveDuration();
        return new ReadResult<T>(result, duration);
    }

    protected record OperationResult(TimeSpan Duration);

    protected record ReadResult<T>(T Value, TimeSpan Duration) : OperationResult(Duration);
}