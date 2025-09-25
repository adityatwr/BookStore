using System.Collections.Concurrent;
using OrderService.Domain;

namespace OrderService.Persistence;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Order>>(_store.Values.OrderByDescending(o => o.CreatedAt).ToList());

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(_store.TryGetValue(id, out var o) ? o : null);

    public Task<Order> AddAsync(Order order, CancellationToken ct = default)
    {
        _store[order.Id] = order;
        return Task.FromResult(order);
    }
}
