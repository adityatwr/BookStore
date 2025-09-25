namespace Canonicals;

public record BookAdded(Guid BookId, string Title, string Author);
public record OrderPlaced(Guid OrderId, Guid BookId, string CustomerEmail);


