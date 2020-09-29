using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SignUp.Core.Middleware
{
    public class FlakinessMiddleware
    {
        private static readonly Random _Random = new Random();

        private readonly RequestDelegate _next;

        public FlakinessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var doNext = true;
            if (Config.Current["Mode"]=="flaky")
            {
                var n = _Random.Next(1, 100);
                // flaky mode - 5% of calls fail:
                if (n <= 5)
                {
                    doNext = false;
                    using (var writer = new StreamWriter(context.Response.Body))
                    {
                        context.Response.StatusCode = 500; 
                        await writer.WriteAsync("Flaky mode error");
                    }
                }
                // 30% of calls are quite slow:
                if (n <= 35)
                {
                    Thread.Sleep(_Random.Next(1, 100));
                }
                // another 20% are slower:
                else if (n <= 55)
                {
                    Thread.Sleep(_Random.Next(200, 600));
                }
                // and 5% are very slow:
                else if (n > 95)
                {
                    Thread.Sleep(_Random.Next(1000, 5000));
                }
            }

            if (doNext)
            {
                await _next(context);
            }
        }
    }
}
