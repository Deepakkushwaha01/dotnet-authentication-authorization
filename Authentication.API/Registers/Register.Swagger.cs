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

            // JWT Bearer in Swagger UI
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
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new List<string>() }
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
