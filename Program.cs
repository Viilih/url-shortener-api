using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UrlShortner.Api.Database;
using UrlShortner.Api.Extensions;
using UrlShortner.Api.Features.ShortenUrl.CreateShortenUrl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(builder
    .Configuration
    .GetConnectionString("PostgresDbConnection")));

builder.Services.AddRedisConfiguration(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddScoped<CreateShortUrlHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }