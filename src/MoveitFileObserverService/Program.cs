using MoveitFileObserverService;
using MoveitFileObserverService.Models.Configuration;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<ServiceConfig>(builder.Configuration.GetSection("ServiceConfig"));

        builder.Services.AddHostedService<FileChangeWorker>();

        var host = builder.Build();

        host.Run();
    }
}