public interface IStorageConcurrencyLimiter
{
	Task<T> LimitCallAsync<T>(Func<Task<T>> reader);
}