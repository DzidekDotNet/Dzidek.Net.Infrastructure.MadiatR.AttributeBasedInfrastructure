namespace WebApiTest_DotNet6;

internal sealed class TestService
{
  private readonly ILogger<TestService> _logger;
  private int _counter = 0;
  public TestService(ILogger<TestService> logger) =>
    _logger = logger;
  
  internal Task<int> GetCounter()
  {
    _counter++;
    _logger.LogInformation("GetCounter {0}", _counter);
    if (_counter % 6 == 0)
    {
      return Task.FromResult(_counter);
    }

    throw new Exception("Expected exception");
  }
}
