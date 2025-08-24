namespace Authentication.API.Settings
{
    public class ConnectionStringSettings(IConfiguration configuration)
    {
        public string IntegrationDbConnectionString { get; } = configuration.GetValue<string>("ConnectionStrings:IntegrationDbConnectionString")!;
    }
}