using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure.Polly.Net.CircuitBreakerPolicies;

public sealed class AdvancedCircuitBreakerPolicyAttribute : BaseInfrastructureAttribute
{
  public string Key { get; set; }
  public double FailureThreshold { get; set; } = 0.5;
  public int SamplingDurationInSeconds { get; set; } = 10;
  public int MinimumThroughput { get; set; } = 2;
  public int DurationOfBreakInSeconds { get; set; } = 30;

  public AdvancedCircuitBreakerPolicyAttribute(string key)
  {
    Key = key;
  }

  private static ConcurrentDictionary<string, AsyncPolicy> _policyCache = new();
  private ILogger<AdvancedCircuitBreakerPolicyAttribute>? _logger;
  public override Task<TResponse> Handle<TRequest, TResponse>(
    Func<TRequest, CancellationToken, Task<TResponse>> action,
    TRequest request,
    CancellationToken cancellationToken)
  {
    _logger?.LogDebug(
      "{PolicyName} has been called. Key: '{Key}', FailureThreshold: '{FailureThreshold}', SamplingDurationInSeconds: '{SamplingDurationInSeconds}', MinimumThroughput: '{MinimumThroughput}', DurationOfBreakInSeconds: '{DurationOfBreakInSeconds}'",
      nameof(AdvancedCircuitBreakerPolicyAttribute), Key, FailureThreshold, SamplingDurationInSeconds, MinimumThroughput, DurationOfBreakInSeconds);

    return GetPolicy(Key, FailureThreshold, SamplingDurationInSeconds, MinimumThroughput, DurationOfBreakInSeconds, _policyCache)
      .ExecuteAsync(async token => await action(request, token), cancellationToken);
  }
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    _logger = contextServiceProvider.GetService<ILogger<AdvancedCircuitBreakerPolicyAttribute>>();
    base.RegisterServices(contextServiceProvider);
  }

  private AsyncPolicy GetPolicy(
    string key,
    double failureThreshold,
    int samplingDurationInSeconds,
    int minimumThroughput,
    int durationOfBreakInSeconds,
    ConcurrentDictionary<string, AsyncPolicy> policyCache)
  {
    if (!policyCache.ContainsKey(key))
    {
      AsyncPolicy circuitBreaker = Policy.Handle<Exception>()
        .AdvancedCircuitBreakerAsync(
          failureThreshold: failureThreshold,
          samplingDuration: TimeSpan.FromSeconds(samplingDurationInSeconds),
          minimumThroughput: minimumThroughput,
          durationOfBreak: TimeSpan.FromSeconds(durationOfBreakInSeconds),
          OnBreak,
          OnReset,
          OnHalfOpen
        );
      policyCache.AddOrUpdate(key, circuitBreaker, (_, _) => circuitBreaker);
    }

    return policyCache[key];
  }
  private void OnHalfOpen() => 
    _logger?.LogDebug("{PolicyName} with '{Key}' was tried open", nameof(AdvancedCircuitBreakerPolicyAttribute), Key);
  private void OnBreak(Exception e, TimeSpan timeSpan) => 
    _logger?.LogDebug("{PolicyName} with '{Key}' was break", nameof(AdvancedCircuitBreakerPolicyAttribute), Key);
  private void OnReset() => 
    _logger?.LogDebug("{PolicyName} with '{Key}' was reset", nameof(AdvancedCircuitBreakerPolicyAttribute), Key);
}
