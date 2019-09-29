using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignUp.Core;
using SignUp.MessageHandlers.SaveProspectCore.Model;
using SignUp.MessageHandlers.SaveProspectCore.Workers;
using System;

namespace SignUp.MessageHandlers.SaveProspectCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(Config.Current)
                .AddSingleton<QueueWorker>()
                .AddDbContext<SignUpContext>(options =>
                     options.UseSqlServer(Config.Current.GetConnectionString("SignUpDb"),
                     sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()))
                .BuildServiceProvider();

            var worker = serviceProvider.GetService<QueueWorker>();
            worker.Start();
        }
    }
}
