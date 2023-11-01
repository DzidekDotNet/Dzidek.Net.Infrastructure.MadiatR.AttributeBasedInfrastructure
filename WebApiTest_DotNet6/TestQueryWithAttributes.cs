namespace WebApiTest_DotNet6;
using MediatR;

[MyAttribute1]
[MyAttribute2]
[MyAttribute3]
internal sealed class TestQueryWithAttributes : IRequest<int>
{

}

internal sealed class TestQueryWithAttributesHandler : IRequestHandler<TestQueryWithAttributes, int>
{
  private readonly TestService _testService;
  public TestQueryWithAttributesHandler(TestService testService) =>
    _testService = testService;

  public Task<int> Handle(TestQueryWithAttributes request, CancellationToken cancellationToken) =>
    _testService.GetCounter();
}