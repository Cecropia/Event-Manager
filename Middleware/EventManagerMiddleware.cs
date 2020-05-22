using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EventManager.Middleware
{
    public class EventManagerMiddleware
    {
        private readonly RequestDelegate _next;

        public EventManagerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMyScopedService is injected into Invoke
        public async Task InvokeAsync(HttpContext httpContext)
        {
            //svc.somee = 100;
            //Console.WriteLine(svc.DoubleMessage("someMessage"));

            // Call the next delegate/middleware in the pipeline
            await _next(httpContext);
        }

        // public async Task InvokeAsync(HttpContext context)
        // {
        //     var cultureQuery = context.Request.Query["culture"];
        //     if (!string.IsNullOrWhiteSpace(cultureQuery))
        //     {
        //         var culture = new CultureInfo(cultureQuery);

        //         CultureInfo.CurrentCulture = culture;
        //         CultureInfo.CurrentUICulture = culture;

        //     }

        //     // Call the next delegate/middleware in the pipeline
        //     await _next(context);
        // }
    }
}