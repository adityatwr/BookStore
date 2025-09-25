using BookCatalogueService.Contracts;
using BookCatalogueService.Domain;

namespace BookCatalogueService.Application;

public interface IBookService
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Book> CreateAsync(CreateBookRequest request, CancellationToken ct = default);
}
