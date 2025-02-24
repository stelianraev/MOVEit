using MoveitApi.Helper;
using System.Reflection;

public static class EndpointBuilderExtension
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && typeof(IEndpoint).IsAssignableFrom(t))
            .ToList()
            .ForEach(t => ((IEndpoint)Activator.CreateInstance(t)!).Map(app));

        return app;
    }
}
