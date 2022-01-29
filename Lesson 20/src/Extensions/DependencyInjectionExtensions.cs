namespace Meets.WebApi.Extensions;

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using Meets.WebApi.Filters;
using Meets.WebApi.Helpers;
using Meets.WebApi.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            var projectDirectory = AppContext.BaseDirectory;
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;
            var xmlFileName = $"{projectName}.xml";
            options.IncludeXmlComments(Path.Combine(projectDirectory, xmlFileName));
            options.CustomSchemaIds(modelType => modelType.FullName);
            
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Put Your access token here (drop **Bearer** prefix):",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            options.OperationFilter<OpenApiAuthFilter>();
        });

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddScoped<JwtTokenHelper>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSecret = Encoding.ASCII.GetBytes(configuration["JwtAuth:Secret"]);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSecret),

                    ValidateAudience = false,
                    ValidateIssuer = false,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.RequireHttpsMetadata = false;

                var tokenHandler = options.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().Single();
                tokenHandler.InboundClaimTypeMap.Clear();
                tokenHandler.OutboundClaimTypeMap.Clear();
            });

        return services;
    }
    
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<DatabaseContext>(options =>
        {
            var connectionString = configuration.GetPostgreSqlConnectionString();
            options.UseNpgsql(connectionString);
        });
}
