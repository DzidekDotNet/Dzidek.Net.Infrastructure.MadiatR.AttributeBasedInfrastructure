using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTest_DotNet6.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class TestController : ControllerBase
{
  private readonly IMediator _mediator;
  public TestController(IMediator mediator) =>
    _mediator = mediator;
  
  [HttpGet("/TestQuery")]
  public async Task<IActionResult> TestQuery() =>
    Ok(await _mediator.Send(new TestQuery(), HttpContext.RequestAborted));
  
  [HttpGet("/TestQueryWithAttributes")]
  public async Task<IActionResult> TestQueryWithAttributes() =>
    Ok(await _mediator.Send(new TestQueryWithAttributes(), HttpContext.RequestAborted));
}
