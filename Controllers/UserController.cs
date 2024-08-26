using Kds.Models;
using Kds.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kds.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ILogger<UserController> logger, UserService UserService) : ControllerBase
{
    private readonly ILogger<UserController> _logger = logger;
    private readonly UserService _UserService = UserService;

    [HttpPost("invite")]
    public async Task<string> GenerateInviteCode() => await _UserService.GenerateInviteCode();

}
