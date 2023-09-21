

using System.ComponentModel.DataAnnotations;

namespace SutService.LimiterStuff;

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
}