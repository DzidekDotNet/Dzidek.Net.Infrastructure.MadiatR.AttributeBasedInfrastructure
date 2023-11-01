using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;

public sealed class AttributeBasedInfrastructureBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  private static ConcurrentDictionary<Type, List<BaseInfrastructureAttribute>> _reflectionCache = new();
  private readonly ILogger<AttributeBasedInfrastructureBehaviour<TRequest, TResponse>> _logger;
  private readonly IServiceProvider _serviceProvider;
  public AttributeBasedInfrastructureBehaviour(ILogger<AttributeBasedInfrastructureBehaviour<TRequest, TResponse>> logger, IServiceProvider serviceProvider)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  private Func<Func<TRequest, CancellationToken, Task<TResponse>>, TRequest, CancellationToken, Task<TResponse>> Combine(
    Func<Func<TRequest, CancellationToken, Task<TResponse>>, TRequest, CancellationToken, Task<TResponse>>[] functions)
  {
    switch (functions.Length)
    {
      case 1:
        return functions[0];
      case >= 2:
      {
        Func<Func<TRequest, CancellationToken, Task<TResponse>>, TRequest, CancellationToken, Task<TResponse>> chainOfFunctions =
          DecorateOneFunctionWithAnother(functions[0], functions[1]);
        
        if (functions.Length == 2)
          return chainOfFunctions;
        
        for (int i = 2; i < functions.Length; i++)
        {
          chainOfFunctions = DecorateOneFunctionWithAnother(chainOfFunctions, functions[i]);
        }
        return chainOfFunctions;
      }
      default:
        throw new ArgumentException("Array should contains at least one element", nameof(functions));
    }
  }

  private Func<Func<TRequest, CancellationToken, Task<TResponse>>, TRequest, CancellationToken, Task<TResponse>> DecorateOneFunctionWithAnother(
    Func<Func<TRequest, CancellationToken, Task<TResponse>>, TRequest, CancellationToken, Task<TResponse>> firstFunction,
    Func<Func<TRequest, CancellationToken, Task<TResponse>>, TRequest, CancellationToken, Task<TResponse>> secondFunction)
  {
    return (fun, request, cancellationToken) => firstFunction((rz, cz) => 
      secondFunction(
        (rx, cx) => fun(rx, cx), 
        rz, 
        cz), 
      request, 
      cancellationToken);
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    _logger.LogDebug("The AttributeBasedInfrastructureBehaviour 'Handle' method has been called");
    Type requestType = request.GetType();
    if (!_reflectionCache.ContainsKey(requestType))
    {
      var typeAttributes = requestType.GetCustomAttributes<BaseInfrastructureAttribute>().ToList();
      _reflectionCache.AddOrUpdate(requestType, typeAttributes, (type, list) => typeAttributes);
    }
    var attributes = _reflectionCache[requestType];
    _logger.LogTrace("Request '{requestType}' has '{attributes}' attributes defined", requestType.FullName, attributes.Select(x => x.GetType().FullName));

    if (attributes.Count == 0)
    {
      return await next();
    }
    var attributesFunctions = new List<Func<Func<TRequest, CancellationToken, Task<TResponse>>, TRequest, CancellationToken, Task<TResponse>>>();
    foreach (var attribute in attributes)
    {
      attribute.RegisterServices(_serviceProvider);
      attributesFunctions.Add(
        (fun, requestData, token) => attribute.Handle(fun, requestData, token));
    }

    return await Combine(attributesFunctions.ToArray())((_, _) => next(), request, cancellationToken);
  }
}
