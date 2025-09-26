using Canonicals;
using MassTransit;

namespace AuditService.Consumers;

public sealed class BookAddedConsumer : IConsumer<BookAdded>
{
    private readonly ILogger<BookAddedConsumer> _logger;

    public BookAddedConsumer(ILogger<BookAddedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<BookAdded> ctx)
    {
        _logger.LogInformation(
            "Audit BookAdded: {BookId} {Title} by {Author}",
            ctx.Message.BookId, ctx.Message.Title, ctx.Message.Author);
        return Task.CompletedTask;
    }
}
