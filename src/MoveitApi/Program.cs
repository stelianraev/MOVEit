using FluentValidation;
using MoveitApiClient;
using MoveitApiClient.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MoveitConfiguration>(builder.Configuration.GetSection("MoveitConfiguration"));

builder.Services.AddControllers();
builder.Services.AddHttpClient<MoveitClient>();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<MoveitClient>();
builder.Services.AddSingleton<CancellationTokenSource>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddLogging();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapEndpoints();

app.MapControllers();

app.Run();
