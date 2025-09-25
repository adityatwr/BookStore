namespace OrderService.Application;

public sealed class BookUnavailableException : Exception
{
    public Guid BookId { get; }
    public BookUnavailableException(Guid bookId)
        : base($"Book {bookId} is not available.") => BookId = bookId;
}
