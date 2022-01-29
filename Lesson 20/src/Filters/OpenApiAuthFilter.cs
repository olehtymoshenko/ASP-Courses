namespace Meets.WebApi.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

internal class OpenApiAuthFilter : IOperationFilter
{
    private readonly OpenApiSecurityRequirement _authenticationRequirement;

    public OpenApiAuthFilter() =>
        _authenticationRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var markedWithAuthorize = actionMetadata.Any(metadataItem => metadataItem is AuthorizeAttribute);
        
        if (markedWithAuthorize)
        {
            operation.Security.Add(_authenticationRequirement);
        }
    }
}
