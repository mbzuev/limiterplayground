using LimiterService.LimiterStuff;
using Prometheus;

public class DefaultPrometheusMetricsReporter : IStorageMetrics
{
    private static readonly Gauge _concurrentRequestGauge = Metrics.CreateGauge(
        "reco_cassandra_concurrent_requests",
        "Concurrent requests to cassandra");

    private static readonly Gauge _concurrentRequestLimitGauge = Metrics.CreateGauge(
        "reco_cassandra_concurrent_request_limit",
        "Limit for concurrent requests to cassandra");

    private static readonly Gauge QueueGauge = Metrics.CreateGauge(
        "reco_cassandra_queue",
        "Queue for cassandra requests");

    private static readonly Histogram StorageCallHistogram = Metrics.CreateHistogram(
        "storage_call_seconds",
        "Histogram of writing data to storage", new HistogramConfiguration()
        {
            Buckets = new[]
            {
                0.1,
                0.125, 
                0.150, 
                0.175, 
                0.200, 
                0.205, 
                0.210, 
                0.215, 
                0.220, 
                0.270, 
                0.340, 
                0.410, 
                0.500, 
                0.600, 
                0.700, 
                0.800, 
                0.900, 
                1.0,
            }
        });

    public ITimer MeasureStorageCall() => StorageCallHistogram.NewTimer();

    public void IncRequests() => _concurrentRequestGauge.Inc();

    public void DecRequests() => _concurrentRequestGauge.Dec();

    public IDisposable RecordStorageRequest() => new ConcurrentRequestRecord(this);

    int IStorageMetrics.GetStorageRequests() => (int)_concurrentRequestGauge.Value;

    public void UpdateQueueSize(long value) => QueueGauge.Set(value);

    public void UpdateConcurrentRequestLimit(long value) => _concurrentRequestLimitGauge.Set(value);
}