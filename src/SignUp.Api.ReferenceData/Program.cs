using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Prometheus;
using System.Runtime.InteropServices;

namespace SignUp.Api.ReferenceData
{
    public class Program
    {        
        private static readonly Gauge _InfoGauge = 
            Metrics.CreateGauge("app_info", "Application info", "dotnet_version", "version");

        public static void Main(string[] args)
        {
            _InfoGauge.Labels(RuntimeInformation.FrameworkDescription, "20.09").Set(1);
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
