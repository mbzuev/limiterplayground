using SutService.LimiterStuff;

public class ConcurrentRequestRecord : IDisposable
{
	private readonly DefaultPrometheusMetricsReporter _metrics;

	public ConcurrentRequestRecord(DefaultPrometheusMetricsReporter metrics)
	{
		_metrics = metrics;
		_metrics.IncRequests();
	}

	public void Dispose()
	{
		_metrics.DecRequests();
	}
}