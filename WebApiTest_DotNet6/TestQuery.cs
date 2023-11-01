namespace WebApiTest_DotNet6;
using MediatR;

internal sealed class TestQuery : IRequest<int>
{

}

internal sealed class TestQueryHandler : IRequestHandler<TestQuery, int>
{
  private readonly TestService _testService;
  public TestQueryHandler(TestService testService) =>
    _testService = testService;

  public Task<int> Handle(TestQuery request, CancellationToken cancellationToken) =>
    _testService.GetCounter();
}