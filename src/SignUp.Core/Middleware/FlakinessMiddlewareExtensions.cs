using Microsoft.AspNetCore.Builder;
using SignUp.Core.Middleware;

namespace SignUp.Core.Middleware
{
    public static class FlakinessMiddlewareExtensions
    {
        public static IApplicationBuilder UseFlakiness(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FlakinessMiddleware>();
        }
    }
}