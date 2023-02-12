using Microsoft.AspNetCore.Mvc;
using ShortUrlApi.DTOs;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Redis
var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions { EndPoints = { builder.Configuration.GetConnectionString("Redis") } });
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

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

    var result = Results.Redirect(redisValue.ToString(), true, true);

    return result;
})
.WithName("GetURL")
.WithOpenApi();

app.MapPost("/p", async ([FromServices] IConnectionMultiplexer redis, PostShortUrlInput input) =>
{
    var cacheDb = redis.GetDatabase();

    var urlOutput = new PostShortUrlOutput(input.Origin);

    var expiration = input.Expiration.Subtract(DateTime.Now);

    await cacheDb.StringSetAsync(urlOutput.NewUrl, input.Origin, expiration);

    return urlOutput;
})
.WithName("PostURL")
.WithOpenApi(); ;

app.Run();