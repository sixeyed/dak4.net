using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerArgs;
using SignUp.Core;
using SignUp.MessageHandlers.SaveProspectCore.Model;
using SignUp.MessageHandlers.SaveProspectCore.Workers;
using System;
using Prometheus;
using System.Runtime.InteropServices;

namespace SignUp.MessageHandlers.SaveProspectCore
{
    class Program
    {        
        private static readonly Gauge _InfoGauge = 
            Metrics.CreateGauge("app_info", "Application info", "dotnet_version", "version");

        public static int Main(string[] args)
        {
            _InfoGauge.Labels(RuntimeInformation.FrameworkDescription, "20.09").Set(1);
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton(Config.Current)
                .AddSingleton<CheckWorker>()
                .AddSingleton<QueueWorker>()
                .AddDbContext<SignUpContext>(options =>
                     options.UseSqlServer(Config.Current.GetConnectionString("SignUpDb"),
                     sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()))
                .BuildServiceProvider();

            var arguments = Args.Parse<Arguments>(args);
            switch (arguments.Mode)
            {
                case RunMode.Listen:
                    Console.WriteLine("Running in Listen mode...");
                    var worker = serviceProvider.GetService<QueueWorker>();
                    worker.Start();
                    break;

                case RunMode.Check:
                    Console.WriteLine("Running in Check mode...");
                    var check = serviceProvider.GetService<CheckWorker>();
                    return check.Run();
            }

            return 0;
        }
    }
}
