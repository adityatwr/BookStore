using BookCatalogueService.Application;
using BookCatalogueService.Contracts;
using BookCatalogueService.Domain;
//using BookCatalogueService.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogueService.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;
    private readonly IPublishEndpoint _bus;

    public BooksController(IBookService service, IPublishEndpoint bus)
    {
        _service = service;
        _bus = bus;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Book>> GetById(Guid id, CancellationToken ct)
    {
        var book = await _service.GetByIdAsync(id);
        return book is null ? NotFound() : Ok(book);

    }

    [HttpPost]
    public async Task<ActionResult<Book>> Create([FromBody] CreateBookRequest req, CancellationToken ct)
    {
        var book = await _service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }
}
