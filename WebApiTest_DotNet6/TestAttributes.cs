using Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;

namespace WebApiTest_DotNet6;

public class MyAttribute1Attribute : BaseInfrastructureAttribute
{
  private ILogger<MyAttribute1Attribute>? _logger;
  public async override Task<TResponse> Handle<TRequest, TResponse>(Func<TRequest, CancellationToken, Task<TResponse>> action, TRequest request, CancellationToken cancellationToken)
  {
    _logger?.LogInformation("MyAttribute1 before");
    TResponse response = await action(request, cancellationToken);
    _logger?.LogInformation("MyAttribute1 after");
    return response;
  }
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    _logger = contextServiceProvider.GetService<ILogger<MyAttribute1Attribute>>();
    base.RegisterServices(contextServiceProvider);
  }
}
public class MyAttribute2Attribute : BaseInfrastructureAttribute
{
  private ILogger<MyAttribute2Attribute>? _logger;
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    _logger = contextServiceProvider.GetService<ILogger<MyAttribute2Attribute>>();
    base.RegisterServices(contextServiceProvider);
  }
  
  public async override Task<TResponse> Handle<TRequest, TResponse>(Func<TRequest, CancellationToken, Task<TResponse>> action, TRequest request, CancellationToken cancellationToken)
  {
    _logger?.LogInformation("MyAttribute2 before");
    TResponse response = await action(request, cancellationToken);
    _logger?.LogInformation("MyAttribute2 after");
    return response;
  }
}

public class MyAttribute3Attribute : BaseInfrastructureAttribute
{
  private ILogger<MyAttribute3Attribute>? _logger;
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    _logger = contextServiceProvider.GetService<ILogger<MyAttribute3Attribute>>();
    base.RegisterServices(contextServiceProvider);
  }
  
  public async override Task<TResponse> Handle<TRequest, TResponse>(Func<TRequest, CancellationToken, Task<TResponse>> action, TRequest request, CancellationToken cancellationToken)
  {
    _logger?.LogInformation("MyAttribute3 before");
    TResponse response = await action(request, cancellationToken);
    _logger?.LogInformation("MyAttribute3 after");
    return response;
  }
}
