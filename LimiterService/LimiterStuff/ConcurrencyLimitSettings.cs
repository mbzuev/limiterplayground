using System.ComponentModel.DataAnnotations;

namespace LimiterService.LimiterStuff;

public class ConcurrencyLimitSettings :IValidatableObject
{
	/// <summary>
	/// Minimum concurrent requests.
	/// </summary>
	[Range(1, 100)]
	public int MinConcurrency { get; set; }

	/// <summary>
	/// Maximum concurrent requests.
	/// </summary>
	[Range(5, 100)]
	public int MaxConcurrency { get; set; }

	/// <summary>
	/// The maximum number of actions that may be queuing, waiting for an execution slot
	/// </summary>
	[Range(0,300)]
	public int MaxQueueSize { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (MinConcurrency >= MaxConcurrency)
		{
			yield return new ValidationResult("MinConcurrency must be lower than MaxConcurrency",
				new[] { nameof(MinConcurrency), nameof(MaxConcurrency) });
		}
	}
}