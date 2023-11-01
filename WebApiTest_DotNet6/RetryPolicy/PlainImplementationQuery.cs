using MediatR;
using Polly.Contrib.WaitAndRetry;

namespace WebApiTest_DotNet6.RetryPolicy;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class PlainImplementationQuery : IRequest<int>
{

}

internal sealed class PlainImplementationQueryHandler : IRequestHandler<PlainImplementationQuery, int>
{
  private readonly TestService _testService;
  
  public PlainImplementationQueryHandler(TestService testService) =>
    _testService = testService;

  public async Task<int> Handle(PlainImplementationQuery request, CancellationToken cancellationToken)
  {
    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);
    var retryAttempt = 0;
    while (retryAttempt < 5)
    {
      try
      {
        return await _testService.GetCounter();
      }
      catch (Exception)
      {
        retryAttempt++;
        await Task.Delay(delay.ToArray()[retryAttempt]);
        if (retryAttempt >= 5)
        {
          throw;
        }
      }
    }
    throw new Exception();
  }
}
