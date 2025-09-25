using BookCatalogueService.Application;
using BookCatalogueService.Persistence;
using MassTransit;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Book Catalogue API",
        Version = "v1"
    });
});

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddMassTransit(s =>
{
    var isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    var rabbitHost = builder.Configuration["RABBITMQ__HOST"] ?? (isInContainer ? "rabbitmq" : "localhost");
    var rabbitPort = int.TryParse(builder.Configuration["RABBITMQ__PORT"], out var p) ? p : 5672;
    var rabbitUser = builder.Configuration["RABBITMQ__USER"] ?? "guest";
    var rabbitPass = builder.Configuration["RABBITMQ__PASS"] ?? "guest";
    
    s.UsingRabbitMq((ctx, cfg) =>
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
        c.DocumentTitle = "Book Catalogue API";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
