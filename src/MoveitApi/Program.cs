using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using MoveitApi.SignalR;
using MoveitClient;
using MoveitClient.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MoveitConfiguration>(builder.Configuration.GetSection("MoveitConfiguration"));
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue; // Api limit
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // Api limit
});

builder.Services.AddControllers();
builder.Services.AddHttpClient<IClient>();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IClient, Client>();
builder.Services.AddSingleton<CancellationTokenSource>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddLogging();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapHub<FileObserverHub>("/file-change/notify");

app.UseSwagger();
app.UseSwaggerUI();
app.MapEndpoints();
app.MapControllers();

app.Run();
