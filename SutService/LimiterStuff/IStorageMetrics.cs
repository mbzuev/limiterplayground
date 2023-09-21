using Prometheus;

namespace SutService.LimiterStuff;

public interface IStorageMetrics
{
    ITimer MeasureStorageRead();

    ITimer MeasureStorageWrite(string tenant, string recoTypeSystemName, string instanceId);

    IDisposable RecordStorageRequest();

    int GetStorageRequests();

    void UpdateQueueSize(long value);

    void UpdateConcurrentRequestLimit(long newLimit);
}