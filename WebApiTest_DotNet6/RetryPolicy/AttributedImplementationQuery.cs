using MediatR;

namespace WebApiTest_DotNet6.RetryPolicy;

// ReSharper disable once ClassNeverInstantiated.Global
[MyAttribute1]
[MyAttribute2]
[RetryPolicy]
internal sealed class AttributedImplementationQuery : IRequest<int> { }

internal sealed class AttributedImplementationQueryHandler : IRequestHandler<AttributedImplementationQuery, int>
{
  private readonly TestService _testService;
  
  public AttributedImplementationQueryHandler(TestService testService) => _testService = testService;

  public Task<int> Handle(AttributedImplementationQuery request, CancellationToken cancellationToken) => _testService.GetCounter();
}
