using System.Collections.Concurrent;
using BookCatalogueService.Domain;

namespace BookCatalogueService.Persistence;

public sealed class InMemoryBookRepository : IBookRepository
{
    private readonly ConcurrentDictionary<Guid, Book> _store = new();

    public Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Book>>(_store.Values.OrderBy(b => b.Title).ToList());

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var book) ? book : null);

    public Task<Book> AddAsync(Book book, CancellationToken ct = default)
    {
        _store[book.Id] = book;
        return Task.FromResult(book);
    }
}
