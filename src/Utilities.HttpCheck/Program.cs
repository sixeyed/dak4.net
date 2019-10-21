using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Utilities.HttpCheck
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()                
                .AddJsonFile("configs/config.json", optional: true)
                .AddJsonFile("secrets/secret.json", optional: true)
                .Build();

            var targetUrl = config["HttpCheck:TargetUrl"];
            var timeout = int.Parse(config["HttpCheck:Timeout"]);

            var exitCode = 1;
            try
            {
                using (var client = new HttpClient())
                {
                    var stopwatch = Stopwatch.StartNew();
                    var response = await client.GetAsync(targetUrl);
                    stopwatch.Stop();
                    Console.WriteLine($"HEALTHCHECK: status {response.StatusCode}, took {stopwatch.ElapsedMilliseconds}ms");
                    if (response.StatusCode == HttpStatusCode.OK && stopwatch.ElapsedMilliseconds < timeout)
                    {
                        exitCode = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HEALTHCHECK: error. Exception {ex.Message}");
            }
            return exitCode;
        }
    }
}
