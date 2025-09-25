using Microsoft.AspNetCore.Mvc;
using OrderService.Application;
using OrderService.Contracts;
using OrderService.Domain;

namespace OrderService.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Order>> GetById(Guid id, CancellationToken ct)
    {
        var order = await _service.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create([FromBody] CreateOrderRequest req, CancellationToken ct)
    {
        try
        {
            var order = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch(BookUnavailableException ex)
        {
            return BadRequest(new { error = ex.Message, bookId = ex.BookId });
        }
        catch(ArgumentException ex)
        {
            return ValidationProblem();
        }
    }
}
