using Projects;
using MassTransit;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<FileObserverService>(nameof(FileObserverService));

        builder.Build().Run();
    }
}
