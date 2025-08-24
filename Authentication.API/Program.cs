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
            return builder.Build();
        }

        private static void RunApplication(WebApplication app)
        {
            app.Run();
        }
    }
}
