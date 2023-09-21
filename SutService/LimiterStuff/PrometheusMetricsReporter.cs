namespace SutService.LimiterStuff;

using System;
using Prometheus;

public class DefaultPrometheusMetricsReporter : IStorageMetrics
{
    private static readonly Gauge _concurrentRequestGauge = Metrics.CreateGauge(
        "reco_cassandra_concurrent_requests",
        "Concurrent requests to cassandra");

    private static readonly Gauge _concurrentRequestLimitGauge = Metrics.CreateGauge(
        "reco_cassandra_concurrent_request_limit",
        "Limit for concurrent requests to cassandra");

    private static readonly Gauge _queueGauge = Metrics.CreateGauge(
        "reco_cassandra_queue",
        "Queue for cassandra requests");

    private static readonly Histogram _storageReadHistogram = Metrics.CreateHistogram(
        "reco_cassandra_read_seconds",
        "Histogram of getting recommendation from cassandra");

    private static readonly Histogram _storageWriteHistogram = Metrics.CreateHistogram(
        "reco_cassandra_write_seconds",
        "Histogram of writing recommendation to cassandra",
        labelNames: new[] { MetricsLabels.TenantId, MetricsLabels.Algorithm, MetricsLabels.InstanceId });

    public ITimer MeasureStorageRead() => _storageReadHistogram.NewTimer();

    public void IncRequests() => _concurrentRequestGauge.Inc();

    public void DecRequests() => _concurrentRequestGauge.Dec();

    public IDisposable RecordStorageRequest() => new ConcurrentRequestRecord(this);

    int IStorageMetrics.GetStorageRequests() => (int)_concurrentRequestGauge.Value;

    public ITimer MeasureStorageWrite(string tenant, string recoTypeSystemName, string instanceId) =>
        _storageWriteHistogram.WithLabels(tenant, recoTypeSystemName, instanceId).NewTimer();

    public void UpdateQueueSize(long value) => _queueGauge.Set(value);

    public void UpdateConcurrentRequestLimit(long value) => _concurrentRequestLimitGauge.Set(value);

    private static void IncForTenant(Counter counter, string tenantId)
    {
        if (counter == null)
            throw new ArgumentNullException(nameof(counter));
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

        counter.WithLabels(tenantId).Inc();
    }
}