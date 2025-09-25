namespace OrderService.Domain;

public sealed record Order(
    Guid Id,
    Guid BookId,
    string CustomerEmail,
    DateTimeOffset CreatedAt);
