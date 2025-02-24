using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<MoveitApi>(nameof(MoveitApi));
builder.AddProject<MoveitFileObserverService>(nameof(MoveitFileObserverService));
builder.AddProject<MoveitDesktopUI>(nameof(MoveitDesktopUI));

var app = builder.Build();
app.Run();
