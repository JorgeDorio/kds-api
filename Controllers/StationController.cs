using Kds.Models;
using Kds.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kds.Controllers;

[ApiController]
[Route("[controller]")]
public class StationController(ILogger<StationController> logger, StationService stationService) : ControllerBase
{
    private readonly ILogger<StationController> _logger = logger;
    private readonly StationService _stationService = stationService;

    [HttpGet]
    public async Task<IEnumerable<Station>> GetAsync()
    {
        return await _stationService.GetAsync();
    }

    [HttpPost]
    public async Task CreateAsync(Station newStation)
    {
        await _stationService.CreateAsync(newStation);
    }
}
