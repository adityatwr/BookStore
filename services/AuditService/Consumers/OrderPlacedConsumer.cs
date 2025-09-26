using Canonicals;
using MassTransit;

namespace AuditService.Consumers;

public sealed class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(ILogger<OrderPlacedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderPlaced> ctx)
    {
        _logger.LogInformation(
            "Audit OrderPlaced: {OrderId} {BookId} {Email}",
            ctx.Message.OrderId, ctx.Message.BookId, ctx.Message.CustomerEmail);
        return Task.CompletedTask;
    }
}
