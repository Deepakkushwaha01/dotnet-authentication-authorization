using Microsoft.OpenApi.Models;

namespace Authentication.API.Registers;

public static partial class Register
{
    public static IServiceCollection RegisterSwagger(this IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = config["Swagger:Title"],
                Version = config["Swagger:Version"],
                Description = config["Swagger:Description"],
            });

            // ✅ JWT Bearer definition
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}'"
            };

            o.AddSecurityDefinition("Bearer", securityScheme);

            // ✅ Security Requirement: Link the definition to all endpoints with [Authorize]
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"  // Must match AddSecurityDefinition name
                        }
                    },
                    new string[] {} // Roles can be empty for all authorized endpoints
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration config)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(config["Swagger:JsonEndpointUrl"], config["Swagger:Title"]);
            c.RoutePrefix = config["Swagger:RoutePrefix"];
        });
        return app;
    }
}
