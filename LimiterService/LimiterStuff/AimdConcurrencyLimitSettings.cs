using System.ComponentModel.DataAnnotations;

namespace LimiterService.LimiterStuff;

public class AimdConcurrencyLimitSettings : ConcurrencyLimitSettings
{
	/// <summary>
	/// Maximum round trip time to storage, from which we start decreasing concurrent requests
	/// </summary>
	[Range(10, 1000)]
	public int MaxLatencyMilliseconds { get; set; }

	/// <summary>
	/// Determines how aggressive we limit concurrency when a request takes longer than
	/// <see cref="MaxLatencyMilliseconds">MaxLatencyMilliseconds</see>
	/// </summary>
	[Range(0.4, 1)]
	public double BackoffRatio { get; set; }
	
	/// <summary>
	/// Count of requests before limit is recalculated
	/// </summary>
	[Range(10, 1000)]
	public int RecalculationRequestCount { get; set; }
	
	/// <summary>
	/// Percentile which we try to reach
	/// </summary>
	[Range(10, 100)]
	public int TargetPercentile { get; set; }
}