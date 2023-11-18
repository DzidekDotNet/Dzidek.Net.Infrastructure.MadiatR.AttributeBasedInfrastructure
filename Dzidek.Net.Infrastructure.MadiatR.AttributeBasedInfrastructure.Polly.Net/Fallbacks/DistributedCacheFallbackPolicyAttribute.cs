using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure.Polly.Net.Fallbacks;

public sealed class DistributedCacheFallbackPolicyAttribute : BaseInfrastructureAttribute
{
  public string CacheKey { get; set; }

  public DistributedCacheFallbackPolicyAttribute(string cacheKey) =>
    CacheKey = cacheKey;


  private ILogger<DistributedCacheFallbackPolicyAttribute>? _logger;
  private IDistributedCache? _cache;
  public override Task<TResponse> Handle<TRequest, TResponse>(
    Func<TRequest, CancellationToken, Task<TResponse>> action,
    TRequest request,
    CancellationToken cancellationToken)
  {
    _logger?.LogDebug("{PolicyName} has been called. CacheKey: '{CacheKey}'",
      nameof(DistributedCacheFallbackPolicyAttribute), CacheKey);
    
    return Policy<TResponse>
      .Handle<Exception>()
      .FallbackAsync(async token => await Fallback<TResponse>(CacheKey, token))
      .ExecuteAsync(async token =>
      {
        try
        {
          var result = await action(request, token);
          var serializedValue = JsonSerializer.Serialize(result);
          _ = _cache?.SetStringAsync(CacheKey, serializedValue, token);
          return result;
        }
        catch (Exception e)
        {
          _logger?.LogWarning(e, e.Message);
          throw;
        }

      }, cancellationToken);
  }
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    _logger = contextServiceProvider.GetService<ILogger<DistributedCacheFallbackPolicyAttribute>>();
    _cache = contextServiceProvider.GetRequiredService<IDistributedCache>();
    base.RegisterServices(contextServiceProvider);
  }

  private async Task<TResponse> Fallback<TResponse>(string cacheKey, CancellationToken cancellationToken)
  {
    if (_cache is null)
    {
      throw new CannotGetValueFromCacheException("Cache is null");
    }
    _logger?.LogDebug("Get value from cache with key {key}", cacheKey);
    var response = await _cache.GetStringAsync(cacheKey, cancellationToken);
    return JsonSerializer.Deserialize<TResponse>(response 
                                                 ?? throw new CannotGetValueFromCacheException($"There is no value in cache for this key: '{cacheKey}'"))
           ?? throw new CannotGetValueFromCacheException("Cannot serialize cache value");
  }
}

public class CannotGetValueFromCacheException : Exception
{
  public CannotGetValueFromCacheException(string message) : base(message)
  {

  }
}
