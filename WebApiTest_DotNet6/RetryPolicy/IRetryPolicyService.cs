using Polly;
using Polly.Contrib.WaitAndRetry;

namespace WebApiTest_DotNet6.RetryPolicy;

public interface IRetryPolicyService
{
  public Task<T> CallWithRetryAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}

internal sealed class RetryPolicyService : IRetryPolicyService
{
  public Task<T> CallWithRetryAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
  {
    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);
    return Policy
      .Handle<Exception>()
      .WaitAndRetryAsync(delay)
      .ExecuteAsync(async () => await action(cancellationToken));
  }
}