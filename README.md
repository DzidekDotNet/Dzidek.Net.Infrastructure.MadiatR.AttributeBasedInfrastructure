<a name="readme-top"></a>
<h1 style="text-align: center;">Extendable attribute-based MediatR infrastructure framework</h1>
<h3 style="text-align: center;">An easy way to focus on what is most important and critical - business requirements</h3>
<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#main-concept">Main concept</a></li>
    <li>
        <a href="#how-to-start">How to start</a>
        <ol>
            <li><a href="#install-the-required-package">Install the required package</a></li>
            <li><a href="#create-your-own-infrastructure-attribute">Create your own infrastructure attribute</a></li>
            <li><a href="#get-services-in-your-attribute-if-you-need-them">Get services in your attribute if you need them</a></li>
            <li><a href="#register-the-attributes-in-the-ioc-container">Register the attributes in the IoC container</a></li>
            <li><a href="#decorate-query-with-your-attribute">Decorate query with your attribute</a></li>
            <li><a href="#call-sequence">Call sequence</a></li>
            <li><a href="#working-example">Working example</a></li>
        </ol>
    </li>
    <li><a href="#changelog">Changelog</a></li>
    <li><a href="#nugetpackages">Nuget packages</a></li>
    <li><a href="#authors">Authors</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

### Main concept
The framework provides an extensible and configurable way to implement infrastructure code such as RetryPolicy, Circuit breaker, Timeout and another infrastructure
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

### How to start
#### Install the required package

To install a Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure module into project, Nuget Package Manager Console can be used:

```
Install-Package Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure -ProjectName <ProjectName>
```
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

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
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

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
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

#### Register the attributes in the IoC container
Register MediatR attribute based infrastructure in IoC
```csharp
using Dzidek.Net.Infrastructure.AttributeBasedInfrastructure.Middlewares;
...
builder.Services
  .AddMediatRAttributeBasedInfrastructure();
```
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

#### Decorate query with your attribute
```csharp
[MyAttribute1]
[RetryPolicy]
internal sealed class AttributedImplementationQuery : IRequest<int> { }
```
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

#### Call sequence
Attributes are called from top to bottom, as shown in the following screen with red numbers that show the order of calling
![Order.png](Order.png)
The sequence of calls is similar to the DotNet middleware
![MediatR_AttributeBasedInfra.png](MediatR_AttributeBasedInfra.png)
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

#### Working example
Working example you can find in project repo https://github.com/DzidekDotNet/Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

### Changelog
- 1.0.0
    - Working example
    - Abstract attribute
    - IoC registration
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

## Nuget packages
[Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure](https://www.nuget.org/packages/Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure)
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

## Authors
[@DzidekDotNet](https://www.github.com/DzidekDotNet)
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

## License
[MIT](https://github.com/DzidekDotNet/Dzidek.Net.Infrastructure.MadiatR.AttributeBasedInfrastructure/blob/main/LICENSE)
<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>