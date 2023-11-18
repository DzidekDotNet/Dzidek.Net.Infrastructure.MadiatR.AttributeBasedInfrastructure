using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure.Polly.Net.RetryPolicies;

public sealed class JitterWaitAndRetryPolicyAttribute : BaseInfrastructureAttribute
{
  public int RetryCount { get; set; } = 5;
  public int RetryDelayInMilliseconds { get; set; } = 2000;
  public bool FastFirst { get; set; } = false;

  private ILogger<JitterWaitAndRetryPolicyAttribute>? _logger;
  public override Task<TResponse> Handle<TRequest, TResponse>(
    Func<TRequest, CancellationToken, Task<TResponse>> action,
    TRequest request,
    CancellationToken cancellationToken)
  {
    _logger?.LogDebug("{PolicyName} has been called. RetryCount: '{RetryCount}', RetryDelayInMilliseconds: '{RetryDelayInMilliseconds}', FastFirst: '{FastFirst}'",
      nameof(JitterWaitAndRetryPolicyAttribute), RetryCount, RetryDelayInMilliseconds, FastFirst);

    var delay = Backoff.DecorrelatedJitterBackoffV2(
      TimeSpan.FromMilliseconds(RetryDelayInMilliseconds),
      RetryCount,
      fastFirst: FastFirst);

    return Policy
      .Handle<Exception>()
      .WaitAndRetryAsync(delay)
      .ExecuteAsync(async () => await action(request, cancellationToken));
  }
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    _logger = contextServiceProvider.GetService<ILogger<JitterWaitAndRetryPolicyAttribute>>();
    base.RegisterServices(contextServiceProvider);
  }
}
