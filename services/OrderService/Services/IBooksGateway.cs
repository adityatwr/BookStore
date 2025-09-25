namespace OrderService.Services;

public interface IBooksGateway
{
    Task<bool> ExistsAsync(Guid bookId, CancellationToken ct = default);
}

