using BookCatalogueService.Domain;

namespace BookCatalogueService.Persistence;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Book> AddAsync(Book book, CancellationToken ct = default);
}
