namespace SutService.LimiterStuff;

public interface IStorageConcurrencyLimiter
{
	Task<T> LimitReadAsync<T>(Func<Task<T>> reader);
	Task LimitWriteAsync(Func<Task> writer);
}