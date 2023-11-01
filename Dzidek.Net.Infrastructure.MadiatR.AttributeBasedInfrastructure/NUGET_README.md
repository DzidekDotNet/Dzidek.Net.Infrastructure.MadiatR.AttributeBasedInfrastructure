#Extendable attribute-based MediatR infrastructure framework
##An easy way to focus on what is most important and critical - business requirements

### Main concept
The framework provides an extensible and configurable way to implement infrastructure code such as RetryPolicy, Circuit breaker, Timeout and another infrastructure

### How to start
#### Install the required package

To install a Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure module into project, Nuget Package Manager Console can be used:

```
Install-Package Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure -ProjectName <ProjectName>
```

#### Create your own infrastructure attribute
Create your own infrastructure attribute
```csharp
using Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;
...
public class MyAttribute1Attribute : BaseInfrastructureAttribute
{
  public override Task<TResponse> Handle<TRequest, TResponse>(Func<TRequest, CancellationToken, Task<TResponse>> action, TRequest request, CancellationToken cancellationToken)
  {
  }
}
```
For example RetryPolicy using Polly.Net
```csharp
using Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure;
using Polly;
using Polly.Contrib.WaitAndRetry;
...
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
```

#### Get services in your attribute if you need them
To get service instance in your attribute override method RegisterServices.
```csharp
  public override void RegisterServices(IServiceProvider contextServiceProvider)
  {
    //Get services right here
    //var service = contextServiceProvider.GetRequiredService<IService>();
    base.RegisterServices(contextServiceProvider);
  }
```

#### Register the attributes in the IoC container
Register MediatR attribute based infrastructure in IoC
```csharp
using Dzidek.Net.Infrastructure.AttributeBasedInfrastructure.Middlewares;
...
builder.Services
  .AddMediatRAttributeBasedInfrastructure();
```

#### Decorate query with your attribute
```csharp
[MyAttribute1]
[RetryPolicy]
internal sealed class AttributedImplementationQuery : IRequest<int> { }
```

#### Call sequence
Attributes are called from top to bottom.
The sequence of calls is similar to the DotNet middleware.
For more information please go to https://github.com/DzidekDotNet/Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure

#### Working example
Working example you can find in project repo https://github.com/DzidekDotNet/Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure

### Changelog
- 1.0.0
  - Working example
  - Abstract attribute
  - IoC registration

## Nuget packages
[Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure](https://www.nuget.org/packages/Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure)

## Authors
[@DzidekDotNet](https://www.github.com/DzidekDotNet)

## License
[MIT](https://github.com/DzidekDotNet/Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure/blob/main/LICENSE)