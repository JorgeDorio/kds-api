using Kds.Models;
using Kds.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kds.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(ILogger<OrderController> logger, OrderService OrderService) : ControllerBase
{
    private readonly ILogger<OrderController> _logger = logger;
    private readonly OrderService _orderService = OrderService;

    [HttpPost]
    public async Task CreateAsync([FromBody] Order newOrder) => await _orderService.CreateAsync(newOrder);

    [HttpGet("{stationId}")]
    public async Task<Station> GetByStationId(string stationId) => await _orderService.GetByStationId(stationId);

    [HttpGet("{stationId}/next")]
    public async Task<Station> GetNextByStationId(string stationId) => await _orderService.GetNextByStationId(stationId);

    [HttpPut("{orderId}/start")]
    public async Task<IActionResult> StartOrder(string orderId) => await _orderService.StartOrder(orderId);

    [HttpPut("{orderId}/end")]
    public async Task EndOrder(string orderId) => await _orderService.EndOrder(orderId);
}
