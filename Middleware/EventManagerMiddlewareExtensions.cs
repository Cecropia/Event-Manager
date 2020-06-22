using EventManager.BusinessLogic.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using System;

namespace EventManager.Middleware
{
    public static class EventManagerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCecEventManager(this IApplicationBuilder builder)
        {
            // see here for reference: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/e33dba2e416469fca6c2f4975c1c41dbac942804/src/Swashbuckle.AspNetCore.SwaggerUI/SwaggerUIBuilderExtensions.cs
            return builder.UseMiddleware<EventManagerMiddleware>();
        }

        public static IServiceCollection ConfigureCecEventManager(this IServiceCollection services)
        {
            //return services.AddTransient<ITestService, TestService>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .CreateLogger();

            services.TryAddSingleton(EventDispatcher.Instance);
            return services;
        }

        public static IServiceCollection ConfigureCecEventManager(this IServiceCollection services, Action<EventDispatcher> ed)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .CreateLogger();

            ed.Invoke(EventDispatcher.Instance);

            //return services.AddTransient<ITestService, TestService>();
            services.TryAddSingleton(EventDispatcher.Instance);
            return services;
        }
    }
}
