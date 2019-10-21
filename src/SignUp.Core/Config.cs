using Microsoft.Extensions.Configuration;

namespace SignUp.Core
{
    public class Config
    {
        public static IConfiguration Current { get; private set; }

        static Config()
        {
            Current = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddJsonFile("configs/config.json", optional: true)
                .AddJsonFile("secrets/secret.json", optional: true)
                .Build();
        }
    }
}
