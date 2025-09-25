using Canonicals;
using MassTransit;
using Microsoft.OpenApi.Models;
using OrderService.Application;
using OrderService.Persistence;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order service API",
        Version = "v1"
    });
});

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService.Application.OrderService>();

var catalogueBase = builder.Configuration["BOOKS_BASE_URL"];

//builder.Services.AddHttpClient("books", c => c.BaseAddress = new Uri(catalogueBase));
builder.Services.AddHttpClient<IBooksGateway, HttpBooksGateway>(c =>
{
    c.BaseAddress = new Uri(catalogueBase);
    c.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddMassTransit(x =>
{
    var isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    var rabbitHost = builder.Configuration["RABBITMQ__HOST"] ?? (isInContainer ? "rabbitmq" : "localhost");
    var rabbitPort = int.TryParse(builder.Configuration["RABBITMQ__PORT"], out var p) ? p : 5672;
    var rabbitUser = builder.Configuration["RABBITMQ__USER"] ?? "guest";
    var rabbitPass = builder.Configuration["RABBITMQ__PASS"] ?? "guest";
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitHost, (ushort)rabbitPort, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });
    });
});

builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "Order service API";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();