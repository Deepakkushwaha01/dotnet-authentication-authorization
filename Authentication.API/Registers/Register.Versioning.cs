using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Registers;

public static partial class Register
{
    public static IServiceCollection RegisterVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });
        return services;
    }
}
