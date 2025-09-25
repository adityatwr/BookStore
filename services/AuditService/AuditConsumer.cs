using Canonicals;
using MassTransit;

namespace AuditService
{
    public class AuditConsumer : IConsumer<BookAdded>, IConsumer<OrderPlaced>
    {
        private readonly ILogger<AuditConsumer> _logger;

        public AuditConsumer(ILogger<AuditConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<BookAdded> context)
        {
            _logger.LogInformation("Audit Log: Book Added {Id} {Title}", context.Message.BookId, context.Message.Title);
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            _logger.LogInformation("Audit Log: Order Placed {Id} {BookId} {Email}", context.Message.OrderId, context.Message.BookId, context.Message.CustomerEmail);
            return Task.CompletedTask;
        }
    }
}
