using MediatR;

namespace WebApiTest_DotNet6.RetryPolicy;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class AbstractedImplementationQuery : IRequest<int> { }

internal sealed class AbstractedImplementationQueryHandler : IRequestHandler<AbstractedImplementationQuery, int>
{
  private readonly TestService _testService;
  private readonly IRetryPolicyService _retryPolicyService;
  
  public AbstractedImplementationQueryHandler(TestService testService, IRetryPolicyService retryPolicyService)
  {
    _testService = testService;
    _retryPolicyService = retryPolicyService;
  }

  public Task<int> Handle(AbstractedImplementationQuery request, CancellationToken cancellationToken) =>
    _retryPolicyService.CallWithRetryAsync(async _ => await _testService.GetCounter(), cancellationToken);
}
