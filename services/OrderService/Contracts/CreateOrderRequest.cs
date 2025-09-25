namespace OrderService.Contracts;

public record CreateOrderRequest(Guid BookId, string CustomerEmail);
