using SutService.LimiterStuff;

public abstract class ConcurrencyLimiterBase : IStorageConcurrencyLimiter
{
    protected readonly IStorageMetrics Metrics;

    protected ConcurrencyLimiterBase(IStorageMetrics metrics)
    {
        Metrics = metrics;
    }

    public abstract Task<T> LimitReadAsync<T>(Func<Task<T>> reader);

    public abstract Task LimitWriteAsync(Func<Task> writer);

    protected async Task<ReadResult<T>> ReadAndMeasureAsync<T>(Func<Task<T>> reader)
    {
        using var timer = Metrics.MeasureStorageRead();
        using var request = Metrics.RecordStorageRequest();

        var result = await reader().ConfigureAwait(false);
        return new ReadResult<T>(result, timer.ObserveDuration());
    }

    protected async Task<OperationResult> WriteAndMeasureAsync(Func<Task> writer)
    {
        using var timer = Metrics.MeasureStorageWrite("", "", "");
        await writer().ConfigureAwait(false);
        return new OperationResult(timer.ObserveDuration());
    }

    protected record OperationResult(TimeSpan Duration);

    protected record ReadResult<T>(T Value, TimeSpan Duration) : OperationResult(Duration);
}