using Kds.Models;
using Kds.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kds.Controllers;

[ApiController]
[Route("[controller]")]
public class FlowController(ILogger<FlowController> logger, FlowService FlowService) : ControllerBase
{
    private readonly ILogger<FlowController> _logger = logger;
    private readonly FlowService _FlowService = FlowService;

    [HttpPost]
    public async Task CreateAsync([FromBody] Flow newFlow) => await _FlowService.CreateAsync(newFlow);

}
