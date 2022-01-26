namespace Meets.WebApi.Extensions;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder OnDevelopment(
        this IApplicationBuilder application,
        IWebHostEnvironment environment,
        Action action)
    {
        if (environment.IsDevelopment())
        {
            action();
        }

        return application;
    }
}
