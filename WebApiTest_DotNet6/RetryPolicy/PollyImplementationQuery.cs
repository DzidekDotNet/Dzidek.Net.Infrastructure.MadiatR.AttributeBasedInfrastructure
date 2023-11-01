using MediatR;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace WebApiTest_DotNet6.RetryPolicy;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class PollyImplementationQuery : IRequest<int>
{

}

internal sealed class PollyImplementationQueryHandler : IRequestHandler<PollyImplementationQuery, int>
{
  private readonly TestService _testService;
  
  public PollyImplementationQueryHandler(TestService testService) =>
    _testService = testService;

  public Task<int> Handle(PollyImplementationQuery request, CancellationToken cancellationToken)
  {
    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);
    return Policy
      .Handle<Exception>()
      .WaitAndRetryAsync(delay)
      .ExecuteAsync(async () => await _testService.GetCounter());
  }
}
