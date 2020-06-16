using EventManager.BusinessLogic.Entities;
using EventManager.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EventManager.Middleware
{
    public class EventManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EventDispatcher EventDispatcher;

        public EventManagerMiddleware(RequestDelegate next)
        {
            _next = next;
            EventDispatcher = EventDispatcher.Instance;
        }

        // IMyScopedService is injected into Invoke
        public async Task InvokeAsync(HttpContext httpContext)
        {
            string method = httpContext.Request.Method.ToLower();
            string path = httpContext.Request.Path;
            int statusCode = httpContext.Response.StatusCode;

            if (("post" == method) && (path == EventManagerConstants.EventReceptionPath))
            {
                string responseBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();

                JObject json = JObject.Parse(responseBody);

                Event e = new Event()
                {
                    Name = (string)json["Name"],
                    Timestamp = (DateTime)json["Timestamp"],
                    Payload = json["Payload"] as JObject,
                    ExtraParams = json["ExtraParams"] as JObject,
                };

                EventDispatcher.Dispatch(e);

                httpContext.Response.StatusCode = 200;
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(httpContext);
            }

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
