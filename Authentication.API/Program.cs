using Authentication.API.Registers;

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
            .RegisterSwagger(builder.Configuration);

            return builder.Build();
        }

        private static void RunApplication(WebApplication app)
        {
            app.UseSwaggerDocumentation(app.Configuration);
            app.Run();
        }
    }
}
