using Microsoft.Extensions.Options;
using Polly.Contrib.MutableBulkheadPolicy;
using Polly.Registry;
using SutService.LimiterStuff;


public enum RecoSlaType { Sync, Bulk
}

public interface IConcurrencyLimiterFactory
{
	public IStorageConcurrencyLimiter Create(RecoSlaType slaType);
}

public class ConcurrencyLimiterFactory : IConcurrencyLimiterFactory
{
	private readonly PolicyRegistry _registry = new();

	private readonly ConcurrencyLimitSettings _settings;

	private readonly IStorageMetrics _metrics;

	private readonly IConcurrencyLimitCalculator _concurrencyLimitCalculator;

	public ConcurrencyLimiterFactory(
		IOptions<ConcurrencyLimitSettings> options,
		IStorageMetrics metrics,
		IConcurrencyLimitCalculator concurrencyLimitCalculator)
	{
		_settings = options.Value;
		_metrics = metrics;
		_concurrencyLimitCalculator = concurrencyLimitCalculator;
	}

	public IStorageConcurrencyLimiter Create(RecoSlaType slaType = RecoSlaType.Sync)
	{
		switch (slaType)
		{
			case RecoSlaType.Bulk:
			{
				var initialConcurrency = _settings.MinConcurrency + _settings.MaxConcurrency / 2;
				var policy = _registry.GetOrAdd(
					slaType.ToString(),
					AsyncMutableBulkheadPolicy
						.Create(
							maxParallelization: initialConcurrency,
							maxQueuingActions: _settings.MaxQueueSize));
				return new LatencyBasedConcurrencyLimiter(_metrics, policy, _concurrencyLimitCalculator);
			}
			default:
				throw new ArgumentOutOfRangeException(nameof(slaType), slaType, null);
		}
	}
}