using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Serilog;
using System.Runtime.InteropServices;

namespace SignUp.Web.Core
{
    public class Program
    {
        private static readonly Gauge _InfoGauge = 
            Metrics.CreateGauge("app_info", "Application info", "dotnet_version", "version");

        public static void Main(string[] args)
        {
            _InfoGauge.Labels(RuntimeInformation.FrameworkDescription, "20.09").Set(1);

            CreateHostBuilder(args).Build().Run();
        
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .WriteTo.File("/logs/signup-web.log")
               .CreateLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
