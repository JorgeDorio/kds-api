using Kds.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kds.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ILogger<UserController> logger, UserService userService) : ControllerBase
{
    private readonly ILogger<UserController> _logger = logger;
    private readonly UserService _userService = userService;

    [HttpPost("invite")]
    public async Task<string> GenerateInviteCode() => await _userService.GenerateInviteCode();

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] User user) => await _userService.CreateAsync(user);

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] User user) => await _userService.LoginAsync(user);

    [HttpGet]
    public async Task<IEnumerable<User>> GetAllAsync() => await _userService.GetAllAsync();
}
