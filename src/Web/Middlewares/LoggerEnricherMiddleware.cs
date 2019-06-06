using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Web.Middlewares
{
    public class LoggerEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _accessor;

        public LoggerEnricherMiddleware(
                RequestDelegate next, IHttpContextAccessor accessor)
        {
            _next = next;
            _accessor = accessor;
        }
        public async Task Invoke(HttpContext context)
        {
            var customerId = context.User.FindFirst("NameIdentifier");
            var name = context.User.FindFirstValue("Name");

            LogContext.PushProperty("CustomerIpAddress",
                _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString());

            if (customerId != null)
            {
                LogContext.PushProperty("CustomerId", customerId);
                LogContext.PushProperty("CustomerName", name);
            }
            
            
            await _next(context);
        }
    }
}