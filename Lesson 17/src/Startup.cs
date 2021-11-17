namespace Meets.WebApi;

using System.Reflection;
using Meets.WebApi.Extensions;

internal class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) =>
        _configuration = configuration;

    public void ConfigureServices(IServiceCollection services) =>
        services
            .AddSwagger()
            .AddJwtAuthentication(_configuration)
            .AddDbContext(_configuration)
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddControllers();

    public void Configure(IApplicationBuilder application, IWebHostEnvironment environment) =>
        application
            .OnDevelopment(environment, () =>
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            })
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => endpoints.MapControllers());
}
