using Prometheus;

public interface IStorageMetrics
{
    ITimer MeasureStorageCall();

    IDisposable RecordStorageRequest();

    int GetStorageRequests();

    void UpdateQueueSize(long value);

    void UpdateConcurrentRequestLimit(long newLimit);
}