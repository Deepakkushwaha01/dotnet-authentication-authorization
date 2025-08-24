using Authentication.API.Registers;
using Authentication.API.Infrastructure.Settings;

namespace Dotnet.Authentication.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                var app = BuildApplication(builder);
                RunApplication(app);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private static WebApplication BuildApplication(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services
            .AddDatabaseRegister(builder.Configuration)
            .RegisterSwagger(builder.Configuration)
            .AddIdentityWithJwt(builder.Configuration)
            .RegisterVersioning();

            builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

            return builder.Build();
        }

        private static void RunApplication(WebApplication app)
        {
            app.UseSwaggerDocumentation(app.Configuration);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
