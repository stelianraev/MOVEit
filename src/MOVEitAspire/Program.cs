using Projects;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<MoveitApi>(nameof(MoveitApi));
        builder.AddProject<MoveitDesktopUI>(nameof(MoveitDesktopUI));
        builder.AddProject<MoveitFileObserverService>(nameof(MoveitFileObserverService));

        var app = builder.Build();
        app.Run();
    }
}
