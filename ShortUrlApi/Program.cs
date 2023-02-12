using ShortUrlApi.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapGet("/g", () =>
{
    var result = Results.Redirect("https://www.google.com", true, true);

    return result;
})
.WithName("GetURL")
.WithOpenApi();

app.MapPost("/p", (PostShortUrlInput input) =>
{
    var urlOutput = new PostShortUrlOutput(input.Origin);

    return urlOutput;
})
.WithName("PostURL")
.WithOpenApi(); ;

app.Run();