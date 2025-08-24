using Authentication.API.Data;
using Authentication.API.Settings;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Registers
{
    public static partial class Registers
    {
        public static IServiceCollection AddDatabaseRegister(this IServiceCollection services, IConfiguration configuration)
        {
            ConnectionStringSettings connectionStringSettings = new(configuration);
            services.AddScoped<IAppDbContext, AppDbContext>().AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionStringSettings.IntegrationDbConnectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                            sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(15),
                            errorNumbersToAdd: null);
                });
            });
            return services;
        }
    }
}