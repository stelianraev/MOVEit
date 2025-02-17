using MoveitApiClient;
using Movit.API.Helper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<MoveitClient>();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<MoveitClient>();
builder.Services.AddSingleton<CancellationTokenSource>();

builder.Services.AddLogging();
//builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapEndpoints();

app.MapControllers();

app.Run();
