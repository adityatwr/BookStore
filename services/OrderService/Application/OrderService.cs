using Canonicals;
using MassTransit;
using OrderService.Contracts;
using OrderService.Domain;
using OrderService.Persistence;
using OrderService.Services;

namespace OrderService.Application;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IBooksGateway _books;
    private readonly IPublishEndpoint _bus;

    public OrderService(IOrderRepository repo, IBooksGateway books, IPublishEndpoint bus)
    {
        _repo = repo;
        _books = books;
        _bus = bus;
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default) =>
        _repo.GetAllAsync(ct);

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _repo.GetByIdAsync(id, ct);

    public async Task<Order> CreateAsync(CreateOrderRequest req, CancellationToken ct = default)
    {
        if (req.BookId == Guid.Empty)
            throw new ArgumentException("BookId is required", nameof(req.BookId));
        if (string.IsNullOrWhiteSpace(req.CustomerEmail))
            throw new ArgumentException("CustomerEmail is required", nameof(req.CustomerEmail));

        if (!await _books.ExistsAsync(req.BookId, ct))
            throw new BookUnavailableException(req.BookId);

        var order = new Order(
                    Id: Guid.NewGuid(),
                    BookId: req.BookId,
                    CustomerEmail: req.CustomerEmail.Trim(),
                    CreatedAt: DateTimeOffset.UtcNow);

        order = await _repo.AddAsync(order, ct);

        await _bus.Publish(new OrderPlaced(order.Id, order.BookId, order.CustomerEmail), ct);

        return order;
    }
}
