using AuditService;
using AuditService.Consumers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    var isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    var rabbitHost = builder.Configuration["RABBITMQ__HOST"] ?? (isInContainer ? "rabbitmq" : "localhost");
    var rabbitPort = int.TryParse(builder.Configuration["RABBITMQ__PORT"], out var p) ? p : 5672;
    var rabbitUser = builder.Configuration["RABBITMQ__USER"] ?? "guest";
    var rabbitPass = builder.Configuration["RABBITMQ__PASS"] ?? "guest";

    x.AddConsumer<BookAddedConsumer>();
    x.AddConsumer<OrderPlacedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitHost, (ushort)rabbitPort, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.ReceiveEndpoint("Audit", e =>
        {
            e.ConfigureConsumer<BookAddedConsumer>(ctx);
            e.ConfigureConsumer<OrderPlacedConsumer>(ctx);
        });
    });
});

var host = builder.Build();
host.Run();
