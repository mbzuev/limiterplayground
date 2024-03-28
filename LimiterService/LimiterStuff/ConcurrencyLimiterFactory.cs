using Microsoft.Extensions.Options;
using Polly.Contrib.MutableBulkheadPolicy;
using Polly.Registry;

namespace LimiterService.LimiterStuff;

public enum RequestType
{
    Sync,
    Bulk
}

public interface IConcurrencyLimiterFactory
{
    public IStorageConcurrencyLimiter Create(RequestType requestType);
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

    public IStorageConcurrencyLimiter Create(RequestType requestType = RequestType.Sync)
    {
        switch (requestType)
        {
            case RequestType.Bulk:
            {
                var initialConcurrency = _settings.MinConcurrency + _settings.MaxConcurrency / 2;
                var policy = _registry.GetOrAdd(
                    requestType.ToString(),
                    AsyncMutableBulkheadPolicy
                        .Create(
                            maxParallelization: initialConcurrency,
                            maxQueuingActions: _settings.MaxQueueSize));
                return new LatencyBasedConcurrencyLimiter(_metrics, policy, _concurrencyLimitCalculator);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
        }
    }
}