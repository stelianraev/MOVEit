using Projects;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<MoveitFileObserverService>(nameof(MoveitFileObserverService));
        builder.AddProject<MoveitWpf>(nameof(MoveitWpf));
        builder.AddProject<MoveitApi>(nameof(MoveitApi));


        builder.Build().Run();
    }
}
