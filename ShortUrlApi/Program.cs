using Microsoft.AspNetCore.Mvc;
using ShortUrlApi.DTOs;
using ShortUrlApi.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Redis
var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions { EndPoints = { builder.Configuration.GetConnectionString("Redis") } });
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

builder.Services.AddTransient<IShortUrlService, ShortUrlService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/g/{url}", async ([FromServices] IConnectionMultiplexer redis, string url) =>
{
    var cacheDb = redis.GetDatabase();

    string decodedUrl = Uri.UnescapeDataString(url);

    var redisValue = await cacheDb.StringGetAsync(decodedUrl);

    return redisValue.HasValue ?
        Results.Redirect(redisValue.ToString(), true, true) :
        Results.NotFound(url);
})
.WithName("GetURL")
.WithOpenApi();

app.MapPost("/p", async ([FromServices] IConnectionMultiplexer redis, [FromServices] IShortUrlService shortUrlService, PostShortUrlInput input) =>
{
    if (!input.IsValidUri()) return Results.Problem($"Invalid Target URI [{input.Target}]");

    var expiration = input.Expiration > DateTime.Now ?
        input.Expiration.Subtract(DateTime.Now) :
        TimeSpan.FromHours(1); // como padrão deixa acessivél por 1h

    var urlOutput = new PostShortUrlOutput(shortUrlService.Generate());

    var cacheDb = redis.GetDatabase();

    await cacheDb.StringSetAsync(urlOutput.NewUrl, input.Target, expiration);

    return Results.Ok(urlOutput);
})
.WithName("PostURL")
.WithOpenApi(); ;

app.Run();
