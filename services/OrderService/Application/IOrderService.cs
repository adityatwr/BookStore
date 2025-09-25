using OrderService.Contracts;
using OrderService.Domain;

namespace OrderService.Application;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Order> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
}
