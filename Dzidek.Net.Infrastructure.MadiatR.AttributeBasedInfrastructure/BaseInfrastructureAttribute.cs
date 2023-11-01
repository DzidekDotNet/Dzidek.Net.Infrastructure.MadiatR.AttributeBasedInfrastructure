namespace Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class BaseInfrastructureAttribute: Attribute
{
  public abstract Task<TResponse> Handle<TRequest, TResponse>(Func<TRequest, CancellationToken, Task<TResponse>> action, TRequest request,  CancellationToken cancellationToken);
  
  public virtual void RegisterServices(IServiceProvider contextServiceProvider)
  {
  }
}
