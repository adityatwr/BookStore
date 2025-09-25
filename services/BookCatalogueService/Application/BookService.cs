using BookCatalogueService.Contracts;
using BookCatalogueService.Domain;
using BookCatalogueService.Persistence;
using Canonicals;
using MassTransit;

namespace BookCatalogueService.Application;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _repo;
    private readonly IPublishEndpoint _bus;

    public BookService(IBookRepository repo, IPublishEndpoint bus)
    {
        _repo = repo;
        _bus = bus;
    }

    public Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default)
        => _repo.GetAllAsync(ct);

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _repo.GetByIdAsync(id, ct);

    public async Task<Book> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required", nameof(request.Title));
        if (string.IsNullOrWhiteSpace(request.Author))
            throw new ArgumentException("Author is required", nameof(request.Author));

        var book = new Book(Guid.NewGuid(), request.Title.Trim(), request.Author.Trim());
        book = await _repo.AddAsync(book, ct);

        await _bus.Publish(new BookAdded(book.Id, book.Title, book.Author), ct);

        return book;
    }
}
