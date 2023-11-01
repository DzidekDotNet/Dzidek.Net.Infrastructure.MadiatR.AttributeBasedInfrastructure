using Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;
using Polly;
using Polly.Contrib.WaitAndRetry;


namespace WebApiTest_DotNet6.RetryPolicy;

internal sealed class RetryPolicyAttribute : BaseInfrastructureAttribute
{
  private ILogger<MyAttribute1Attribute>? _logger;
  public override Task<TResponse> Handle<TRequest, TResponse>(Func<TRequest, CancellationToken, Task<TResponse>> action, TRequest request, CancellationToken cancellationToken)
  {
    _logger?.LogInformation("RetryPolicyAttribute has been called");
    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);
    return Policy
      .Handle<Exception>()
      .WaitAndRetryAsync(delay)
      .ExecuteAsync(async () => await action(request, cancellationToken));
  }
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    _logger = contextServiceProvider.GetService<ILogger<MyAttribute1Attribute>>();
    base.RegisterServices(contextServiceProvider);
  }
}
