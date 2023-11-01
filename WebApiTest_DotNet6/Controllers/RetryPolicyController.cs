using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApiTest_DotNet6.RetryPolicy;

namespace WebApiTest_DotNet6.Controllers;

[ApiController]
[Route("[controller]")]
public class RetryPolicyController : ControllerBase
{
  private readonly IMediator _mediator;
  public RetryPolicyController(IMediator mediator) =>
    _mediator = mediator;
  
  [HttpGet("PlainImplementation")]
  public async Task<IActionResult> PlainImplementation() =>
    Ok(await _mediator.Send(new PlainImplementationQuery(), HttpContext.RequestAborted));
  
  [HttpGet("PollyImplementation")]
  public async Task<IActionResult> PollyImplementation() =>
    Ok(await _mediator.Send(new PollyImplementationQuery(), HttpContext.RequestAborted));
  
  [HttpGet("AbstractedImplementation")]
  public async Task<IActionResult> AbstractedImplementation() =>
    Ok(await _mediator.Send(new AbstractedImplementationQuery(), HttpContext.RequestAborted));
  
  [HttpGet("AttributedImplementation")]
  public async Task<IActionResult> AttributedImplementation() =>
    Ok(await _mediator.Send(new AttributedImplementationQuery(), HttpContext.RequestAborted));
}
