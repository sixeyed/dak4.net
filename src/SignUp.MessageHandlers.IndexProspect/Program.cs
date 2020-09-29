using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignUp.Core;
using SignUp.MessageHandlers.IndexProspect.Indexer;
using SignUp.MessageHandlers.IndexProspect.Workers;
using Prometheus;
using System.Runtime.InteropServices;

namespace SignUp.MessageHandlers.IndexProspect
{
    class Program
    {        
        private static readonly Gauge _InfoGauge = 
            Metrics.CreateGauge("app_info", "Application info", "dotnet_version", "version");

        public static void Main(string[] args)
        {
            _InfoGauge.Labels(RuntimeInformation.FrameworkDescription, "20.09").Set(1);
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton(Config.Current)
                .AddSingleton<Index>()
                .AddSingleton<QueueWorker>()
                .BuildServiceProvider();

            var worker = serviceProvider.GetService<QueueWorker>();
            worker.Start();
        }
    }
}
